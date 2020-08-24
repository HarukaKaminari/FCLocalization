using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using NPOI.HSSF.UserModel;
using System.Text.RegularExpressions;
using CoreV3;
using System.Drawing;
using System.Drawing.Text;

namespace Tools
{
    class Common
    {

        /// <summary>
        /// bank号+地址转ROM真实地址
        /// </summary>
        /// <param name="prgBank"></param>
        /// <param name="addr"></param>
        /// <returns></returns>
        public static int GetPRGROMAddress(int prgBank, int addr)
        {
            return prgBank * 8192 + (addr & 0x1FFF);
        }
        public static int GetCHRROMAddress(int chrBank, int addr)
        {
            return chrBank * 1024 + (addr & 0x3FF);
        }

        /// <summary>
        /// 读ROM文件，获得文件头
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static Byte[] GetHeaderData(string fileName)
        {
            Byte[] header = null;
            FileStream fs = new FileStream(fileName, FileMode.Open);
            if (fs != null)
            {
                BinaryReader br = new BinaryReader(fs);
                if (br != null)
                {
                    header = br.ReadBytes(16);

                    br.Close();
                    br.Dispose();
                }
                fs.Close();
                fs.Dispose();
            }
            return header;
        }

        /// <summary>
        /// 读ROM文件，获得PRG数据
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static Byte[] GetPRGData(string fileName)
        {
            Byte[] prgData = null;
            FileStream fs = new FileStream(fileName, FileMode.Open);
            if(fs != null)
            {
                BinaryReader br = new BinaryReader(fs);
                if(br != null)
                {
                    Byte[] header = br.ReadBytes(16);
                    int prgSize = header[4] * 16 * 1024;
                    prgData = br.ReadBytes(prgSize);

                    br.Close();
                    br.Dispose();
                }
                fs.Close();
                fs.Dispose();
            }
            return prgData;
        }

        /// <summary>
        /// 读ROM文件，获得CHR数据
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static Byte[] GetCHRData(string fileName)
        {
            Byte[] chrData = null;
            FileStream fs = new FileStream(fileName, FileMode.Open);
            if (fs != null)
            {
                BinaryReader br = new BinaryReader(fs);
                if (br != null)
                {
                    Byte[] header = br.ReadBytes(16);
                    int prgSize = header[4] * 16 * 1024;
                    int chrSize = header[5] * 8 * 1024;
                    br.ReadBytes(prgSize);
                    chrData = br.ReadBytes(chrSize);

                    br.Close();
                    br.Dispose();
                }
                fs.Close();
                fs.Dispose();
            }
            return chrData;
        }

        /// <summary>
        /// 给ROM打补丁。如果prgData或chrData为null则不给相应的数据打补丁
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="prgData"></param>
        /// <param name="chrData"></param>
        public static void PatchROM(string fileName, Byte[] prgData, Byte[] chrData)
        {
            if (prgData == null)
                prgData = GetPRGData(fileName);
            if (chrData == null)
                chrData = GetCHRData(fileName);

            FileStream fs = new FileStream(fileName, FileMode.Open);
            if(fs != null)
            {
                BinaryWriter bw = new BinaryWriter(fs);
                if(bw != null)
                {
                    bw.Seek(16, SeekOrigin.Begin);
                    bw.Write(prgData);
                    bw.Write(chrData);
                    bw.Flush();
                    bw.Close();
                    bw.Dispose();
                }
                fs.Close();
                fs.Dispose();
            }
        }

        /// <summary>
        /// 将CHR ROM从0K改为指定大小
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="size"></param>
        public static void ExpandCHR(string fileName, int size)
        {
            Byte[] header = GetHeaderData(fileName);
            header[5] = (Byte)(size >> 13);
            Byte[] prgData = GetPRGData(fileName);
            Byte[] chrData = new Byte[size];
            for(int i = 0; i < chrData.Length; ++i)
            {
                chrData[i] = 0;
            }

            FileStream fs = new FileStream(fileName, FileMode.Open);
            if (fs != null)
            {
                BinaryWriter bw = new BinaryWriter(fs);
                if (bw != null)
                {
                    bw.Write(header);
                    bw.Write(prgData);
                    bw.Write(chrData);
                    bw.Flush();
                    bw.Close();
                    bw.Dispose();
                }
                fs.Close();
                fs.Dispose();
            }
        }

