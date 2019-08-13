using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TranslationOrganizer
{
    class TranslationOrganizer
    {
        // 翻译文本表
        public static List<string> s_TableTranslation;
        // 每个bank（严格来说应该是每两个bank）最多允许的字符个数
        private static readonly int MAX_UNIQUE_CHAR_COUNT = 29;

        private struct TextBlockInfo
        {
            public int bankNo; // 文本bank号（0为默认bank（0x7E, 0x7F））
            public string str; // 文本内容
            public List<Byte> data; // 文本字节码
        }
        private struct TranslationInfo
        {
            public int group;  // 组
            public int addrLo; // 保存文本首地址的低八位的地址
            public int addrHi; // 保存文本首地址的高八位的地址
            public int addr;   // 文本首地址
            public List<TextBlockInfo> textBlocks; // 文本块列表
        }
        private static List<TranslationInfo> s_TranslationInfos;
        private static Dictionary<int, string> s_DicUniqueChars;
        private static Dictionary<int, string> s_DicUniqueASCII;

        private static Dictionary<string, Byte> s_Ctrl2Code = new Dictionary<string, Byte>
        {
            { "[CR]", 0xF0 },
            { "[CLS]", 0xF1 },
            { "[NEXT]", 0xF2 },
            { "[FIN]", 0xFF },
        };

        public static List<Byte> s_CharOffsets = new List<Byte>
        {
            0x60, 0x61, 0x62, 0x63,
            0x64, 0x65, 0x66, 0x67,
            0x68, 0x69, 0x6A, 0x6B,
            0x6C, 0x6D, 0x6E, 0x6F,
            0x70, 0x71, 0x72, 0x73,
            0x74, 0x75, 0x76, 0x77,
            0x78, 0x79, 0x7A,
            0x7C,       0x7E,
        };

        public static List<Byte> s_ASCIIOffsets = new List<Byte>
        {
            0xEC, 0xED, 0xEE, 0xF4, 0xF5, 0xFC, 0xFD, 0xFE, 0xFF
        };

        public static void Start(string TranslationFileName, string ROMFileName)
        {
            LoadTranslationTable(TranslationFileName);
            GetInfos();
            ProcessInfos();
            dump();
            Patch(ROMFileName);
        }

        /// <summary>
        /// 载入翻译文本
        /// </summary>
        /// <param name="TranslationFileName">翻译文本文件名</param>
        private static void LoadTranslationTable(string TranslationFileName)
        {
            FileStream fs = new FileStream(TranslationFileName, FileMode.Open);
            if (fs != null)
            {
                StreamReader sr = new StreamReader(fs);
                if (sr != null)
                {
                    s_TableTranslation = new List<string>();
                    List<string> splitContents = new List<string>();
                    while (!sr.EndOfStream)
                    {
                        string content = sr.ReadLine();
                        if (content.Trim().Equals(string.Empty))
                            continue;
                        // 原始文本保存到翻译文本表
                        s_TableTranslation.Add(content);
                    }
                    sr.Close();
                    sr.Dispose();
                    sr = null;
                }
                fs.Close();
                fs.Dispose();
                fs = null;
            }
        }

        /// <summary>
        /// 语法解析，获得每个文本
        /// </summary>
        private static void GetInfos()
        {
            s_TranslationInfos = new List<TranslationInfo>();
            foreach(string line in s_TableTranslation)
            {
                string[] contents = line.Split(',');
                // 逐句检测，一旦发现未定义bank号的句子，就停止解析
                if(contents.Length != 6)
                {
                    return;
                }
                TranslationInfo info = new TranslationInfo();
                info.group = int.Parse(contents[0]);
                info.addrLo = int.Parse(contents[1], System.Globalization.NumberStyles.AllowHexSpecifier);
                info.addrHi = int.Parse(contents[2], System.Globalization.NumberStyles.AllowHexSpecifier);
                info.addr = int.Parse(contents[3], System.Globalization.NumberStyles.AllowHexSpecifier);
                info.textBlocks = new List<TextBlockInfo>();
                // 以[NEXT]为分割符进行分割
                List<string> subContents = SplitContent(contents[5]);
                foreach(string str in subContents)
                {
                    TextBlockInfo ti = new TextBlockInfo();
                    ti.bankNo = findBankNo(str);
                    ti.str = str;
                    ti.data = new List<Byte>();
                    info.textBlocks.Add(ti);
                }
                s_TranslationInfos.Add(info);
            }
        }

        /// <summary>
        /// 搜索字符串，查找{XX}，从而获得bankNo
        /// </summary>
        /// <param name="content">待查找的字符串</param>
        /// <returns>bankNo</returns>
        private static int findBankNo(string content)
        {
            int pos0 = content.IndexOf('{');
            int pos1 = content.IndexOf('}');
            if(pos0 >= 0 && pos1 == pos0 + 3)
            {
                return int.Parse(content.Substring(pos0 + 1, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
            }
            return 0;
        }

        /// <summary>
        /// 文本处理
        /// </summary>
        private static void ProcessInfos()
        {
            s_DicUniqueChars = new Dictionary<int, string>();
            s_DicUniqueASCII = new Dictionary<int, string>();
            int nextAddr = 0;
            for(int i = 0; i < s_TranslationInfos.Count; ++i) {
                TranslationInfo info = s_TranslationInfos[i];
                if (nextAddr == 0)
                    nextAddr = info.addr;
                info.addr = nextAddr;
                
                for(int j = 0; j < info.textBlocks.Count; ++j)
                {
                    TextBlockInfo ti = info.textBlocks[j];

                    // 如果bankNo为0，说明使用了默认bank。一般来说这是有问题的！不过也不排除某种极端情况。安全起见给出个警告吧！
                    if(ti.bankNo > 0)
                    {
                        if (!s_DicUniqueChars.ContainsKey(ti.bankNo))
                            s_DicUniqueChars.Add(ti.bankNo, string.Empty);
                        if (!s_DicUniqueASCII.ContainsKey(ti.bankNo))
                            s_DicUniqueASCII.Add(ti.bankNo, string.Empty);
                        s_DicUniqueChars[ti.bankNo] = GetUniqueChars(s_DicUniqueChars[ti.bankNo] + ti.str);
                        s_DicUniqueASCII[ti.bankNo] = GetUniqueASCII(s_DicUniqueASCII[ti.bankNo] + ti.str);
                        if (s_DicUniqueChars[ti.bankNo].Length > MAX_UNIQUE_CHAR_COUNT)
                        {
                            Console.WriteLine("FATAL ERROR! Too many characters in bank " + ti.bankNo);
                            Console.WriteLine(s_DicUniqueChars[ti.bankNo]);
                        }
                    }
                    else
                    {
                        Console.WriteLine("WARNING! bankNo == 0!");
                    }

                    char[] chars = ti.str.ToArray();
                    int start = -1;
                    for (int k = 0; k < chars.Length; ++k)
                    {
                        char c = chars[k];
                        switch (c)
                        {
                            case '[':
                                {
                                    start = k;
                                    break;
                                }
                            case ']':
                                {
                                    string control = ti.str.Substring(start, k - start + 1);
                                    start = -1;
                                    Byte v = 0xFF;
                                    if (s_Ctrl2Code.ContainsKey(control))
                                        v = s_Ctrl2Code[control];
                                    ti.data.Add(v);
                                    break;
                                }
                            case '<':
                                {
                                    start = k;
                                    break;
                                }
                            case '>':
                                {
                                    string value = ti.str.Substring(start + 1, k - start - 1);
                                    start = -1;
                                    Byte v = (Byte)int.Parse(value, System.Globalization.NumberStyles.AllowHexSpecifier);
                                    ti.data.Add(v);
                                    break;
                                }
                            case '{':
                                {
                                    start = k;
                                    break;
                                }
                            case '}':
                                {
                                    string value = ti.str.Substring(start + 1, k - start - 1);
                                    start = -1;
                                    Byte bankNo = (Byte)int.Parse(value, System.Globalization.NumberStyles.AllowHexSpecifier);
                                    ti.data.Add((Byte)(bankNo + 0x20));
                                    break;
                                }
                            default:
                                {
                                    if (start < 0)
                                    {
                                        if (c <= 0x7F)
                                        {
                                            if(c == ' ')
                                            {
                                                ti.data.Add(0xEF);
                                            }
                                            else
                                            {
                                                int offset = s_DicUniqueASCII[ti.bankNo].IndexOf(c);
                                                ti.data.Add(s_ASCIIOffsets[offset]);
                                            }
                                        }
                                        else
                                        {
                                            int offset = s_DicUniqueChars[ti.bankNo].IndexOf(c);
                                            ti.data.Add(s_CharOffsets[offset]);
                                        }
                                    }
                                    break;
                                }
                        }
                    }
                    nextAddr += ti.data.Count;
                }
                s_TranslationInfos[i] = info;
            }
        }

        /// <summary>
        /// 获得不重复汉字
        /// </summary>
        /// <param name="str">待处理的字符串</param>
        /// <returns>不重复的汉字字符串</returns>
        private static string GetUniqueChars(string str)
        {
            string ret = string.Empty;
            char[] chars = str.ToArray();
            foreach (char c in chars)
            {
                if(c >= 0x80 && !ret.Contains(c))
                {
                    ret += c;
                }
            }
            return ret;
        }

        /// <summary>
        /// 获得不重复ASCII
        /// </summary>
        /// <param name="str">待处理的字符串</param>
        /// <returns>不重复的ASCII字符串</returns>
        private static string GetUniqueASCII(string str)
        {
            string ret = string.Empty;
            char[] chars = str.ToArray();
            bool isInControlMode = false;
            foreach (char c in chars)
            {
                if (c <= 0 || c >= 0x80)
                    continue;
                if (c == '[' || c == '<' || c == '{')
                {
                    isInControlMode = true;
                }
                else if (c == ']' || c == '>' || c == '}')
                {
                    isInControlMode = false;
                }
                else if (!isInControlMode)
                {
                    if (!ret.Contains(c))
                        ret += c;
                }
            }
            return ret;
        }

        private static void dump()
        {
            foreach(TranslationInfo info in s_TranslationInfos)
            {
                string contents = string.Empty;
                foreach(TextBlockInfo ti in info.textBlocks)
                {
                    contents += ti.str;
                }
                string str = string.Format("{0},{1:X4},{2:X4},{3:X4},{4},{5}", info.group, info.addrLo, info.addrHi, info.addr, 1, contents);
                Console.WriteLine(str);
            }
            foreach(KeyValuePair<int, string> kv in s_DicUniqueChars)
            {
                Console.WriteLine(string.Format("{0},{1},{2}", kv.Key, kv.Value.Length, kv.Value));
            }
        }

        /// <summary>
        /// 给ROM打补丁
        /// </summary>
        /// <param name="fileName">ROM文件名</param>
        private static void Patch(string fileName)
        {
            Byte[] prgData = Common.GetPRGData(fileName);
            Byte[] chrData = Common.GetCHRData(fileName);

            foreach (TranslationInfo info in s_TranslationInfos)
            {
                int addrLo = TableOrganizer.GetAbsoluteAddress(info.addrLo);
                int addrHi = TableOrganizer.GetAbsoluteAddress(info.addrHi);
                int addr = TableOrganizer.GetAbsoluteAddress(info.addr);
                prgData[addrLo] = (Byte)((info.addr >> 0) & 0xFF);
                prgData[addrHi] = (Byte)((info.addr >> 8) & 0xFF);
                int offset = 0;
                foreach(TextBlockInfo ti in info.textBlocks)
                {
                    for (int i = 0; i < ti.data.Count; ++i)
                        prgData[addr + offset + i] = ti.data[i];
                    offset += ti.data.Count;
                }
            }
            foreach (KeyValuePair<int, string> kv in s_DicUniqueChars)
            {
                int bankNo = kv.Key;
                string uniqueChars = kv.Value;
                string uniqueASCII = s_DicUniqueASCII[bankNo];
                TileCreator.GeneratePatternTable(ref chrData, bankNo, uniqueChars, uniqueASCII);
            }

            Common.PatchROM(fileName, prgData, chrData);
        }

        /// <summary>
        /// 根据指定规则分割内容字符串
        /// 规则如下：
        /// 搜索[NEXT]，如果不存在的话，那么这句话无需分割
        /// 如果存在，则按[NEXT]分割
        /// 但如果分割之后的字符串内容只包含控制符而不包含有效字符，则该内容合并到它的前面那个字符串
        /// 该合并检测要递归，直到无法再合并为止
        /// </summary>
        /// <param name="content">内容字符串</param>
        /// <returns>分割好的内容列表</returns>
        private static List<string> SplitContent(string content)
        {
            List<string> ret = null;
            // 按[NEXT]分割
            string[] splitContent = content.Split(new string[] { "[NEXT]" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < splitContent.Length - 1; ++i)
            {
                splitContent[i] += "[NEXT]";
            }
            ret = splitContent.ToList();
            // 检查内容合法性并企图将不合法的内容进行合并
            while (true)
            {
                bool isMerged = false;
                for (int i = 0; i < ret.Count; ++i)
                {
                    if (IsOnlyControl(ret[i]) && i > 0)
                    {
                        ret[i - 1] += ret[i];
                        ret.RemoveAt(i);
                        isMerged = true;
                        break;
                    }
                }
                if (!isMerged)
                    break;
            }
            return ret;
        }

        /// <summary>
        /// 给定的字符串是否完全由控制符组成？
        /// </summary>
        /// <param name="content">给定字符串</param>
        /// <returns>是否全都是控制符</returns>
        private static bool IsOnlyControl(string content)
        {
            bool isInControlMode = false;
            char[] chars = content.ToArray();
            for (int i = 0; i < chars.Length; ++i)
            {
                if (chars[i] == '[' || chars[i] == '<' || chars[i] == '{')
                {
                    isInControlMode = true;
                }
                else if(chars[i] == ']' || chars[i] == '>' || chars[i] == '}')
                {
                    isInControlMode = false;
                }
                else {
                    if (!isInControlMode)
                        return false;
                }
            }
            return true;
        }
    }
}
