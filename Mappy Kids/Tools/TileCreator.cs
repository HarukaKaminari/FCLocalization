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
        public static readonly Color COLOR0 = Color.Black;
        public static readonly Color COLOR1 = Color.White;
        public static readonly Color COLOR2 = Color.Red;
        public static readonly Color COLOR3 = Color.Green;
        public static readonly Color[] VALID_COLORS = { COLOR0, COLOR1, COLOR2, COLOR3 };

        private static Dictionary<char, Bitmap> asciiImgs = null;

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
                    Color[] _pixels = Bitmap2Tile(img, x, y);
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
        /// <returns>像素数组。这个数组必定是拥有64个元素的Color数组，每个元素代表一个像素的颜色</returns>
        private static Color[] Bitmap2Tile(Bitmap img, int x, int y)
        {
            List<Color> pixels = new List<Color>();
            for(int _y = 0; _y < 8; ++_y)
            {
                for(int _x = 0; _x < 8; ++_x)
                {
                    Color c = img.GetPixel(x * 8 + _x, y * 8 + _y);
                    pixels.Add(c);
                }
            }
            return pixels.ToArray();
        }
        /// <summary>
        /// 将tile的所有像素转成tile数据
        /// </summary>
        /// <param name="pixels">所有的像素颜色。这是一个拥有64个元素的Color数组，每个元素即为像素的颜色</param>
        /// <returns>tile数据</returns>
        private static Byte[] Tile2Data(Color[] pixels)
        {
            List<Byte> data0 = new List<Byte>();
            List<Byte> data1 = new List<Byte>();
            for (int y = 0; y < 8; ++y)
            {
                Byte[] plane = new Byte[2];
                plane[0] = plane[1] = 0;
                for(int x = 0; x < 8; ++x)
                {
                    Color c = pixels[x + y * 8];
                    // 查表获得颜色索引
                    int idx = 0;
                    for(idx = 0; idx < VALID_COLORS.Length; ++idx)
                    {
                        if(c.ToArgb() == VALID_COLORS[idx].ToArgb())
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

        private static void CreateASCIIImgs()
        {
            if(asciiImgs == null)
            {
                char[] chars = new char[] {
                    '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
                    'A', 'B', 'C', 'D', 'E', 'F', 'G',
                    'H','I','J','K','L','M','N',
                    'O','P','Q','R','S','T',
                    'U','V','W','X','Y','Z',
                };
                int[] x = new int[] {
                    0,1,2,3,4,5,6,7,8,9,
                    1,2,3,4,5,6,7,
                    8,9,10,11,12,13,14,
                    15,0,1,2,3,4,
                    5,6,7,8,9,10,
                };
                int[] y = new int[] {
                    0,0,0,0,0,0,0,0,0,0,
                    4,4,4,4,4,4,4,
                    4,4,4,4,4,4,4,
                    4,5,5,5,5,5,
                    5,5,5,5,5,5,
                };
                asciiImgs = new Dictionary<char, Bitmap>();
                for(int i = 0; i < chars.Length; ++i)
                {
                    Bitmap img = new Bitmap(8, 8, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                    Graphics g = Graphics.FromImage(img);
                    g.Clear(COLOR0);
                    g.DrawImage(Properties.Resources.ASCII, new Rectangle(0, 0, 8, 8), new Rectangle(x[i] * 8, (y[i] + 8) * 8, 8, 8), GraphicsUnit.Pixel);
                    asciiImgs.Add(chars[i], img);
                }
            }
        }

        public static void GeneratePatternTable(ref Byte[] chrData, int bankNo, string uniqueChars, string uniqueASCII)
        {
            CreateASCIIImgs();
            // 生成一张图
            Bitmap canvas = new Bitmap(128, 64, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            Graphics g = Graphics.FromImage(canvas);
            Bitmap charImg = new Bitmap(16, 16, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            Graphics charG = Graphics.FromImage(charImg);
            charG.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            Font font = new Font("宋体", 9);
            Brush brush = new SolidBrush(COLOR3);
            int idx = 0;
            foreach (char c in uniqueChars)
            {
                int x = ((TranslationOrganizer.s_CharOffsets[idx] - 0x60) & 3) * 32;
                int y = ((TranslationOrganizer.s_CharOffsets[idx] - 0x60) >> 2) * 8;
                charG.Clear(COLOR0);
                charG.DrawString(c.ToString(), font, brush, new Point(0, 4));
                g.DrawImage(charImg, new Rectangle(x + 0, y, 8, 8), new Rectangle(0, 0, 8, 8), GraphicsUnit.Pixel);
                g.DrawImage(charImg, new Rectangle(x + 8, y, 8, 8), new Rectangle(0, 8, 8, 8), GraphicsUnit.Pixel);
                g.DrawImage(charImg, new Rectangle(x + 16, y, 8, 8), new Rectangle(8, 0, 8, 8), GraphicsUnit.Pixel);
                g.DrawImage(charImg, new Rectangle(x + 24, y, 8, 8), new Rectangle(8, 8, 8, 8), GraphicsUnit.Pixel);
                idx++;
            }
            idx = 0;
            foreach(char c in uniqueASCII)
            {
                int x = ((TranslationOrganizer.s_ASCIIOffsets[idx] - 0x80) & 15) * 8;
                int y = ((TranslationOrganizer.s_ASCIIOffsets[idx] - 0x80) >> 4) * 8;
                if (asciiImgs.ContainsKey(c))
                {
                    g.DrawImage(asciiImgs[c], new Rectangle(x, y, 8, 8), new Rectangle(0, 0, 8, 8), GraphicsUnit.Pixel);
                }
                idx++;
            }
            canvas.Save(bankNo + ".png", System.Drawing.Imaging.ImageFormat.Png);
            // 生成PatternTable
            Byte[] data = Bitmap2Data(canvas);
            // 写入CHR Bank
            for (int i = 0; i < data.Length; ++i)
            {
                chrData[0x20000 + (bankNo - 1) * 2048 + i] = data[i];
            }
        }
        
        public static Byte[] CreateGiantFont(char[] cnChars)
        {
            Bitmap img = new Bitmap(128, 32, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            Graphics g = Graphics.FromImage(img);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixel;
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            g.Clear(COLOR0);
            Font font = new Font(new FontFamily("楷体"), 13, FontStyle.Bold);
            for(int i = 0; i < cnChars.Length; ++i)
            {
                Brush brush = new SolidBrush(i < 2 ? COLOR2 : COLOR3);
                if (cnChars[i].Equals('布'))
                    cnChars[i] = '旗';
                g.DrawString(cnChars[i].ToString(), font, brush, new Point((Hataage.indices[i] & 7) * 16 - 4, (Hataage.indices[i] >> 3) * 16 - 1));
            }
            //img.Save("GiantText.png", System.Drawing.Imaging.ImageFormat.Png);
            return Bitmap2Data(img);
        }
    }
}
