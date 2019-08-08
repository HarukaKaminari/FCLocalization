using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TranslationOrganizer
{
    class TableOrganizer
    {
        // 指针表所在的bank
        public static readonly int PRGBANK_8000_9FFF = 0x08;
        public static readonly int PRGBANK_A000_BFFF = 0x09;
        // 指针表首地址
        public static readonly int[] POINTER_TABLE_LO = { 0xB3D6, 0xB9E3, 0x8C4A };
        public static readonly int[] POINTER_TABLE_HI = { 0xB435, 0xB9F8, 0x8C97 };
        public static readonly int[] POINTER_TABLE_LEN = { 95, 21, 77 };

        // 文本首地址表
        public static List<List<int>> s_TableAddr;
        // 文本表
        public static List<List<string>> s_TableString;

        public static void Start(string ROMFileName, string TBLFileName)
        {
            s_TableAddr = new List<List<int>>();
            s_TableString = new List<List<string>>();
            Byte[] prgData = Common.GetPRGData(ROMFileName);
            Dictionary<Byte, string> tblData = Common.GetTBL(TBLFileName);

            for(int i = 0; i < POINTER_TABLE_LO.Length; ++i)
            {
                List<int> table = new List<int>();
                List<string> table2 = new List<string>();
                for(int j = 0; j < POINTER_TABLE_LEN[i]; ++j)
                {
                    Byte lo = prgData[GetAbsoluteAddress(POINTER_TABLE_LO[i]) + j];
                    Byte hi = prgData[GetAbsoluteAddress(POINTER_TABLE_HI[i]) + j];
                    int addr = lo | (hi << 8);
                    table.Add(addr);
                    string str = data2string(prgData, tblData, addr);
                    table2.Add(str);
                }
                s_TableAddr.Add(table);
                s_TableString.Add(table2);
            }
        }
        
        public static void Save(string fileName)
        {
            FileStream fs = new FileStream(fileName, FileMode.Create);
            if(fs != null)
            {
                StreamWriter sw = new StreamWriter(fs);
                if(sw != null)
                {
                    for (int i = 0; i < POINTER_TABLE_LO.Length; ++i)
                    {
                        for(int j = 0; j < POINTER_TABLE_LEN[i]; ++j)
                        {
                            string content = string.Format("{0},{1:X4},{2:X4},{3:X4},{4}",
                                i,
                                POINTER_TABLE_LO[i] + j,
                                POINTER_TABLE_HI[i] + j,
                                s_TableAddr[i][j],
                                s_TableString[i][j]);
                            sw.WriteLine(content);
                        }
                    }
                    sw.Flush();
                    sw.Close();
                    sw.Dispose();
                    sw = null;
                }
                fs.Close();
                fs.Dispose();
                fs = null;
            }
        }

        /// <summary>
        /// 从指定地址计算绝对地址。注意，目前指定地址只允许在0x8000到0xBFFF之间
        /// </summary>
        /// <param name="addr">指定地址（0x8000~0xBFFF）</param>
        /// <returns>绝对地址</returns>
        private static int GetAbsoluteAddress(int addr)
        {
            if(addr >= 0x8000 && addr <= 0x9FFF)
            {
                return PRGBANK_8000_9FFF * 8 * 1024 + (addr & (8 * 1024 - 1));
            }
            else if(addr >= 0xA000 && addr <= 0xBFFF)
            {
                return PRGBANK_A000_BFFF * 8 * 1024 + (addr & (8 * 1024 - 1));
            }
            return -1;
        }

        /// <summary>
        /// 给定一个地址，获得这块内存中
        /// </summary>
        /// <param name="addr"></param>
        /// <returns></returns>
        private static string data2string(Byte[] prgData, Dictionary<Byte, string> tblData, int addr)
        {
            string ret = string.Empty;
            int absoluteAddr = GetAbsoluteAddress(addr);
            while (true)
            {
                Byte d = prgData[absoluteAddr];
                if (tblData.ContainsKey(d))
                {
                    ret += tblData[d];
                }
                else
                {
                    Console.WriteLine("Can not find key " + d + " in the TBL!");
                }
                if (d == 0xFF)
                    break;
                absoluteAddr++;
            }
            return ret;
        }
    }
}
