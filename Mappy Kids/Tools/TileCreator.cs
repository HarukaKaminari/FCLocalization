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
                    int c = img.GetPixel(x * 8 + _x, y * 8 + _y).ToArgb();
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
                        if(c == VALID_COLORS[idx])
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
                        plane[i] |= (Byte)(((idx >> i) & 1) << x);
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
    }
}
