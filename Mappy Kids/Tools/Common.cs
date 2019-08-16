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
        

        public class RecordInfo
        {
            public int offset;  // 偏移量。长度3字节。BigEndian
            public bool isRLE;  // 是否是RLE记录
            public short len;   // 记录长度。长度2字节。BigEndian
            public Byte[] data; // 记录数据，或RLE字节数据
        }
        public class IPSInfo
        {
            public char[] patch;    // 5字节ASCII字符“PATCH”
            public List<RecordInfo> records;  // 记录列表
            public char[] eof;      // 3字节ASCII字符“EOF”
            //public int truncateOffset;  // 截断的偏移量。长度3字节。BigEndian。目前暂不支持truncate
        }

        /// <summary>
        /// 给ROM打补丁，并生成IPS文件
        /// </summary>
        /// <param name="fileName">ROM文件名</param>
        /// <param name="prgData">PRG补丁数据。如果为null的话则PRG无需打补丁</param>
        /// <param name="chrData">CHR补丁数据。如果为null的话则CHR无需打补丁</param>
        /// <param name="IPSFileName">IPS补丁文件名</param>
        public static void CreateIPS(string fileName, Byte[] prgData, Byte[] chrData, string IPSFileName)
        {
            // 获得原始ROM数据
            Byte[] romData = null;
            FileStream fs = new FileStream(fileName, FileMode.Open);
            if (fs != null)
            {
                BinaryReader br = new BinaryReader(fs);
                if (br != null)
                {
                    romData = br.ReadBytes((int)fs.Length);
                    br.Close();
                    br.Dispose();
                    br = null;
                }
                fs.Close();
                fs.Dispose();
                fs = null;
            }
            if(prgData == null)
            {
                prgData = GetPRGData(fileName);
            }
            if(chrData == null)
            {
                chrData = GetCHRData(fileName);
            }
            // 获得新ROM数据
            Byte[] newROMData = new Byte[romData.Length];
            for(int i = 0; i < 16; ++i)
            {
                newROMData[0 + i] = romData[i];
            }
            for(int i = 0; i < prgData.Length; ++i)
            {
                newROMData[16 + i] = prgData[i];
            }
            for(int i = 0; i < chrData.Length; ++i)
            {
                newROMData[16 + prgData.Length + i] = chrData[i];
            }
            // 差异对比
            IPSInfo info = new IPSInfo();
            info.patch = new char[] { 'P', 'A', 'T', 'C', 'H' };
            info.records = new List<RecordInfo>();
            int offset = 0;
            while (offset < Math.Min(romData.Length, newROMData.Length) )
            {
                if(romData[offset] != newROMData[offset])
                {
                    RecordInfo ri = new RecordInfo();
                    short RLERecordLen = getRLERecord(newROMData, offset);
                    if(RLERecordLen > 1)
                    {
                        ri.isRLE = true;
                        ri.len = RLERecordLen;
                        ri.data = new Byte[] { newROMData[offset] };
                        offset += RLERecordLen;
                    }
                    else
                    {
                        ri.isRLE = false;
                        ri.data = getRecord(romData, newROMData, offset);
                        ri.len = (short)ri.data.Length;
                        offset += ri.len;
                    }
                    info.records.Add(ri);
                }
            }
            if(newROMData.Length > romData.Length)
            {
                RecordInfo ri = new RecordInfo();
                ri.isRLE = false;
                ri.len = (short)(newROMData.Length - romData.Length);
                ri.data = new Byte[ri.len];
                for(int i = 0; i < ri.data.Length; ++i)
                {
                    ri.data[i] = newROMData[romData.Length + i];
                }
                info.records.Add(ri);
            }
            else if(newROMData.Length < romData.Length)
            {
                Console.WriteLine("暂不支持truncate！");
            }
            info.eof = new char[] { 'E', 'O', 'F' };
            // 写IPS文件
            fs = new FileStream(IPSFileName, FileMode.Create);
            if(fs != null)
            {
                BinaryWriter bw = new BinaryWriter(fs);
                if(bw != null)
                {
                    bw.Write(info.patch);
                    foreach(RecordInfo ri in info.records)
                    {
                        bw.Write((Byte)((ri.offset >> 16) & 0xFF));
                        bw.Write((Byte)((ri.offset >> 8) & 0xFF));
                        bw.Write((Byte)((ri.offset >> 0) & 0xFF));
                        if (ri.isRLE)
                        {
                            bw.Write((Byte)0);
                            bw.Write((Byte)0);
                        }
                        bw.Write((Byte)((ri.len >> 8) & 0xFF));
                        bw.Write((Byte)((ri.len >> 0) & 0xFF));
                        foreach(Byte b in ri.data)
                        {
                            bw.Write(b);
                        }
                    }
                    bw.Write(info.eof);

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


        private static short getRLERecord(Byte[] newROMData, int offset)
        {
            short len = 1;
            int value = newROMData[offset];
            offset++;
            while (newROMData[offset] == value)
            {
                len++;
                offset++;
            }
            return len;
        }

        private static Byte[] getRecord(Byte[] romData, Byte[] newROMData, int offset)
        {
            List<Byte> ret = new List<Byte>();
            while (true)
            {
                offset++;
                if(offset >= romData.Length || offset >= newROMData.Length)
                {
                    break;
                }
                Byte value0 = romData[offset];
                Byte value1 = newROMData[offset];
                if (value0 != value1)
                {
                    ret.Add(value1);
                }
                else
                {
                    break;
                }
            }
            return ret.ToArray();
        }

        /// <summary>
        /// 载入IPS补丁，获得补丁信息
        /// </summary>
        /// <param name="IPSFileName">IPS补丁文件名</param>
        /// <returns>补丁信息</returns>
        public static IPSInfo LoadIPS(string IPSFileName)
        {
            IPSInfo info = null;
            FileStream fs = new FileStream(IPSFileName, FileMode.Open);
            if(fs != null)
            {
                BinaryReader br = new BinaryReader(fs);
                if(br != null)
                {
                    info = new IPSInfo();
                    // 先读5字节的patch
                    Byte[] patch = br.ReadBytes(5);
                    if(patch[0] != 'P' || patch[1] != 'A' || patch[1] != 'T' || patch[1] != 'C' || patch[1] != 'H')
                    {
                        info = null;
                    }
                    if(info != null)
                    {
                        info.records = new List<RecordInfo>();
                        // 循环读取记录，直到读到eof为止
                        while (true)
                        {
                            RecordInfo ri = new RecordInfo();
                            // 先读3字节
                            Byte[] data = br.ReadBytes(3);
                            // 检测是否读到了eof，是的话就跳出循环
                            if (data[0] == 'E' && data[1] == 'O' && data[2] == 'F')
                            {
                                break;
                            }
                            // 3字节的offset
                            ri.offset = ((int)data[0] << 16) | ((int)data[1] << 8) | ((int)data[2] << 0);
                            // 再读2字节
                            data = br.ReadBytes(2);
                            // 检测是否是00 00。如果是的话则证明是RLE记录
                            if (data[0] == 0 && data[1] == 0)
                            {
                                ri.isRLE = true;
                                // 再读2字节作为len
                                data = br.ReadBytes(2);
                                ri.len = (short)(((short)data[0] << 8) | ((short)data[1] << 0));
                                // 再读1字节作为RLE数据
                                ri.data = br.ReadBytes(1);
                            }
                            // 不是RLE记录
                            else
                            {
                                ri.isRLE = false;
                                // 2字节的len
                                ri.len = (short)(((short)data[0] << 8) | ((short)data[1] << 0));
                                // 再读len字节作为数据
                                ri.data = br.ReadBytes(ri.len);
                            }
                            info.records.Add(ri);
                        }
                        //// 如果此时已经读完了IPS文件，则无truncate，否则再读3字节作为truncateOffset
                        //if (br.BaseStream.Position >= br.BaseStream.Length)
                        //{
                        //    info.truncateOffset = 0;
                        //}
                        //else
                        //{
                        //    Byte[] data = br.ReadBytes(3);
                        //    info.truncateOffset = ((int)data[0] << 16) | ((int)data[1] << 8) | ((int)data[2] << 0);
                        //}
                    }
                    br.Close();
                    br.Dispose();
                    br = null;
                }
                fs.Close();
                fs.Dispose();
                fs = null;
            }
            return info;
        }

        /// <summary>
        /// 给ROM打IPS补丁
        /// </summary>
        /// <param name="IPSFileName">IPS补丁文件名</param>
        /// <param name="ROMFileName">ROM文件名</param>
        public static void ApplyPatchToROM(string IPSFileName, string ROMFileName)
        {
            IPSInfo info = LoadIPS(IPSFileName);
            if(info == null)
            {
                return;
            }
            FileStream fs = new FileStream(ROMFileName, FileMode.Open);
            if(fs != null)
            {
                BinaryWriter bw = new BinaryWriter(fs);
                if(bw != null)
                {
                    foreach (RecordInfo ri in info.records)
                    {
                        bw.Seek(ri.offset, SeekOrigin.Begin);
                        if (ri.isRLE)
                        {
                            for(int i = 0; i < ri.len; ++i)
                            {
                                bw.Write(ri.data);
                            }
                        }
                        else
                        {
                            bw.Write(ri.data);
                        }
                    }
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
    }
}
