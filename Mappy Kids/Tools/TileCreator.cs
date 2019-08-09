using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace TranslationOrganizer
{
    class TileCreator
    {
        public static readonly int COLOR0 = 0x000000;
        public static readonly int COLOR1 = 0xFFFFFF;
        public static readonly int COLOR2 = 0xFF0000;
        public static readonly int COLOR3 = 0x00FF00;
        public static readonly int[] VALID_COLORS = { COLOR0, COLOR1, COLOR2, COLOR3 };

        public static Byte[] Bitmap2Data(Bitmap img)
        {
            // 图片宽高必须为8的倍数
            if(!isMultipleOf8(img.Width) || !isMultipleOf8(img.Height))
            {
                return null;
            }

            List<Byte> data = new List<Byte>();
            for(int y = 0; y < img.Height / 8; ++y)
            {
                for(int x = 0; x < img.Width / 8; ++x)
                {
                    int[] _pixels = Bitmap2Tile(img, x, y);
                    Byte[] _data = Tile2Data(_pixels);
                    data.AddRange(_data);
                }
            }
            return data.ToArray();
        }

        /// <summary>
        /// 取图片中指定坐标处的tile，将其转成像素数组
        /// </summary>
        /// <param name="img">图片。宽高必须为8的倍数</param>
        /// <param name="x">x坐标。单位是tile</param>
        /// <param name="y">y坐标。单位是tile</param>
        /// <returns>像素数组。这个数组必定是拥有64个元素的int数组，每个元素代表一个像素的ARGB颜色值</returns>
        private static int[] Bitmap2Tile(Bitmap img, int x, int y)
        {
            List<int> pixels = new List<int>();
            for(int _y = 0; _y < 8; ++_y)
            {
                for(int _x = 0; _x < 8; ++_x)
                {
                    int c = img.GetPixel(x * 8 + _x, y * 8 + _y).ToArgb() & 0xFFFFFF;
                    pixels.Add(c);
                }
            }
            return pixels.ToArray();
        }
        /// <summary>
        /// 将tile的所有像素转成tile数据
        /// </summary>
        /// <param name="pixels">所有的像素颜色。这是一个拥有64个元素的int数组，每个元素即为像素的ARGB颜色值</param>
        /// <returns>tile数据</returns>
        private static Byte[] Tile2Data(int[] pixels)
        {
            List<Byte> data0 = new List<Byte>();
            List<Byte> data1 = new List<Byte>();
            for (int y = 0; y < 8; ++y)
            {
                Byte[] plane = new Byte[2];
                plane[0] = plane[1] = 0;
                for(int x = 0; x < 8; ++x)
                {
                    int c = pixels[x + y * 8];
                    // 查表获得颜色索引
                    int idx = 0;
                    for(idx = 0; idx < VALID_COLORS.Length; ++idx)
                    {
                        if(c == (VALID_COLORS[idx] & 0xFFFFFF))
                        {
                            break;
                        }
                    }
                    // 如果是表中不存在的颜色，强制认为是背景色
                    if (idx >= 4)
                        idx = 0;
                    // 将颜色索引分解成两个bit，然后分别存入两个plane
                    for(int i = 0; i < 2; ++i)
                    {
                        plane[i] |= (Byte)(((idx >> i) & 1) << (7 - x));
                    }
                }
                data0.Add(plane[0]);
                data1.Add(plane[1]);
            }
            // 合并两个plane
            data0.AddRange(data1);
            return data0.ToArray();
        }

        /// <summary>
        /// 检查指定数值是否是8的倍数
        /// </summary>
        /// <param name="value">给定数值</param>
        /// <returns>是否是8的倍数</returns>
        private static bool isMultipleOf8(int value)
        {
            return value / 8 * 8 == value;
        }

        public static void Test()
        {
            string ROMFileName = "D:/Mesen-0.9.8/bin/x64/Release/Roms/Mappy Kids (J).nes";
            // 生成一张图
            Bitmap canvas = new Bitmap(128, 64, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            Graphics g = Graphics.FromImage(canvas);
            Bitmap charImg = new Bitmap(16, 16, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            Graphics charG = Graphics.FromImage(charImg);
            charG.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            string str = "你很努力嘛！想要什么礼物吗？";
            Font font = new Font("宋体", 9);
            Brush brush = new SolidBrush(Color.FromArgb(255, 255, 255));
            int x = 0, y = 0;
            foreach(char c in str)
            {
                charG.Clear(Color.FromArgb(0, 0, 0));
                charG.DrawString(c.ToString(), font, brush, new Point(0, 0));
                g.DrawImage(charImg, new Rectangle(x + 0, y + 0, 8, 8), new Rectangle(0, 0, 8, 8), GraphicsUnit.Pixel);
                g.DrawImage(charImg, new Rectangle(x + 8, y + 0, 8, 8), new Rectangle(0, 8, 8, 8), GraphicsUnit.Pixel);
                g.DrawImage(charImg, new Rectangle(x + 16, y + 0, 8, 8), new Rectangle(8, 0, 8, 8), GraphicsUnit.Pixel);
                g.DrawImage(charImg, new Rectangle(x + 24, y + 0, 8, 8), new Rectangle(8, 8, 8, 8), GraphicsUnit.Pixel);
                x += 32;
                if(x >= 128)
                {
                    x = 0;
                    y += 8;
                }
            }
            // 生成PatternTable
            Byte[] data = Bitmap2Data(canvas);
            // 写入CHR Bank $80
            Byte[] chrData = Common.GetCHRData(ROMFileName);
            for(int i = 0; i < data.Length; ++i)
            {
                chrData[0x20000 + i] = data[i];
            }
            // 修改文本
            Byte[] prgData = Common.GetPRGData(ROMFileName);
            int addr = TableOrganizer.GetAbsoluteAddress(0xB5A5);
            prgData[addr + 0] = 0xF1;
            prgData[addr + 1] = 0x20;
            prgData[addr + 2] = 0x60;
            prgData[addr + 3] = 0x61;
            prgData[addr + 4] = 0x62;
            prgData[addr + 5] = 0x63;
            prgData[addr + 6] = 0x64;
            prgData[addr + 7] = 0x65;
            prgData[addr + 8] = 0xF0;
            prgData[addr + 9] = 0x66;
            prgData[addr + 10] = 0x67;
            prgData[addr + 11] = 0x68;
            prgData[addr + 12] = 0x69;
            prgData[addr + 13] = 0x6A;
            prgData[addr + 14] = 0x6B;
            prgData[addr + 15] = 0x6C;
            prgData[addr + 16] = 0x6D;
            prgData[addr + 17] = 0xF2;
            prgData[addr + 18] = 0xF1;
            // 给ROM打补丁
            Common.PatchROM(ROMFileName, prgData, chrData);
        }
    }
}
