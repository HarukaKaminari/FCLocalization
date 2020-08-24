using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tools;
using System.IO;
using NPOI.HSSF.UserModel;
using CoreV3;

namespace Localizer
{
    public partial class formMain : Form
    {
        public static string s_strRootPath = "C:/Users/雷精灵/Desktop/FCLocalization/Roms/GreenBeret/";
        public static string s_strRomFileName = s_strRootPath + "GreenBeret (C) - Namcot163.nes";
        public static string s_strBackupRomFileName = s_strRootPath + "GreenBeret (J).nes";
        public static string s_strXLSFileName = s_strRootPath + "CodeBlock.xls";

        private static int curFixedAddr = 0xFFFA;
        private static int s_AddrCopyBankTo6000 = 0;
        private static int s_AddrSwitchCHR = 0;

        private static readonly int FillNTRAM_StartAddr_Table_Offset_ClearScreen = 0;
        private static readonly int FillNTRAM_StartAddr_Table_Offset_FillTitle = 1;
        private static readonly int FillNTRAM_StartAddr_Table_Offset_Unknown = 2;
        private static readonly int FillNTRAM_StartAddr_Table_Offset_FillIntro = 3;

        // intro界面汉字tile覆盖的位置
        private int[] posIntro = new int[] {
                  0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F,
            0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18, 0x19, 0x1A, 0x1B, 0x1C, 0x1D, 0x1E, 0x1F,
            0x20, 0x21, 0x22, 0x23, 0x24, 0x25, 0x26,
                        0x32,
                                                0x96,                                     0x9D, 0x9E, 0x9F,
                                                                  0xC9,
                                                                  0xD9,
                              0xE3,                   0xE7,
                                                                                                      0xFF,
        };
        // StageClear界面汉字tile覆盖的位置
        private int[] posStageClear = new int[]
        {
                                                                              0x0B, 0x0C, 0x0D, 0x0E, 0x0F,
            0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18, 0x19, 0x1A, 0x1B, 0x1C, 0x1D, 0x1E, 0x1F,
            0x20, 0x21, 0x22, 0x23, 0x24, 0x25, 0x26,
            0x80, 0x81, 0x82, 0x83, 0x84, 0x85, 0x86, 0x87, 0x88, 0x89, 0x8A, 0x8B, 0x8C, 0x8D, 0x8E, 0x8F,
            0x90, 0x91, 0x92, 0x93, 0x94, 0x95, 0x96, 0x97, 0x98, 0x99, 0x9A, 0x9B, 0x9C, 0x9D, 0x9E, 0x9F,
            0xA0, 0xA1, 0xA2, 0xA3, 0xA4, 0xA5, 0xA6, 0xA7, 0xA8, 0xA9, 0xAA, 0xAB, 0xAC, 0xAD,
        };
        // ending界面汉字tile覆盖的位置
        private int[] posEnding = new int[]
        {
                  0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F,
            0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18, 0x19, 0x1A, 0x1B, 0x1C, 0x1D, 0x1E, 0x1F,
            0x20, 0x21, 0x22, 0x23, 0x24, 0x25, 0x26,
                                                                                          0xBD, 0xBE, 0xBF,
            0xC0, 0xC1,
                                                                                                      0xFF,
        };
        // GameOver界面汉字tile覆盖的位置
        private Byte[] posGameOver = new Byte[]
        {
                  0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F,
            0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18, 0x19, 0x1A, 0x1B, 0x1C,
        };
        // Staff界面汉字tile覆盖的位置
        private Byte[] posStaff = new Byte[]
        {
                                                      0x27, 0x28, 0x29, 0x2A, 0x2B, 0x2C, 0x2D, 0x2E, 0x2F,
            0x30, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37, 0x38, 0x39, 0x3A, 0x3B, 0x3C, 0x3D, 0x3E, 0x3F,
            0x40, 0x41, 0x42, 0x43, 0x44, 0x45, 0x46, 0x47, 0x48, 0x49, 0x4A, 0x4B, 0x4C, 0x4D, 0x4E, 0x4F,
            0x50, 0x51, 0x52, 0x53, 0x54, 0x55, 0x56, 0x57, 0x58, 0x59, 0x5A, 0x5B, 0x5C, 0x5D, 0x5E, 0x5F,
            0x60, 0x61, 0x62, 0x63, 0x64, 0x65, 0x66, 0x67, 0x68, 0x69, 0x6A, 0x6B, 0x6C, 0x6D, 0x6E, 0x6F,
            0x70, 0x71, 0x72, 0x73, 0x74, 0x75, 0x76, 0x77, 0x78, 0x79, 0x7A, 0x7B, 0x7C, 0x7D, 0x7E, 0x7F,
            0x80, 0x81, 0x82, 0x83, 0x84, 0x85, 0x86, 0x87, 0x88, 0x89, 0x8A, 0x8B, 0x8C, 0x8D, 0x8E,
        };


        public formMain()
        {
            InitializeComponent();
        }

        private void btnCreateNewROM_Click(object sender, EventArgs e)
        {
            Byte[] data = null;
            FileStream fs = new FileStream(s_strBackupRomFileName, FileMode.Open);
            if(fs != null)
            {
                BinaryReader br = new BinaryReader(fs);
                if(br != null)
                {
                    data = br.ReadBytes((int)fs.Length);
                    br.Close();
                    br.Dispose();
                }
                fs.Close();
                fs.Dispose();
            }
            fs = new FileStream(s_strRomFileName, FileMode.OpenOrCreate);
            if(fs != null)
            {
                fs.SetLength(0);
                BinaryWriter bw = new BinaryWriter(fs);
                if(bw != null)
                {
                    bw.Write(data);
                    bw.Flush();
                    bw.Close();
                    bw.Dispose();
                }
                fs.Close();
                fs.Dispose();
            }
            MessageBox.Show("OK!");
        }

        private void btnCHRRAM2CHRROM_Click(object sender, EventArgs e)
        {
            Common.ExpandCHR(s_strRomFileName, 256 * 1024);
            Common.ChangeMapperTo19(s_strRomFileName);
            MessageBox.Show("OK!");
        }

