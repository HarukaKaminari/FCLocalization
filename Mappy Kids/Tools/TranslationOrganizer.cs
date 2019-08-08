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
        // 不重复字符表
        public static List<List<char>> s_UniqueChars;
        // 文本bank表
        public static List<List<char>> s_TextBanks;
        // 每个bank（严格来说应该是每两个bank）最多允许的字符个数
        private static readonly int MAX_UNIQUE_CHAR_COUNT = 30;

        public static void Start(string TranslationFileName)
        {
            FileStream fs = new FileStream(TranslationFileName, FileMode.Open);
            if(fs != null)
            {
                StreamReader sr = new StreamReader(fs);
                if(sr != null)
                {
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
            // 获得所有分割好的文本
            List<string> tmpList = new List<string>();
            foreach(string str in s_TableTranslation)
            {
                tmpList.AddRange(SplitContent(str));
            }
            // 获得不重复字符
            s_UniqueChars = new List<List<char>>();
            foreach(string str in tmpList)
            {
                List<char> uniqueChars = GetUniqueChars(str);
                if(uniqueChars.Count > 0)
                    s_UniqueChars.Add(uniqueChars);
            }
            // 不重复字符列表进行合并，从而获得bank字符表
            MergeUniqueChars();
            // 重建翻译文本列表！
            RebuildTranslationTable();
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
            string[] splitContent = content.Split("[NEXT]".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            for(int i = 0; i < splitContent.Length - 1; ++i)
            {
                splitContent[i] += "[NEXT]";
            }
            ret = splitContent.ToList();
            // 检查内容合法性并企图将不合法的内容进行合并
            while (true)
            {
                bool isMerged = false;
                for(int i = 0; i < ret.Count; ++i)
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
            char[] chars = content.ToArray();
            for(int i = 0; i < chars.Length; ++i)
            {
                if(chars[i] == ']')
                {
                    if (chars[i + 1] != '[')
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 获得指定文本中所有不重复字符
        /// </summary>
        /// <param name="content">文本</param>
        /// <returns>所有不重复字符列表</returns>
        private static List<char> GetUniqueChars(string content)
        {
            // 搜索最后一个逗号，去掉它和它之前的内容
            int comma = content.LastIndexOf(',');
            if (comma >= 0)
                content = content.Substring(comma + 1);
            // 循环检测每一个字符
            List<char> ret = new List<char>();
            bool isIgnore = false;
            char[] chars = content.ToArray();
            foreach(char c in chars)
            {
                switch (c)
                {
                    case '[':
                        {
                            isIgnore = true;
                            break;
                        }
                    case ']':
                        {
                            isIgnore = false;
                            break;
                        }
                    case '<':
                        {
                            isIgnore = true;
                            break;
                        }
                    case '>':
                        {
                            isIgnore = false;
                            break;
                        }
                    default:
                        {
                            if (!isIgnore)
                            {
                                if (!ret.Contains(c))
                                    ret.Add(c);
                            }
                            break;
                        }
                }
            }
            return ret;
        }

        /// <summary>
        /// 合并不重复列表
        /// </summary>
        private static void MergeUniqueChars()
        {
            // 首先对列表进行排序
            s_UniqueChars.Sort((a, b) => {
                if (a.Count > b.Count)
                    return 1;
                else if (a.Count < b.Count)
                    return -1;
                else
                    return 0;
            });
            // 尝试合并
            List<List<char>> tmpUniqueChars = new List<List<char>>();
            int curIndex = 0;
            while (curIndex <= s_UniqueChars.Count)
            {
                // 从不重复字符表中取出第0号元素，添加到临时列表中
                tmpUniqueChars.Add(s_UniqueChars[0]);
                // 不重复字符表删除第0号元素
                s_UniqueChars.RemoveAt(0);
                // 尝试从剩下的不重复字符表中取元素，并添加到临时列表的当前列中
                while (true)
                {
                    bool isManipulated = false;
                    for (int i = 0; i < s_UniqueChars.Count; ++i)
                    {
                        List<char> mergedItem = TryMerging(tmpUniqueChars[curIndex], s_UniqueChars[i]);
                        if(mergedItem != null)
                        {
                            tmpUniqueChars[curIndex] = mergedItem;
                            s_UniqueChars.RemoveAt(i);
                            isManipulated = true;
                            break;
                        }
                    }
                    if (!isManipulated)
                    {
                        curIndex++;
                        break;
                    }
                }
            }
            // 合并完成！
            s_TextBanks = tmpUniqueChars;
        }

        /// <summary>
        /// 尝试合并两个不重复列表项。如果合并成功，则返回合并好的列表项；如果合并失败，则返回null
        /// </summary>
        /// <param name="original">原始列表项</param>
        /// <param name="newItem">企图合并进来的列表项</param>
        /// <returns>合并好的列表项</returns>
        private static List<char> TryMerging(List<char> original, List<char> newItem)
        {
            foreach(char c in newItem)
            {
                if (!original.Contains(c))
                {
                    original.Add(c);
                }
            }
            if (original.Count > MAX_UNIQUE_CHAR_COUNT)
                return null;
            else
                return original;
        }


        private static void RebuildTranslationTable()
        {
            
        }
    }
}
