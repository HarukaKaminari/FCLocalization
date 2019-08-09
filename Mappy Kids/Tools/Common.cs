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
        /// 读ROM文件，获得CHR数据
        /// </summary>
        /// <param name="fileName">ROM文件名</param>
        /// <returns>CHR数据</returns>
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
                    br = null;
                }
                fs.Close();
                fs.Dispose();
                fs = null;
            }
            return chrData;
        }

        /// <summary>
        /// 给ROM打补丁。要配合GetPRGData和GetCHRData这两个函数使用
        /// </summary>
        /// <param name="fileName">ROM文件名</param>
        /// <param name="prgData">PRG补丁数据。如果为null的话则忽略给PRG打补丁</param>
        /// <param name="chrData">CHR补丁数据。如果为null的话则忽略给CHR打补丁</param>
        public static void PatchROM(string fileName, Byte[] prgData, Byte[] chrData)
        {
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
                    bw = null;
                }
                fs.Close();
                fs.Dispose();
                fs = null;
            }
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