        private void btnLoadInclude_Click(object sender, EventArgs e)
        {
            FileStream fs = new FileStream(s_strXLSFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            HSSFSheet sheet = Common.GetSheet(fs, "标签表");
            Common.LoadIncludeCodeString(sheet);
            fs.Close();
            MessageBox.Show("OK!");
        }

        private void btnInit_Click(object sender, EventArgs e)
        {
            // 读excel的Init页签
            FileStream fs = new FileStream(s_strXLSFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            HSSFSheet sheet = Common.GetSheet(fs, "Init");
            // 编译代码
            int bank, addr, size;
            Byte[] code = Common.CompileSheet(sheet, out bank, out addr, out size);
            fs.Close();
            // 初始化代码始终位于ROM的最后端，因此根据大小计算首地址
            addr = curFixedAddr - code.Length;
            // 写入ROM
            Byte[] prgData = Common.GetPRGData(s_strRomFileName);
            for(int i = 0; i < code.Length; ++i)
            {
                prgData[Common.GetPRGROMAddress(bank, addr) + i] = code[i];
            }
            // ROM的RST中断向量指向初始化代码的首地址
            prgData[prgData.Length - 3] = (Byte)(addr >> 8);
            prgData[prgData.Length - 4] = (Byte)(addr & 0xFF);

            // 重新设置固定地址
            curFixedAddr = addr;

            Common.PatchROM(s_strRomFileName, prgData, null);
            MessageBox.Show("OK!");

            Console.WriteLine("Init首地址={0:X4}，大小={1}字节", addr, code.Length);
        }
        
        private void btnCopyBankTo6000_Click(object sender, EventArgs e)
        {
            // 读excel
            FileStream fs = new FileStream(s_strXLSFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            HSSFSheet sheet = Common.GetSheet(fs, "CopyBankTo6000");
            // 编译代码
            int bank, addr, size;
            Byte[] code = Common.CompileSheet(sheet, out bank, out addr, out size);
            fs.Close();
            // CopyBankTo6000代码始终位于Init的前面，因此根据大小计算首地址
            addr = curFixedAddr - code.Length;
            s_AddrCopyBankTo6000 = addr;
            // 写入ROM
            Byte[] prgData = Common.GetPRGData(s_strRomFileName);
            for (int i = 0; i < code.Length; ++i)
            {
                prgData[Common.GetPRGROMAddress(bank, addr) + i] = code[i];
            }
            // 重新设置固定地址
            curFixedAddr = addr;

            Common.PatchROM(s_strRomFileName, prgData, null);
            MessageBox.Show("OK!");

            Console.WriteLine("CopyBankTo6000首地址={0:X4}，大小={1}字节", addr, code.Length);
        }

        private void btnHijackInterrupt_Click(object sender, EventArgs e)
        {
            Byte[] prgData = Common.GetPRGData(s_strRomFileName);
            if (true)
            {
                // 读excel
                FileStream fs = new FileStream(s_strXLSFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                HSSFSheet sheet = Common.GetSheet(fs, "HijackNMI");
                // 编译代码
                int bank, addr, size;
                Byte[] code = Common.CompileSheet(sheet, out bank, out addr, out size);
                fs.Close();
                // HijackNMI始终位于CopyBankTo6000的前面，因此根据大小计算首地址
                addr = curFixedAddr - code.Length;
                // 写入ROM
                for (int i = 0; i < code.Length; ++i)
                {
                    prgData[Common.GetPRGROMAddress(bank, addr) + i] = code[i];
                }
                // ROM的NMI中断向量指向初始化代码的首地址
                prgData[prgData.Length - 5] = (Byte)(addr >> 8);
                prgData[prgData.Length - 6] = (Byte)(addr & 0xFF);
                // 重新设置固定地址
                curFixedAddr = addr;

                Console.WriteLine("HijackNMI首地址={0:X4}，大小={1}字节", addr, code.Length);
            }
            if (true)
            {
                // 读excel
                FileStream fs = new FileStream(s_strXLSFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                HSSFSheet sheet = Common.GetSheet(fs, "HijackIRQ");
                // 编译代码
                int bank, addr, size;
                Byte[] code = Common.CompileSheet(sheet, out bank, out addr, out size);
                fs.Close();
                // HijackIRQ始终位于HijackNMI的前面，因此根据大小计算首地址
                addr = curFixedAddr - code.Length;
                // 写入ROM
                for (int i = 0; i < code.Length; ++i)
                {
                    prgData[Common.GetPRGROMAddress(bank, addr) + i] = code[i];
                }
                // ROM的IRQ中断向量指向初始化代码的首地址
                prgData[prgData.Length - 1] = (Byte)(addr >> 8);
                prgData[prgData.Length - 2] = (Byte)(addr & 0xFF);
                // 重新设置固定地址
                curFixedAddr = addr;

                Console.WriteLine("HijackIRQ首地址={0:X4}，大小={1}字节", addr, code.Length);
            }

            Common.PatchROM(s_strRomFileName, prgData, null);
            MessageBox.Show("OK!");
        }

        private void btnPatchWriteE000_Click(object sender, EventArgs e)
        {
            KeyValuePair<int, int>[] bankAndAddr = new KeyValuePair<int, int>[]
            {
                new KeyValuePair<int, int>(0x1F, 0xE1FC),
                new KeyValuePair<int, int>(0x1F, 0xE202),
            };

            Byte[] prgData = Common.GetPRGData(s_strRomFileName);
            foreach (var kv in bankAndAddr)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(Common.GetIncludeCodeString());
                sb.Append("\n\n\n");
                sb.Append(string.Format("jsr ${0:X4}", s_AddrCopyBankTo6000));
                Byte[] code = Compile.CompileAllTextToByte(sb.ToString(), string.Empty);
                for(int i = 0; i < code.Length; ++i)
                {
                    prgData[Common.GetPRGROMAddress(kv.Key, kv.Value) + i] = code[i];
                }
            }
            Common.PatchROM(s_strRomFileName, prgData, null);
            MessageBox.Show("OK!");
        }
        
        private void btnImportCHR_Click(object sender, EventArgs e)
        {
            Byte[] chrData = Common.GetCHRData(s_strRomFileName);
            string[] imgFileNames = new string[] {
                "title.png",
                "intro.png",
                "stage1.png",
                "stage2.png",
                "stage3.png",
                "stage4.png",
                "stage5.png",
                "stage6.png",
                "stageclear.png",
                "ending.png",
                "gameover.png",
                "staff.png",
            };
            int idx = 0;
            foreach(string imgFileName in imgFileNames)
            {
                Byte[] data = TileCreator.Bitmap2Data(new Bitmap(s_strRootPath + imgFileName));
                for (int i = 0; i < data.Length; ++i)
                {
                    chrData[8192 * idx + i] = data[i];
                }
                idx++;
            }
            Common.PatchROM(s_strRomFileName, null, chrData);
            MessageBox.Show("OK!");
        }

        private void btnSwitchCHR_Click(object sender, EventArgs e)
        {
            // 读excel
            FileStream fs = new FileStream(s_strXLSFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            HSSFSheet sheet = Common.GetSheet(fs, "SwitchCHR");
            // 编译代码
            int bank, addr, size;
            Byte[] code = Common.CompileSheet(sheet, out bank, out addr, out size);
            // 注意，由于地址不确定，所以需要预先编译一遍代码，然后根据获得的代码大小重新计算首地址
            addr = curFixedAddr - code.Length;
            code = Common.CompileSheet(sheet, out bank, out addr, out size, addr);

            fs.Close();

            // SwitchCHR始终位于HijackIRQ的前面，因此根据大小计算首地址
            addr = curFixedAddr - code.Length;
            s_AddrSwitchCHR = addr;

            Byte[] prgData = Common.GetPRGData(s_strRomFileName);
            // 写入ROM
            for (int i = 0; i < code.Length; ++i)
            {
                prgData[Common.GetPRGROMAddress(bank, addr) + i] = code[i];
            }
            // 重新设置固定地址
            curFixedAddr = addr;
            Console.WriteLine("SwitchCHR首地址={0:X4}，大小={1}字节", addr, code.Length);
            Common.PatchROM(s_strRomFileName, prgData, null);
            MessageBox.Show("OK!");
        }

        private void btnPatchJSRDrawVRAM_Click(object sender, EventArgs e)
        {
            Byte[] prgData = Common.GetPRGData(s_strRomFileName);

            // 0x8269@Bank1C的jsr DrawVRAM改成NOP
            for(int i = 0; i < 3; ++i)
            {
                prgData[Common.GetPRGROMAddress(0x1C, 0x8269) + i] = 0xEA;
            }
            // 0x8274@Bank1C的jsr DrawVRAM改成NOP
            for (int i = 0; i < 3; ++i)
            {
                prgData[Common.GetPRGROMAddress(0x1C, 0x8274) + i] = 0xEA;
            }
            // 0x827F@Bank1C的jsr DrawVRAM改成jsr SwitchCHR
            prgData[Common.GetPRGROMAddress(0x1C, 0x827F) + 1] = (Byte)(s_AddrSwitchCHR & 0xFF);
            prgData[Common.GetPRGROMAddress(0x1C, 0x827F) + 2] = (Byte)(s_AddrSwitchCHR >> 8);

            Common.PatchROM(s_strRomFileName, prgData, null);
            MessageBox.Show("OK!");
        }

        private void btnSwitchCHRForIntro_Click(object sender, EventArgs e)
        {
            // 读excel
            FileStream fs = new FileStream(s_strXLSFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            HSSFSheet sheet = Common.GetSheet(fs, "SwitchCHRForIntro");
            // 编译代码
            int bank, addr, size;
            Byte[] code = Common.CompileSheet(sheet, out bank, out addr, out size);
            // 注意，由于地址不确定，所以需要预先编译一遍代码，然后根据获得的代码大小重新计算首地址
            addr = curFixedAddr - code.Length;
            code = Common.CompileSheet(sheet, out bank, out addr, out size, addr);
            fs.Close();

            // SwitchCHRForIntro始终位于SwitchCHR的前面，因此根据大小计算首地址
            addr = curFixedAddr - code.Length;

            Byte[] prgData = Common.GetPRGData(s_strRomFileName);
            // 写入ROM
            for (int i = 0; i < code.Length; ++i)
            {
                prgData[Common.GetPRGROMAddress(bank, addr) + i] = code[i];
            }
            // 0xDB69@Bank1E的sta $2001改成jsr SwitchCHRForIntro
            prgData[Common.GetPRGROMAddress(0x1E, 0xDB69) + 0] = 0x20;
            prgData[Common.GetPRGROMAddress(0x1E, 0xDB69) + 1] = (Byte)(addr & 0xFF);
            prgData[Common.GetPRGROMAddress(0x1E, 0xDB69) + 2] = (Byte)(addr >> 8);

            // 重新设置固定地址
            curFixedAddr = addr;
            Console.WriteLine("SwitchCHRForIntro首地址={0:X4}，大小={1}字节", addr, code.Length);
            Common.PatchROM(s_strRomFileName, prgData, null);
            MessageBox.Show("OK!");
        }

        private void btnImportFontForIntro_Click(object sender, EventArgs e)
        {
            string str = "任务目标摧毁敌方秘密武器祝你好运";
            Font font = Common.LoadCustomTTFFont("C:/Users/雷精灵/Desktop/SIMSUN.TTC", 9);
            Bitmap img = new Bitmap(192, 16, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            Graphics g = Graphics.FromImage(img);
            g.Clear(Color.Black);
            g.DrawString(str, font, new SolidBrush(TileCreator.DEFAULT_COLORS[3]), new Point(-2, 2));
            //img.Save("temp.png", System.Drawing.Imaging.ImageFormat.Png);

            // 拆成若干个8*8的tile，然后分别转成数据，再逐一写到指定的位置
            Byte[] chrData = Common.GetCHRData(s_strRomFileName);
            List<Byte[]> datas = new List<Byte[]>();
            for(int y = 0; y < img.Height; y += 8)
            {
                for(int x = 0; x < img.Width; x += 8)
                {
                    Bitmap tile = new Bitmap(8, 8, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                    Graphics gTile = Graphics.FromImage(tile);
                    gTile.DrawImage(img, new Rectangle(0, 0, 8, 8), new Rectangle(x, y, 8, 8), GraphicsUnit.Pixel);
                    //tile.Save(string.Format("{0:X2}.png", x / 8 + y * img.Height / 8), System.Drawing.Imaging.ImageFormat.Png);
                    Byte[] data = TileCreator.Bitmap2Data(tile);
                    datas.Add(data);
                }
            }
            for (int i = 0; i < posIntro.Length; ++i)
            {
                Byte[] data = datas[i];
                for(int j = 0; j < data.Length; ++j)
                {
                    chrData[Common.GetCHRROMAddress(8, 0) + posIntro[i] * 16 + j] = data[j];
                }
            }
            Common.PatchROM(s_strRomFileName, null, chrData);
            MessageBox.Show("OK!");
        }

        private void btnRebuildIntroNametable_Click(object sender, EventArgs e)
        {
            Byte[] prgData = Common.GetPRGData(s_strRomFileName);
            int startAddr = 0x62A2; // 关键地址！！！
            int endAddr = 0;
            // 写关键地址
            prgData[Common.GetPRGROMAddress(0x1C, 0x92BC + FillNTRAM_StartAddr_Table_Offset_FillIntro * 2) + 0] = (Byte)(startAddr & 0xFF);
            prgData[Common.GetPRGROMAddress(0x1C, 0x92BC + FillNTRAM_StartAddr_Table_Offset_FillIntro * 2) + 1] = (Byte)(startAddr >> 8);
            // 首先解压缩nametable
            List<Byte> uncompressedNT = new List<Byte>();
            // 一共四个block
            int[] blockAddr = new int[4];
            int offset = 0;
            for(int i = 0; i < blockAddr.Length; ++i)
            {
                Byte addrLo = prgData[Common.GetPRGROMAddress(3, startAddr - 0x6000) + offset];
                offset++;
                Byte addrHi = prgData[Common.GetPRGROMAddress(3, startAddr - 0x6000) + offset];
                offset++;
                int addr = ((int)addrHi << 8) | (int)addrLo;
                blockAddr[i] = addr;
            }
            offset = 0;
            // 读每个block
            bool isExit = false;
            bool isRLE = false;
            int rleLength = -1;
            foreach(int addr in blockAddr)
            {
                offset = 0;
                isExit = false;
                isRLE = false;
                rleLength = -1;
                while (true)
                {
                    Byte data = prgData[Common.GetPRGROMAddress(3, addr - 0x6000) + offset];
                    offset++;
                    if (isRLE)
                    {
                        if(rleLength < 0)
                        {
                            rleLength = data;
                        }else
                        {
                            for(int i = 0; i < rleLength; ++i)
                            {
                                uncompressedNT.Add(data);
                            }
                            rleLength = -1;
                            isRLE = false;
                        }
                    }else
                    {
                        switch (data)
                        {
                            case 0x34:
                                {
                                    isRLE = true;
                                    break;
                                }
                            case 0x39:
                                {
                                    isExit = true;
                                    break;
                                }
                            default:
                                {
                                    uncompressedNT.Add(data);
                                    break;
                                }
                        }
                    }
                    if (isExit)
                        break;
                }
            }
            endAddr = blockAddr[3] + offset;
            Console.WriteLine(string.Format("NT size={0}, compressed size={1}", uncompressedNT.Count, endAddr - startAddr));

            // 搜索NT的前960字节，把所有被汉字占用的tile索引都改为0
            for(int i = 0; i < 960; ++i)
            {
                for(int j = 0; j < posIntro.Length; ++j)
                {
                    if (uncompressedNT[i] == posIntro[j])
                        uncompressedNT[i] = 0;
                }
            }

            // 压缩nametable
            List<List<Byte>> compressedNT = new List<List<Byte>>();
            compressedNT.Add(new List<Byte>());
            int curblock = 0;
            offset = 0;
            List<Byte> tmp = new List<Byte>();
            while (offset < uncompressedNT.Count)
            {
                Byte data = uncompressedNT[offset];
                offset++;
                if(tmp.Count == 0 || tmp[tmp.Count - 1] == data)
                {
                    tmp.Add(data);
                }
                else
                {
                    while(tmp.Count > 0)
                    {
                        List<Byte> tmpComp = new List<Byte>();
                        if (tmp.Count >= 4 && tmp.Count < 256 && tmp.Count != 0x34 && tmp.Count != 0x39)
                        {
                            // 如果这个tmp的长度>=4且<256，则可以压缩
                            tmpComp.Add(0x34);
                            tmpComp.Add((Byte)tmp.Count);
                            tmpComp.Add(tmp[0]);
                            tmp.Clear();
                            tmp.Add(data);
                            // 如果当前数据块长度超过0xFE则开辟新block
                            _tryCreateNewBlock(ref compressedNT, ref curblock, ref tmpComp);
                            compressedNT[curblock].AddRange(tmpComp);
                            break;
                        }
                        else if (tmp.Count >= 256)
                        {
                            // 如果这个tmp的长度>=256则分成若干次压缩
                            tmpComp.Add(0x34);
                            tmpComp.Add((Byte)tmp.Count);
                            tmpComp.Add(tmp[0]);
                            tmp.RemoveRange(0, 256);
                            // 如果当前数据块长度超过0xFE则开辟新block
                            _tryCreateNewBlock(ref compressedNT, ref curblock, ref tmpComp);
                            compressedNT[curblock].AddRange(tmpComp);
                        }
                        else if(tmp.Count == 0x34)
                        {
                            // 如果这个tmp的长度正好==0x34则分成两次压缩
                            tmpComp.Add(0x34);
                            tmpComp.Add(0x33);
                            tmpComp.Add(tmp[0]);
                            tmpComp.Add(tmp[0]);
                            tmp.Clear();
                            tmp.Add(data);
                            // 如果当前数据块长度超过0xFE则开辟新block
                            _tryCreateNewBlock(ref compressedNT, ref curblock, ref tmpComp);
                            compressedNT[curblock].AddRange(tmpComp);
                            break;
                        }
                        else if (tmp.Count == 0x39)
                        {
                            // 如果这个tmp的长度正好==0x39则分成两次压缩
                            tmpComp.Add(0x34);
                            tmpComp.Add(0x38);
                            tmpComp.Add(tmp[0]);
                            tmpComp.Add(tmp[0]);
                            tmp.Clear();
                            tmp.Add(data);
                            // 如果当前数据块长度超过0xFE则开辟新block
                            _tryCreateNewBlock(ref compressedNT, ref curblock, ref tmpComp);
                            compressedNT[curblock].AddRange(tmpComp);
                            break;
                        }
                        else
                        {
                            // 如果这个tmp的长度<4则不压缩
                            tmpComp.AddRange(tmp);
                            tmp.Clear();
                            tmp.Add(data);
                            // 如果当前数据块长度超过0xFE则开辟新block
                            _tryCreateNewBlock(ref compressedNT, ref curblock, ref tmpComp);
                            compressedNT[curblock].AddRange(tmpComp);
                            break;
                        }
                    }
                }
            }
            compressedNT[curblock].Add(0x39);

            foreach(List<Byte> block in compressedNT)
            {
                Console.WriteLine(string.Format("new compressed size={0}", block.Count));
            }

            // 重建数据块首地址指针表
            int _addr = startAddr + 8;
            for (int i = 0; i < compressedNT.Count; ++i)
            {
                blockAddr[i] = _addr;
                _addr += compressedNT[i].Count;
            }
            for(int i = compressedNT.Count; i < 4; ++i)
            {
                blockAddr[i] = _addr - 1;
            }
            // 写入ROM
            for (int i = 0; i < blockAddr.Length; ++i)
            {
                prgData[Common.GetPRGROMAddress(3, startAddr - 0x6000) + i * 2 + 0] = (Byte)(blockAddr[i] & 0xFF);
                prgData[Common.GetPRGROMAddress(3, startAddr - 0x6000) + i * 2 + 1] = (Byte)(blockAddr[i] >> 8);
            }
            curblock = 0;
            foreach(List<Byte> block in compressedNT)
            {
                for (int i = 0; i < block.Count; ++i)
                {
                    prgData[Common.GetPRGROMAddress(3, blockAddr[curblock] - 0x6000) + i] = block[i];
                }
                curblock++;
            }
            Common.PatchROM(s_strRomFileName, prgData, null);
            MessageBox.Show("OK!");
        }

        private void _tryCreateNewBlock(ref List<List<Byte>> compressedNT, ref int curblock, ref List<Byte> tmpComp)
        {
            // 如果当前压缩块加上本压缩块超出0xFE字节，则新开辟压缩块，否则本压缩块添加到当前压缩块中
            if (compressedNT[curblock].Count + tmpComp.Count > 0xFE)
            {
                compressedNT[curblock].Add(0x39);
                curblock++;
                compressedNT.Add(new List<Byte>());
            }
        }

        private int TextLocalization(int idx, int startAddr, int startBank, int[] strAddr, List<Byte[]> strTileIdx, int[] pos)
        {
            Byte[] prgData = Common.GetPRGData(s_strRomFileName);
            // 创建数据块
            List<Byte> dataBlock = new List<Byte>();
            for (int y = 0; y < strTileIdx.Count; y += 2)
            {
                for (int x = 0; x < strTileIdx[y].Length; x += strTileIdx[y].Length)
                {
                    dataBlock.Add((Byte)(strAddr[y + 0] >> 8));
                    dataBlock.Add((Byte)((strAddr[y + 0] & 0xFF) + x));
                    for (int i = 0; i < strTileIdx[y].Length; ++i)
                    {
                        int v = strTileIdx[y + 0][x + i];
                        if (v >= 0x80)
                            dataBlock.Add((Byte)(v - 0x80));
                        else
                            dataBlock.Add((Byte)pos[v]);

                    }
                    dataBlock.Add(0x41);
                    dataBlock.Add((Byte)(strAddr[y + 1] >> 8));
                    dataBlock.Add((Byte)((strAddr[y + 1] & 0xFF) + x));
                    for (int i = 0; i < strTileIdx[y].Length; ++i)
                    {
                        int v = strTileIdx[y + 1][x + i];
                        if (v >= 0x80)
                            dataBlock.Add((Byte)(v - 0x80));
                        else
                            dataBlock.Add((Byte)pos[v]);
                    }
                    dataBlock.Add(0x41);
                }
            }
            dataBlock.RemoveAt(dataBlock.Count - 1);
            dataBlock.Add(0x40);

            Console.WriteLine("idx={0}, size={1}", idx, dataBlock.Count);

            for (int i = 0; i < dataBlock.Count; ++i)
            {
                prgData[Common.GetPRGROMAddress(startBank, startAddr - dataBlock.Count) + i] = dataBlock[i];
            }

            // 写关键地址
            prgData[Common.GetPRGROMAddress(0x1C, 0x8B59) + idx * 2 + 0] = (Byte)((startAddr - dataBlock.Count) & 0xFF);
            prgData[Common.GetPRGROMAddress(0x1C, 0x8B59) + idx * 2 + 1] = (Byte)((startAddr - dataBlock.Count) >> 8);

            Common.PatchROM(s_strRomFileName, prgData, null);

            return dataBlock.Count;
        }

        private void btnIntroLocalization_Click(object sender, EventArgs e)
        {
            int startAddr = curFixedAddr;//0x8B7F; // 关键地址！！！现在改到新的地址以容纳更多的内容
            int startBank = 0x1F;//0x1C;   // 关键bank！！！现在改到fixedbank，因为原本bank没空间了
            int[] strAddr = new int[] { 0x2087, 0x20A7, 0x20CA, 0x20EA, 0x21CD, 0x21ED };
            List<Byte[]> strTileIdx = new List<Byte[]>()
            {
                new Byte[] { 0,1,2,3,4,5, },
                new Byte[] { 24,25,26,27,28,29, },  // 任务目标
                new Byte[] { 6,7,8,9,10,11,12,13,14,15,16,17, },
                new Byte[] { 30,31,32,33,34,35,36,37,38,39,40,41, },    // 摧毁敌方秘密武器
                new Byte[] { 18,19,20,21,22,23, },
                new Byte[] { 42,43,44,45,46,47, },  // 祝你好运
            };
            int size = TextLocalization(0, startAddr, startBank, strAddr, strTileIdx, posIntro);
            curFixedAddr -= size;
            MessageBox.Show("OK!");
        }

        private void btnInGameHUDLocalization_Click(object sender, EventArgs e)
        {
            int startAddr = 0x97DF; // 关键地址！！！
            Byte[] newData = new Byte[] {
                0x20, 0x40, 0x00, 0xFF,
                0x20, 0x56, 0x00, 0xFF,
                0x20, 0x2B, 0x0B, 0x0C, 0x00, 0xFF,
                0x20, 0x20, 0x02, 0x0E, 0x00, 0xFF,
                0x20, 0x36, 0x03, 0x0E, 0x00, 0xFF,
                0x20, 0x45, 0x00, 0x0F, 0x10, 0x00, 0xFF,
                0x20, 0x5B, 0x00, 0x0F, 0x10, 0x00, 0xFF,
            };
            Byte[] prgData = Common.GetPRGData(s_strRomFileName);
            for (int i = 0; i < newData.Length; ++i)
            {
                prgData[Common.GetPRGROMAddress(0x1C, startAddr) + i] = newData[i];
            }

            // 导入图片
            string imgFileName = s_strRootPath + "ingamehud.png";
            Bitmap img = new Bitmap(imgFileName);
            Byte[] data = TileCreator.Bitmap2Data(img);

            Byte[] chrData = Common.GetCHRData(s_strRomFileName);
            int[] tileIdx = new int[] { 0x0B, 0x0C, 0x0F, 0x10 };
            for(int i = 0; i < 6; ++i)
            {
                int offset = 0;
                for (int j = 0; j < tileIdx.Length; ++j)
                {
                    for(int k = 0; k < 16; ++k)
                    {
                        chrData[Common.GetCHRROMAddress((i + 2) * 8, tileIdx[j] * 16) + k] = data[offset];
                        offset++;
                    }
                }
            }

            Common.PatchROM(s_strRomFileName, prgData, chrData);
            MessageBox.Show("OK!");
        }

        private void btnImportFontForStageClear_Click(object sender, EventArgs e)
        {
            string str = "干得漂亮｜第关已攻克继续战斗马上就要到达目标点了！";
            Font font = Common.LoadCustomTTFFont("C:/Users/雷精灵/Desktop/SIMSUN.TTC", 9);
            Bitmap img = new Bitmap(296, 16, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            Graphics g = Graphics.FromImage(img);
            g.Clear(Color.Black);
            g.DrawString(str, font, new SolidBrush(TileCreator.DEFAULT_COLORS[3]), new Point(-2, 4));
            img.Save("temp.png", System.Drawing.Imaging.ImageFormat.Png);

            // 拆成若干个8*8的tile，然后分别转成数据，再逐一写到指定的位置
            Byte[] chrData = Common.GetCHRData(s_strRomFileName);
            List<Byte[]> datas = new List<Byte[]>();
            for (int y = 0; y < img.Height; y += 8)
            {
                for (int x = 0; x < img.Width; x += 8)
                {
                    Bitmap tile = new Bitmap(8, 8, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                    Graphics gTile = Graphics.FromImage(tile);
                    gTile.DrawImage(img, new Rectangle(0, 0, 8, 8), new Rectangle(x, y, 8, 8), GraphicsUnit.Pixel);
                    //tile.Save(string.Format("{0:X2}.png", x / 8 + y * img.Height / 8), System.Drawing.Imaging.ImageFormat.Png);
                    Byte[] data = TileCreator.Bitmap2Data(tile);
                    datas.Add(data);
                }
            }
            for (int i = 0; i < posStageClear.Length; ++i)
            {
                Byte[] data = datas[i];
                for (int j = 0; j < data.Length; ++j)
                {
                    chrData[Common.GetCHRROMAddress(0x40, 0) + posStageClear[i] * 16 + j] = data[j];
                }
            }
            Common.PatchROM(s_strRomFileName, null, chrData);
            MessageBox.Show("OK!");
        }

        private void btnStageClearLocalization_Click(object sender, EventArgs e)
        {
            int size = 0;
            int startAddr = curFixedAddr;//0x6F1F; // 关键地址！！！现在改到新的地址以容纳更多的内容
            int startBank = 0x1F;//0x??;   // 关键bank！！！现在改到fixedbank，因为原本bank没空间了
            int[] strAddr = new int[] { 0x2087, 0x20A7, 0x20CA, 0x20EA };
            for(int i = 0; i < 4; ++i)
            {
                List<Byte[]> strTileIdx = new List<Byte[]>()
                {
                    new Byte[] { 0,1,2,3,4,5,36, },
                    new Byte[] { 37,38,39,40,41,42,73, },  // 干得漂亮！
                    new Byte[] { 7,8,0x80,9,10,11,12,13,14,36, },
                    new Byte[] { 44,45,(Byte)(0x82 + i),46,47,48,49,50,51,73, },    // 第x关已攻克！
                };
                size = TextLocalization(i + 1, curFixedAddr, startBank, strAddr, strTileIdx, posStageClear);
                curFixedAddr -= size;
            }

            int startAddr2 = curFixedAddr;
            int[] strAddr2 = new int[] { 0x2087, 0x20A7, 0x210A, 0x212A, 0x214A, 0x216A };
            List<Byte[]> strTileIdx2 = new List<Byte[]>()
            {
                new Byte[] { 0,1,2,3,4,5,7,8,0x80,9,10,11,12,13,14,36, },
                new Byte[] { 37,38,39,40,41,42,44,45,0x86,46,47,48,49,50,51,73, },  // 干得漂亮！第x关已攻克！
                new Byte[] { 15,16,17,18,19,20,36, },
                new Byte[] { 52,53,54,55,56,57,73, },  // 继续战斗！
                new Byte[] { 21,22,23,24,25,26,27,28,29,30,31,32,33,34,35,36, },
                new Byte[] { 58,59,60,61,62,63,64,65,66,67,68,69,70,71,72,73, },  // 马上就要到达目标点了！
            };
            size = TextLocalization(5, startAddr2, startBank, strAddr2, strTileIdx2, posStageClear);
            curFixedAddr -= size;
            MessageBox.Show("OK!");
        }

        private void btnImportFontForEnding_Click(object sender, EventArgs e)
        {
            string str = "恭喜你拯救了世界终于和平了！！";
            Font font = Common.LoadCustomTTFFont("C:/Users/雷精灵/Desktop/SIMSUN.TTC", 9);
            Bitmap img = new Bitmap(176, 16, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            Graphics g = Graphics.FromImage(img);
            g.Clear(TileCreator.DEFAULT_COLORS[1]);
            g.DrawString(str, font, new SolidBrush(TileCreator.DEFAULT_COLORS[3]), new Point(-2, 3));
            img.Save("temp.png", System.Drawing.Imaging.ImageFormat.Png);

            // 拆成若干个8*8的tile，然后分别转成数据，再逐一写到指定的位置
            Byte[] chrData = Common.GetCHRData(s_strRomFileName);
            List<Byte[]> datas = new List<Byte[]>();
            for (int y = 0; y < img.Height; y += 8)
            {
                for (int x = 0; x < img.Width; x += 8)
                {
                    Bitmap tile = new Bitmap(8, 8, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                    Graphics gTile = Graphics.FromImage(tile);
                    gTile.DrawImage(img, new Rectangle(0, 0, 8, 8), new Rectangle(x, y, 8, 8), GraphicsUnit.Pixel);
                    //tile.Save(string.Format("{0:X2}.png", x / 8 + y * img.Height / 8), System.Drawing.Imaging.ImageFormat.Png);
                    Byte[] data = TileCreator.Bitmap2Data(tile);
                    datas.Add(data);
                }
            }
            for (int i = 0; i < posEnding.Length; ++i)
            {
                Byte[] data = datas[i];
                for (int j = 0; j < data.Length; ++j)
                {
                    chrData[Common.GetCHRROMAddress(0x48, 0) + posEnding[i] * 16 + j] = data[j];
                }
            }
            Common.PatchROM(s_strRomFileName, null, chrData);
            MessageBox.Show("OK!");
        }

        private void btnEndingLocalization_Click(object sender, EventArgs e)
        {
            int startAddr = curFixedAddr;//0x70B7; // 关键地址！！！现在改到新的地址以容纳更多的内容
            int startBank = 0x1F;//0x1C;   // 关键bank！！！现在改到fixedbank，因为原本bank没空间了
            int[] strAddr = new int[] { 0x204A, 0x206A, 0x208A, 0x20AA };
            List<Byte[]> strTileIdx = new List<Byte[]>()
            {
                new Byte[] { 0,1,2,3,4,5,6,7,8,9,10,11,21 },
                new Byte[] { 22,23,24,25,26,27,28,29,30,31,32,33,43 },  // 恭喜你拯救了世界！
                new Byte[] { 9,10,11,12,13,14,15,16,17,18,19,20 },
                new Byte[] { 31,32,33,34,35,36,37,38,39,40,41,42 },    // 世界终于和平了！
            };
            int size = TextLocalization(6, startAddr, startBank, strAddr, strTileIdx, posEnding);
            curFixedAddr -= size;
            MessageBox.Show("OK!");
        }
        //9938  97c9
        private void btnImportFontForGameOver_Click(object sender, EventArgs e)
        {
            Font font = Common.LoadCustomTTFFont("C:/Users/雷精灵/Desktop/SIMSUN.TTC", 9);
            Bitmap img = new Bitmap(14 * 8, 16, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            Graphics g = Graphics.FromImage(img);
            g.Clear(TileCreator.DEFAULT_COLORS[0]);
            g.DrawString("游戏结束继续？", font, new SolidBrush(TileCreator.DEFAULT_COLORS[3]), new Point(-2, 3));
            g.DrawString("是", font, new SolidBrush(TileCreator.DEFAULT_COLORS[3]), new Point(10 * 8 - 2, 3));
            g.DrawString("否", font, new SolidBrush(TileCreator.DEFAULT_COLORS[3]), new Point(12 * 8 - 2, 3));
            //g.DrawString("开发团队程序女装大佬角色设计音乐总监图像作曲特别感谢汉化你妹雷精灵", font, new SolidBrush(TileCreator.DEFAULT_COLORS[3]), new Point(14 * 8 - 2, 3));
            // 开发团队
            // 请按START键
            img.Save("temp.png", System.Drawing.Imaging.ImageFormat.Png);

            // 拆成若干个8*8的tile，然后分别转成数据，再逐一写到指定的位置
            Byte[] chrData = Common.GetCHRData(s_strRomFileName);
            List<Byte[]> datas = new List<Byte[]>();
            for (int y = 0; y < img.Height; y += 8)
            {
                for (int x = 0; x < img.Width; x += 8)
                {
                    Bitmap tile = new Bitmap(8, 8, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                    Graphics gTile = Graphics.FromImage(tile);
                    gTile.DrawImage(img, new Rectangle(0, 0, 8, 8), new Rectangle(x, y, 8, 8), GraphicsUnit.Pixel);
                    //tile.Save(string.Format("{0:X2}.png", x / 8 + y * img.Height / 8), System.Drawing.Imaging.ImageFormat.Png);
                    Byte[] data = TileCreator.Bitmap2Data(tile);
                    datas.Add(data);
                }
            }
            for (int i = 0; i < posGameOver.Length; ++i)
            {
                try
                {
                    Byte[] data = datas[i];
                    for (int j = 0; j < data.Length; ++j)
                    {
                        chrData[Common.GetCHRROMAddress(0x50, 0) + posGameOver[i] * 16 + j] = data[j];
                    }
                }catch(Exception exc)
                {
                    break;
                }
            }
            Common.PatchROM(s_strRomFileName, null, chrData);
            MessageBox.Show("OK!");
        }

        private void btnGameOverLocalization_Click(object sender, EventArgs e)
        {
            Byte[] prgData = Common.GetPRGData(s_strRomFileName);

            //int startAddr = curFixedAddr; // 关键地址！！！现在改到新的地址以容纳更多的内容
            int startBank = 0x1F;//0x1C;   // 关键bank！！！现在改到fixedbank，因为原本bank没空间了

            // 游戏结束
            if (true)
            {
                Byte[] data = new Byte[]
                {
                    0x21, 0x8D, // 地址
                    posGameOver[0], posGameOver[1], posGameOver[2], posGameOver[3], posGameOver[4], posGameOver[5],
                    0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
                    posGameOver[14], posGameOver[15], posGameOver[16], posGameOver[17], posGameOver[18], posGameOver[19],
                    0xFE,
                };
                curFixedAddr -= data.Length;
                for (int i = 0; i < data.Length; ++i)
                {
                    prgData[Common.GetPRGROMAddress(startBank, curFixedAddr) + i] = data[i];
                }
                // 重建指针
                prgData[Common.GetPRGROMAddress(0x1C, 0x9692)] = (Byte)(curFixedAddr & 0xFF);
                prgData[Common.GetPRGROMAddress(0x1C, 0x9693)] = (Byte)(curFixedAddr >> 8);

                Console.WriteLine(string.Format("游戏结束地址={0:X4}", curFixedAddr));
            }
            // 继续游戏？
            if (true)
            {
                Byte[] data = new Byte[]
                {
                    0x21, 0xCD, // 地址
                    posGameOver[6], posGameOver[7], posGameOver[8], posGameOver[0], posGameOver[1], posGameOver[2], posGameOver[9],
                    0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
                    posGameOver[20], posGameOver[21], posGameOver[22], posGameOver[14], posGameOver[15], posGameOver[16], posGameOver[23],
                    0xFE,
                };
                curFixedAddr -= data.Length;
                for (int i = 0; i < data.Length; ++i)
                {
                    prgData[Common.GetPRGROMAddress(startBank, curFixedAddr) + i] = data[i];
                }
                // 重建指针
                prgData[Common.GetPRGROMAddress(0x1C, 0x969A)] = (Byte)(curFixedAddr & 0xFF);
                prgData[Common.GetPRGROMAddress(0x1C, 0x969B)] = (Byte)(curFixedAddr >> 8);

                Console.WriteLine(string.Format("继续游戏？地址={0:X4}", curFixedAddr));
            }
            // 是
            if (true)
            {
                Byte[] data = new Byte[]
                {
                    0x22, 0x2E, // 地址
                    posGameOver[10], posGameOver[11],
                    0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
                    posGameOver[24], posGameOver[25],
                    0xFE,
                };
                curFixedAddr -= data.Length;
                for (int i = 0; i < data.Length; ++i)
                {
                    prgData[Common.GetPRGROMAddress(startBank, curFixedAddr) + i] = data[i];
                }
                // 重建指针
                prgData[Common.GetPRGROMAddress(0x1C, 0x969C)] = (Byte)(curFixedAddr & 0xFF);
                prgData[Common.GetPRGROMAddress(0x1C, 0x969D)] = (Byte)(curFixedAddr >> 8);

                Console.WriteLine(string.Format("是地址={0:X4}", curFixedAddr));
            }
            // 否
            if (true)
            {
                Byte[] data = new Byte[]
                {
                    0x22, 0x6E, // 地址
                    posGameOver[12], posGameOver[13],
                    0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
                    posGameOver[26], posGameOver[27],
                    0xFE,
                };
                curFixedAddr -= data.Length;
                for (int i = 0; i < data.Length; ++i)
                {
                    prgData[Common.GetPRGROMAddress(startBank, curFixedAddr) + i] = data[i];
                }
                // 重建指针
                prgData[Common.GetPRGROMAddress(0x1C, 0x969E)] = (Byte)(curFixedAddr & 0xFF);
                prgData[Common.GetPRGROMAddress(0x1C, 0x969F)] = (Byte)(curFixedAddr >> 8);

                Console.WriteLine(string.Format("否地址={0:X4}", curFixedAddr));
            }
            
            Common.PatchROM(s_strRomFileName, prgData, null);
            MessageBox.Show("OK!");
        }

        private void btnImportFontForStaff_Click(object sender, EventArgs e)
        {
            Font font = Common.LoadCustomTTFFont("C:/Users/雷精灵/Desktop/SIMSUN.TTC", 9);
            Bitmap img = new Bitmap(416, 16, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            Graphics g = Graphics.FromImage(img);
            g.Clear(TileCreator.DEFAULT_COLORS[0]);
            g.DrawString("开发团队程序角色设计音乐总监图像鸡鸡作曲特别感谢汉化你妹雷精灵", font, new SolidBrush(TileCreator.DEFAULT_COLORS[3]), new Point(-2, 3));
            g.DrawString("请按键", font, new SolidBrush(TileCreator.DEFAULT_COLORS[3]), new Point(376 - 2, 5));
            img.Save("temp.png", System.Drawing.Imaging.ImageFormat.Png);

            // 拆成若干个8*8的tile，然后分别转成数据，再逐一写到指定的位置
            Byte[] chrData = Common.GetCHRData(s_strRomFileName);
            List<Byte[]> datas = new List<Byte[]>();
            for (int y = 0; y < img.Height; y += 8)
            {
                for (int x = 0; x < img.Width; x += 8)
                {
                    Bitmap tile = new Bitmap(8, 8, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                    Graphics gTile = Graphics.FromImage(tile);
                    gTile.DrawImage(img, new Rectangle(0, 0, 8, 8), new Rectangle(x, y, 8, 8), GraphicsUnit.Pixel);
                    //tile.Save(string.Format("{0:X2}.png", x / 8 + y * img.Height / 8), System.Drawing.Imaging.ImageFormat.Png);
                    Byte[] data = TileCreator.Bitmap2Data(tile);
                    datas.Add(data);
                }
            }
            for (int i = 0; i < posStaff.Length; ++i)
            {
                Byte[] data = datas[i];
                for (int j = 0; j < data.Length; ++j)
                {
                    chrData[Common.GetCHRROMAddress(0x58, 0) + posStaff[i] * 16 + j] = data[j];
                }
            }
            // &符号
            img = new Bitmap(s_strRootPath + "and.png");
            if (true)
            {
                Byte[] data = TileCreator.Bitmap2Data(img);
                for(int i = 0; i < data.Length; ++i)
                {
                    chrData[Common.GetCHRROMAddress(0x58, 0x25 * 16) + i] = data[i];
                }
            }

            // 哎呀！嘿呀！
            img = new Bitmap(s_strRootPath + "ouch.png");
            if (true)
            {
                Byte[] data = TileCreator.Bitmap2Data(img);
                for(int j = 0x5F; j <= 0x71; j += 2)
                {
                    for (int i = 0; i < 16; ++i)
                    {
                        chrData[Common.GetCHRROMAddress(0x5D, j * 16) + i] = data[((j - 0x5F) / 2) * 16 + i];
                    }
                }
            }
            
            Common.PatchROM(s_strRomFileName, null, chrData);
            MessageBox.Show("OK!");
        }

        private void btnStaffLocalization_Click(object sender, EventArgs e)
        {
            Byte[] prgData = Common.GetPRGData(s_strRomFileName);

            List<Byte[]> data = new List<Byte[]>()
            {
                // 总监：Phazer Atsuku
                new Byte[]
                {
                    0x21, 0x64,     posStaff[18], posStaff[19], posStaff[20],   0x41,
                    0x21, 0x84,     posStaff[70], posStaff[71], posStaff[72],   0x41,
                    0x21, 0xA4,     0x1A, 0x12, 0x0B, 0x24, 0x0F, 0x1C, 0x00, 0x0B, 0x1E, 0x1D, 0x1F, 0x15, 0x19,   0x40
                },
                // 程序：Naniwa no Hideo
                new Byte[] {
                    0x21, 0x64,     posStaff[6], posStaff[7], posStaff[8],   0x41,
                    0x21, 0x84,     posStaff[58], posStaff[59], posStaff[60],   0x41,
                    0x21, 0xA4,     0x18, 0x0B, 0x18, 0x13, 0x21, 0x0B, 0x00, 0x18, 0x19, 0x00, 0x12, 0x13, 0x0E, 0x0F, 0x19,   0x40
                },
                // 作曲：Rusher Sakamoto
                // 作曲：Iku Iku Mizutani
                new Byte[]
                {
                    0x21, 0x64,     posStaff[27], posStaff[28], posStaff[29],   0x41,
                    0x21, 0x84,     posStaff[79], posStaff[80], posStaff[81],   0x41,
                    0x21, 0xA4,     0x1C, 0x1F, 0x1D, 0x12, 0x0F, 0x1C, 0x00, 0x1D, 0x0B, 0x15, 0x0B, 0x17, 0x19, 0x1E, 0x19,   0x41,
                    0x21, 0xC4,     0x13, 0x15, 0x1F, 0x00, 0x13, 0x15, 0x1F, 0x00, 0x17, 0x13, 0x24, 0x1F, 0x1E, 0x0B, 0x18, 0x13,     0x40,
                },
                // 作曲：Hevimeta Satoe
                // 作曲：Nanda Adachi
                new Byte[]
                {
                    0x21, 0xA4,     0x12, 0x0F, 0x20, 0x13, 0x17, 0x0F, 0x1E, 0x0B, 0x00, 0x1D, 0x0B, 0x1E, 0x19, 0x0F,   0x41,
                    0x21, 0xC4,     0x18, 0x0B, 0x18, 0x0E, 0x0B, 0x00, 0x0B, 0x0E, 0x0B, 0x0D, 0x12, 0x13,     0x40
                },
                // 角色设计：Keiko and Harachan
                // 角色设计：Gokigen Yoshimoto
                new Byte[]
                {
                    0x21, 0x64,     posStaff[9], posStaff[10], posStaff[11], posStaff[12], posStaff[13], posStaff[14],     0x41,
                    0x21, 0x84,     posStaff[61], posStaff[62], posStaff[63], posStaff[64], posStaff[65], posStaff[66],     0x41,
                    0x21, 0xA4,     0x15, 0x0F, 0x13, 0x15, 0x19, 0x00, 0x25, 0x00, 0x12, 0x0B, 0x1C, 0x0B, 0x0D, 0x12, 0x0B, 0x18,     0x41,
                    0x21, 0xC4,     0x11, 0x19, 0x15, 0x13, 0x11, 0x0F, 0x18, 0x00, 0x23, 0x19, 0x1D, 0x12, 0x13, 0x17, 0x19, 0x1E, 0x19,   0x40
                },
                // 音乐设计：Chary Sadakichi
                new Byte[]
                {
                    0x21, 0x64,     posStaff[15], posStaff[16], posStaff[17],   0x41,
                    0x21, 0x84,     posStaff[67], posStaff[68], posStaff[69],   0x41,
                    0x21, 0xA4,     0x0D, 0x12, 0x0B, 0x1C, 0x23, 0x00, 0x1D, 0x0B, 0x0E, 0x0B, 0x15, 0x13, 0x0D, 0x12, 0x13,   0x40
                },
                // 图像设计：Shimoneta Kenchan
                // 图像设计：Daibutsu Mari
                new Byte[]
                {
                    0x21, 0x64,     posStaff[21], posStaff[22], posStaff[23],   0x41,
                    0x21, 0x84,     posStaff[73], posStaff[74], posStaff[75],   0x41,
                    0x21, 0xA4,     0x1D, 0x12, 0x13, 0x17, 0x19, 0x18, 0x0F, 0x1E, 0x0B, 0x00, 0x15, 0x0F, 0x18, 0x0D, 0x12, 0x0B, 0x18,   0x41,
                    0x21, 0xC4,     0x0E, 0x0B, 0x13, 0x0C, 0x1F, 0x1E, 0x1D, 0x1F, 0x00, 0x17, 0x0B, 0x1C, 0x13,   0x40
                },
                // 特别感谢：Hashibou and Shinobu
                new Byte[]
                {
                    0x21, 0x64,     posStaff[30], posStaff[31], posStaff[32], posStaff[33], posStaff[34], posStaff[35],     0x41,
                    0x21, 0x84,     posStaff[82], posStaff[83], posStaff[84], posStaff[85], posStaff[86], posStaff[87],     0x41,
                    0x21, 0xA4,     0x12, 0x0B, 0x1D, 0x12, 0x13, 0x0C, 0x19, 0x1F,     0x41, 
                    0x21, 0xC4,     0x1D, 0x12, 0x13, 0x18, 0x19, 0x0C, 0x1F,       0x40
                },
                // 汉化：汉化你妹 雷精灵
                new Byte[]
                {
                    0x21, 0x64,     posStaff[36], posStaff[37], posStaff[38], 0x00, 0x00, 0x00,     0x41,
                    0x21, 0x84,     posStaff[88], posStaff[89], posStaff[90], 0x00, 0x00, 0x00,     0x41,
                    0x21, 0xA4,     posStaff[36], posStaff[37], posStaff[38], posStaff[39], posStaff[40], posStaff[41], 0x00, posStaff[42], posStaff[43], posStaff[44], posStaff[45], posStaff[46],     0x41,
                    0x21, 0xC4,     posStaff[88], posStaff[89], posStaff[90], posStaff[91], posStaff[92], posStaff[93], 0x00, posStaff[94], posStaff[95], posStaff[96], posStaff[97], posStaff[98],      0x40
                },
                // 清除“汉化”
                new Byte[]
                {
                    0x21, 0x64,     0x00, 0x00, 0x00,     0x41,
                    0x21, 0x84,     0x00, 0x00, 0x00,     0x40,
                },
                // 空
                new Byte[]
                {
                    0x20, 0x00,     0x00,   0x40
                },
            };
            int len = 0;
            foreach(Byte[] b in data)
            {
                len += b.Length;
            }
            Console.WriteLine("新数据块长度={0:X4}", len);

            // 原本数据块大小为396字节
            if(len > 396)
            {
                MessageBox.Show("超出容量上限！强制取消本次数据写入！", "错误！", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int addr = 0x7100;
            for(int j = 0; j < data.Count; ++j)
            {
                prgData[Common.GetPRGROMAddress(0x1C, 0x8B67) + j * 2 + 0] = (Byte)(addr & 0xFF);
                prgData[Common.GetPRGROMAddress(0x1C, 0x8B67) + j * 2 + 1] = (Byte)(addr >> 8);
                Byte[] b = data[j];
                for (int i = 0; i < b.Length; ++i)
                {
                    prgData[Common.GetPRGROMAddress(0x02, 0x1100) + (addr - 0x7100) + i] = b[i];
                }
                addr += b.Length;
            }
            // 剩余的首地址填充空白
            int dummyAddr = addr;
            dummyAddr -= data[data.Count - 1].Length;
            for(int i = data.Count; i < 12; ++i)
            {
                prgData[Common.GetPRGROMAddress(0x1C, 0x8B67) + i * 2 + 0] = (Byte)(dummyAddr & 0xFF);
                prgData[Common.GetPRGROMAddress(0x1C, 0x8B67) + i * 2 + 1] = (Byte)(dummyAddr >> 8);
            }

            Common.PatchROM(s_strRomFileName, prgData, null);
            MessageBox.Show("OK!");
        }

        private void btnStaffLocalization2_Click(object sender, EventArgs e)
        {
            Byte[] prgData = Common.GetPRGData(s_strRomFileName);
            // 重建指针表
            int KeyAddress = 0x96AC;    // 关键地址！
            Byte[] dataStaff = new Byte[] {
                0x20, 0xED,
                posStaff[0], posStaff[1], posStaff[2], posStaff[3], posStaff[4], posStaff[5],
                0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
                0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
                posStaff[52], posStaff[53], posStaff[54], posStaff[55], posStaff[56], posStaff[57],
                0xFE,
            };
            Byte[] dataClearTitle = new Byte[]
            {
                0x21, 0x44,
                0x00,
                0xFE,
            };
            Byte[] dataClearName = new Byte[]
            {
                0x21, 0xA4,
                0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
                0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
                0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
                0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
                0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
                0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
                0xFE,
            };
            Byte[] dataPleasePushStartButton = new Byte[]
            {
                0x20, 0xEB,
                posStaff[47], posStaff[48], posStaff[49], 0x00, 0x00, 0x00, 0x00, 0x00, posStaff[50], posStaff[51],
                0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
                0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
                posStaff[99], posStaff[100], posStaff[101], 0x1D, 0x1E, 0x0B, 0x1C, 0x1E, posStaff[102], posStaff[103],
                0xFE,
            };
            Byte[] dataClearStaff = new Byte[]
            {
                0x20, 0xCD,
                0x00,
                0xFE,
            };
            List<Byte[]> newData = new List<Byte[]>()
            {
                dataStaff, dataClearTitle, dataClearName, dataPleasePushStartButton, dataClearStaff
            };

            // 原本数据块大小为102字节
            if(dataStaff.Length + dataClearTitle.Length + dataPleasePushStartButton.Length + dataClearStaff.Length > 102)
            {
                MessageBox.Show("超出容量上限！强制取消本次数据写入！", "错误！", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int[] newAddress = new int[] {
                0x9A07,
                0x9A07 + dataStaff.Length,
                0x8B81, // 这块内存空间原本是Intro的文本，共59字节，现在将其占用，用于容纳这里的数据
                0x9A07 + dataStaff.Length + dataClearTitle.Length,
                0x9A07 + dataStaff.Length + dataClearTitle.Length + dataPleasePushStartButton.Length,
            };

            for(int i = 0; i < newData.Count; ++i)
            {
                Byte[] data = newData[i];
                for(int j = 0; j < data.Length; ++j)
                {
                    prgData[Common.GetPRGROMAddress(0x1C, newAddress[i]) + j] = data[j];
                }
            }
            for(int i = 0; i < newAddress.Length; ++i)
            {
                prgData[Common.GetPRGROMAddress(0x1C, KeyAddress) + i * 2 + 0] = (Byte)(newAddress[i] & 0xFF);
                prgData[Common.GetPRGROMAddress(0x1C, KeyAddress) + i * 2 + 1] = (Byte)(newAddress[i] >> 8);
            }

            Common.PatchROM(s_strRomFileName, prgData, null);
            MessageBox.Show("OK!");
        }

        private void btnTitleLocalization_Click(object sender, EventArgs e)
        {
            Byte[] prgData = Common.GetPRGData(s_strRomFileName);
            Byte[] chrData = Common.GetCHRData(s_strRomFileName);

            Bitmap imgTitle = new Bitmap(s_strRootPath + "newtitle_mono.png");
            Bitmap imgMask = new Bitmap(s_strRootPath + "newtitle_mask.png");
            TileCreator.CHRInfo info = TileCreator.Image2CHRInfo(imgTitle, imgMask);
            // 先将pattern数据写入CHR
            for(int i = 0; i < info.pattern.Count; ++i)
            {
                chrData[i] = info.pattern[i];
            }
            // 生成nametable+attribtable
            List<Byte> nametable = new List<Byte>();
            nametable.AddRange(info.map);
            nametable.AddRange(info.attrib);
            if(nametable.Count != 1024)
            {
                MessageBox.Show("错误！nametable大小非法！", "错误！", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            // 压缩
            int[] addr = new int[4];
            List<Byte> compressedData = new List<Byte>();
            // 先预留8字节，最后确定地址之后再填充
            compressedData.AddRange(new Byte[8]);
            // 开始压缩
            List<Byte> ret0 = new List<Byte>();
            List<int> ret1 = new List<int>();
            compressNametable(nametable, ref ret0, ref ret1);
            compressedData.AddRange(ret0);
            // 重建指针表
            for(int i = 0; i < addr.Length; ++i)
            {
                addr[i] = ret1[i] + 0x6008;
            }
            // 填充地址
            for (int i = 0; i < addr.Length; ++i)
            {
                compressedData[i * 2 + 0] = (Byte)(addr[i] & 0xFF);
                compressedData[i * 2 + 1] = (Byte)(addr[i] >> 8);
            }
            // 压缩之后的数据大小必须<=337字节
            Console.WriteLine("compressedData=" + compressedData.Count);
            if(compressedData.Count > 337)
            {
                MessageBox.Show(string.Format("错误！压缩数据（{0}字节）太大了！超过了337字节的限制。", compressedData.Count), "错误！", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            // 写入PRG
            for (int i = 0; i < compressedData.Count; ++i)
            {
                prgData[Common.GetPRGROMAddress(0x03, 0) + i] = compressedData[i];
            }

            // 改写选项文字
            if (true)
            {
                // 关键地址！！！
                int keyAddress = 0x9674;
                int menuTextAddr = 0x9794;
                List<Byte[]> data = new List<Byte[]>
                {
                    new Byte[] { 0x22, 0xD0,                0xC2, 0xC3, 0xC4, 0xC5,     0xFE, },
                    new Byte[] { 0x22, 0xEE,    0xC6, 0xC7, 0xC8, 0xC9, 0xCA, 0xCB,     0xFE, },
                    new Byte[] { 0x23, 0x10,                0xC2, 0xC3, 0xC4, 0xC5,     0xFE, },
                    new Byte[] { 0x23, 0x2E,    0xCC, 0xCD, 0xC8, 0xC9, 0xCA, 0xCB,     0xFE, },
                };
                // 写入PRG
                for(int i = 0; i < data.Count; ++i)
                {
                    prgData[Common.GetPRGROMAddress(0x1C, keyAddress) + i * 2 + 0] = (Byte)(menuTextAddr & 0xFF);
                    prgData[Common.GetPRGROMAddress(0x1C, keyAddress) + i * 2 + 1] = (Byte)(menuTextAddr >> 8);
                    for(int j = 0; j < data[i].Length; ++j)
                    {
                        prgData[Common.GetPRGROMAddress(0x1C, menuTextAddr) + j] = data[i][j];
                    }
                    menuTextAddr += data[i].Length;
                }
            }

            // 重建调色盘
            if (true)
            {
                Byte[] data = new Byte[]
                {
                    0x3F, 0x00,
                    0x0F, 0x00, 0x10, 0x30,
                    0x0F, 0x15, 0x19, 0x27,
                    0x0F, 0x00, 0x10, 0x30,
                    0x0F, 0x16, 0x30, 0x27,
                    0x0F, 0x0C, 0x26, 0x30,
                    0x0F, 0x0C, 0x2C, 0x30,
                    0x0F, 0x12, 0x26, 0x30,
                    0x0F, 0x06, 0x30, 0x30,
                    0xFE,
                };
                for (int i = 0; i < data.Length; ++i)
                {
                    prgData[Common.GetPRGROMAddress(0x1C, 0x9807) + i] = data[i];
                }
            }

            // 调整光标位置
            prgData[Common.GetPRGROMAddress(0x1C, 0x82A0)] = 0xBB;

            // 将$963C的sta $0700, X改成jmp $97B4，同时将patch写入$97B4开始的38字节范围内
            if (true)
            {
                Byte[] opCode = new Byte[] { 0x4C, 0xB4, 0x97 };
                for(int i = 0; i < opCode.Length; ++i)
                {
                    prgData[Common.GetPRGROMAddress(0x1C, 0x963C) + i] = opCode[i];
                }
                // 读excel
                FileStream fs = new FileStream(s_strXLSFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                HSSFSheet sheet = Common.GetSheet(fs, "ChangeMenuAttr");
                // 编译代码
                int _bank, _addr, _size;
                Byte[] code = Common.CompileSheet(sheet, out _bank, out _addr, out _size);
                for (int i = 0; i < code.Length; ++i)
                {
                    prgData[Common.GetPRGROMAddress(_bank, _addr) + i] = code[i];
                }
            }

            Common.PatchROM(s_strRomFileName, prgData, chrData);
            MessageBox.Show("OK!");
        }

        private void compressNametable(List<Byte> data, ref List<Byte> ret0, ref List<int> ret1)
        {
            int offset = 0;
            int blockOffset = 0;
            // 先取第0个字节，作为rle初始值
            int rleValue = data[offset];
            offset++;
            int rleLength = 1;
            ret1.Add(0);
            while (true)
            {
                bool isEnd = false;
                rleLength = 1;
                while (true)
                {
                    Byte value = data[blockOffset + offset];
                    offset++;
                    // 快要超过1字节的长度了，强制终止，重开
                    if (offset >= 0xFF)
                    {
                        blockOffset += offset;
                        offset -= 0xFF;
                        if (rleLength >= 4)
                        {
                            ret0.Add(0x34);
                            ret0.Add((Byte)rleLength);
                            ret0.Add((Byte)rleValue);
                        }
                        else
                        {
                            for (int i = 0; i < rleLength; ++i)
                            {
                                ret0.Add((Byte)rleValue);
                            }
                        }
                        ret0.Add(0x39);
                        ret1.Add(ret0.Count);
                        break;
                    }
                    // 绝对禁止要压缩的值为0x34或者0x39！
                    if (value == 0x34 || value == 0x39)
                    {
                        ret0 = null;
                        ret1 = null;
                        return;
                    }
                    if (value == rleValue)
                    {
                        rleLength++;
                    }
                    else
                    {
                        if (rleLength >= 1 && rleLength < 4)
                        {
                            for (int i = 0; i < rleLength; ++i)
                            {
                                ret0.Add((Byte)rleValue);
                            }
                            rleValue = value;
                            rleLength = 1;
                        }
                        else if (rleLength >= 4 && rleLength < 0xFE)
                        {
                            ret0.Add(0x34);
                            ret0.Add((Byte)rleLength);
                            ret0.Add((Byte)rleValue);

                            rleValue = value;
                            rleLength = 1;
                        }
                        else if (rleLength >= 0xFE)
                        {
                            while (true)
                            {
                                ret0.Add(0x34);
                                ret0.Add((Byte)0xFE);
                                ret0.Add((Byte)rleValue);
                                rleLength -= 0xFE;
                                if (rleLength < 0xFE)
                                {
                                    ret0.Add(0x34);
                                    ret0.Add((Byte)rleLength);
                                    ret0.Add((Byte)rleValue);

                                    rleValue = value;
                                    rleLength = 1;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            ret0.Add(value);
                            rleValue = value;
                            rleLength = 1;
                        }
                    }

                    if (blockOffset + offset >= data.Count)
                    {
                        if (rleLength >= 4)
                        {
                            ret0.Add(0x34);
                            ret0.Add((Byte)rleLength);
                            ret0.Add((Byte)rleValue);
                        }
                        else
                        {
                            for (int i = 0; i < rleLength; ++i)
                            {
                                ret0.Add((Byte)rleValue);
                            }
                        }
                        ret0.Add(0x39);
                        isEnd = true;
                        break;
                    }
                }
                if (isEnd) break;
            }
        }
    }
}