        public static void ChangeMapperTo19(string fileName)
        {
            Byte[] header = GetHeaderData(fileName);
            header[6] = 0x33;
            header[7] = 0x10;
            Byte[] prgData = GetPRGData(fileName);
            Byte[] chrData = GetCHRData(fileName);
            for (int i = 0; i < chrData.Length; ++i)
            {
                chrData[i] = 0;
            }

            FileStream fs = new FileStream(fileName, FileMode.Open);
            if (fs != null)
            {
                BinaryWriter bw = new BinaryWriter(fs);
                if (bw != null)
                {
                    bw.Write(header);
                    bw.Write(prgData);
                    bw.Write(chrData);
                    bw.Flush();
                    bw.Close();
                    bw.Dispose();
                }
                fs.Close();
                fs.Dispose();
            }
        }

        /// <summary>
        /// 可乐妹压缩算法
        /// </summary>
        /// <param name="data"></param>
        /// <param name="addr"></param>
        /// <returns></returns>
        public static Byte[] KonamiEncode(List<Byte> data, int addr)
        {
            List<Byte> ret = new List<Byte>();
            // 先写地址
            ret.Add((Byte)((addr >> 0) & 0xFF));
            ret.Add((Byte)((addr >> 8) & 0xFF));
            // 压缩
            ret.AddRange(encode(data));
            // 写结束
            ret.Add(0xFF);
            return ret.ToArray();
        }

        private static List<Byte> encode(List<Byte> input)
        {
            List<Byte> ret = new List<Byte>();

            List<Byte> tmpBlock = new List<Byte>();
            Byte curValue = 0;
            int maxLen = 0x7E;
            bool isIdenticalMode = false;
            for(int i = 0; i < input.Count; ++i)
            {
                curValue = input[i];
                if(tmpBlock.Count == 0)
                {
                    tmpBlock.Add(input[0]);
                    tmpBlock.Add(input[1]);
                    isIdenticalMode = tmpBlock[0] == tmpBlock[1];
                    i++;
                }
                else
                {
                    if (isIdenticalMode)
                    {
                        if(curValue == tmpBlock[tmpBlock.Count - 1])
                        {
                            if(tmpBlock.Count < maxLen)
                            {
                                tmpBlock.Add(curValue);
                            }
                            else
                            {
                                ret.Add((Byte)tmpBlock.Count);
                                ret.Add(tmpBlock[tmpBlock.Count - 1]);
                                tmpBlock.Clear();
                                tmpBlock.Add(curValue);
                            }
                        }
                        else
                        {
                            ret.Add((Byte)tmpBlock.Count);
                            ret.Add(tmpBlock[tmpBlock.Count - 1]);
                            tmpBlock.Clear();
                            tmpBlock.Add(curValue);
                            isIdenticalMode = false;
                        }
                    }
                    else
                    {
                        if(curValue != tmpBlock[tmpBlock.Count - 1])
                        {
                            if (tmpBlock.Count < maxLen)
                            {
                                tmpBlock.Add(curValue);
                            }
                            else
                            {
                                ret.Add((Byte)(tmpBlock.Count | 0x80));
                                ret.AddRange(tmpBlock);
                                tmpBlock.Clear();
                                tmpBlock.Add(curValue);
                            }
                        }
                        else
                        {
                            ret.Add((Byte)(tmpBlock.Count | 0x80));
                            ret.AddRange(tmpBlock);
                            tmpBlock.Clear();
                            tmpBlock.Add(curValue);
                            isIdenticalMode = true;
                        }
                    }
                }
            }
            return ret;
        }

        /// <summary>
        /// 加载自定义ttf字体文件
        /// </summary>
        /// <param name="fontFileName"></param>
        /// <param name="fontSize"></param>
        /// <returns></returns>
        public static Font LoadCustomTTFFont(string fontFileName, float fontSize)
        {
            PrivateFontCollection pfc = new PrivateFontCollection();
            pfc.AddFontFile(fontFileName);
            Font font = new Font(pfc.Families[0], fontSize);
            return font;
        }

        public static HSSFSheet GetSheet(FileStream xlsFileStream, string sheetName)
        {
            HSSFWorkbook workbook = new HSSFWorkbook(xlsFileStream);
            return workbook.GetSheet(sheetName);
        }

        public static string GetCellValueFromSheet(HSSFSheet sheet, int row, int col)
        {
            HSSFCell cell = sheet.GetRow(row).GetCell(col);
            return cell.StringCellValue;
        }

