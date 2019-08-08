using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TranslationOrganizer
{
    class Common
    {

        /// <summary>
        /// 读ROM文件，获得PRG数据
        /// </summary>
        /// <param name="fileName">ROM文件名</param>
        /// <returns>PRG数据</returns>
        public static Byte[] GetPRGData(string fileName)
        {
            Byte[] prgData = null;
            FileStream fs = new FileStream(fileName, FileMode.Open);
            if (fs != null)
            {
                BinaryReader br = new BinaryReader(fs);
                if (br != null)
                {
                    Byte[] header = br.ReadBytes(16);
                    int prgSize = header[4] * 16 * 1024;
                    prgData = br.ReadBytes(prgSize);

                    br.Close();
                    br.Dispose();
                    br = null;
                }
                fs.Close();
                fs.Dispose();
                fs = null;
            }
            return prgData;
        }

        /// <summary>
        /// 读TBL文件，获得码表
        /// </summary>
        /// <param name="fileName">码表文件名</param>
        /// <returns>码表字典</returns>
        public static Dictionary<Byte, string> GetTBL(string fileName)
        {
            Dictionary<Byte, string> ret = null;
            FileStream fs = new FileStream(fileName, FileMode.Open);
            if (fs != null)
            {
                StreamReader sr = new StreamReader(fs);
                if (sr != null)
                {
                    ret = new Dictionary<byte, string>();
                    while (!sr.EndOfStream)
                    {
                        string str = sr.ReadLine();
                        if (str.Trim().Equals(string.Empty))
                            continue;
                        string[] c = str.Split('=');
                        Byte key = Byte.Parse(c[0], System.Globalization.NumberStyles.AllowHexSpecifier);
                        string value = c[1];
                        // 如果没有键值，给它做个标记
                        if (value.Equals(string.Empty))
                            value = string.Format("<{0:X}>", key);
                        ret.Add(key, value);
                    }

                    sr.Close();
                    sr.Dispose();
                    sr = null;
                }
                fs.Close();
                fs.Dispose();
                fs = null;
            }
            return ret;
        }
        
    }
}