        public static void GetBankAddressSizeFromSheet(HSSFSheet sheet, out int bank, out int addr, out int size)
        {
            string value = GetCellValueFromSheet(sheet, 0, 0);
            if (true)
            {
                Regex reg = new Regex("Bank: \\$(\\w{2}),", RegexOptions.IgnoreCase);
                Match match = reg.Match(value);
                string bankNo = match.Groups[1].Value;
                if (bankNo.Trim().Equals(string.Empty))
                    bank = -1;
                else
                    bank = int.Parse(bankNo, System.Globalization.NumberStyles.AllowHexSpecifier);
            }
            if (true)
            {
                Regex reg = new Regex("Address: \\$(\\w{4})", RegexOptions.IgnoreCase);
                Match match = reg.Match(value);
                string address = match.Groups[1].Value;
                if (address.Trim().Equals(string.Empty))
                    addr = -1;
                else
                    addr = int.Parse(address, System.Globalization.NumberStyles.AllowHexSpecifier);
            }
            if (true)
            {
                Regex reg = new Regex("Size: (\\d+) Byte", RegexOptions.IgnoreCase);
                Match match = reg.Match(value);
                string codesize = match.Groups[1].Value;
                if (codesize.Trim().Equals(string.Empty))
                    size = -1;
                else
                    size = int.Parse(codesize);
            }
        }

        public static List<string> GetCodeBlockFromSheet(HSSFSheet sheet)
        {
            List<string> codeBlock = new List<string>();
            int curRow = 1;
            string[] contents = new string[3];
            while (true)
            {
                HSSFRow row = sheet.GetRow(curRow);
                if (row != null)
                {
                    for (int i = 0; i < contents.Length; ++i)
                    {
                        contents[i] = string.Empty;
                        HSSFCell cell = row.GetCell(i);
                        if (cell != null)
                        {
                            contents[i] = cell.StringCellValue;
                        }
                    }
                    if (contents[0].Equals("EOF"))
                        break;

                    string line = string.Empty;
                    for (int i = 0; i < contents.Length; ++i)
                    {
                        if (!contents[i].Trim().Equals(string.Empty))
                            line += contents[i];
                    }
                    codeBlock.Add(line);
                }
                curRow++;
            }
            return codeBlock;
        }

        public static string GetCodeStringFromSheet(HSSFSheet sheet)
        {
            List<string> codeBlock = GetCodeBlockFromSheet(sheet);
            StringBuilder ret = new StringBuilder();
            foreach(string str in codeBlock)
            {
                ret.Append(str);
                ret.Append("\n");
            }
            return ret.ToString();
        }

        public static string GetLegitCodeStringFromSheet(HSSFSheet sheet)
        {
            List<string> codeBlock = GetCodeBlockFromSheet(sheet);
            StringBuilder ret = new StringBuilder();
            foreach (string str in codeBlock)
            {
                if (str.EndsWith(":"))
                {
                    ret.Append(str.Replace(':', ' ').Trim());
                }
                else if (str.Contains(".byte"))
                {
                    ret.Append(str.Replace(".byte", ".db"));
                }
                else
                {
                    ret.Append(str);
                }
                ret.Append("\n");
            }
            return ret.ToString();
        }

        private static string s_strIncludeStr = string.Empty;
        public static void LoadIncludeCodeString(HSSFSheet sheet)
        {
            StringBuilder sb = new StringBuilder();
            int curRow = 0;
            while (true)
            {
                HSSFRow row = sheet.GetRow(curRow);
                if (row == null)
                    break;
                string cellName = row.GetCell(0).StringCellValue;
                string cellAddr = row.GetCell(1).StringCellValue;
                sb.Append(cellName);
                sb.Append("=");
                sb.Append(cellAddr);
                sb.Append("\n");
                curRow++;
            }
            s_strIncludeStr = sb.ToString();
        }

        public static string GetIncludeCodeString()
        {
            return s_strIncludeStr;
        }

        public static Byte[] CompileSheet(HSSFSheet sheet, out int bank, out int addr, out int size, int presetAddr = -1)
        {
            GetBankAddressSizeFromSheet(sheet, out bank, out addr, out size);
            // 如果excel表中注明了地址，那么直接忽略最后一个参数
            if(addr != -1)
            {
                presetAddr = addr;
            }
            string codeString = GetLegitCodeStringFromSheet(sheet);
            // 将首地址追加到代码开头从而能够正确编译
            codeString = string.Format("{0}\n\n\n.org ${1:X4}\n\n{2}", s_strIncludeStr, presetAddr & 0xFFFF, codeString);
            
            Byte[] code = null;
            try
            {
                code = Compile.CompileAllTextToByte(codeString, string.Empty);
            }catch(Exception e)
            {
                if (e != null) ;
            }
            return code;
        }
    }
}
