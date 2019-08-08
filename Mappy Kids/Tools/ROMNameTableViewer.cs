using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace TranslationOrganizer
{
    class ROMNameTableViewer
    {

        // 所有ROMNameTable首地址
        private static readonly int[] ADDRESSES = { 0xA800, 0xF800, 0xFC00, 0x18000, 0x18400, 0x1F000 };
        // Tile图
        private static Bitmap s_ImgTile;
        // Tile表
        private static Dictionary<Byte, Bitmap> s_TileArray;

        public static void Start(string ROMFileName, string TileFileName)
        {
            Byte[] chrData = Common.GetCHRData(ROMFileName);
            s_ImgTile = new Bitmap(TileFileName);
            Tile2Array();
            GetImages(chrData);
        }

        /// <summary>
        /// 将Tile图分解成Tile列表
        /// </summary>
        private static void Tile2Array()
        {
            s_TileArray = new Dictionary<byte, Bitmap>();
            for(int y = 0; y < s_ImgTile.Height / 8; ++y)
            {
                for(int x = 0; x < s_ImgTile.Width / 8; ++x)
                {
                    Bitmap tile = new Bitmap(8, 8, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                    Graphics g = Graphics.FromImage(tile);
                    g.Clear(Color.Transparent);
                    g.DrawImage(s_ImgTile, new Rectangle(0, 0, 8, 8), new Rectangle(x * 8, y * 8, 8, 8), GraphicsUnit.Pixel);
                    s_TileArray.Add((Byte)(x + y * s_ImgTile.Width / 8), tile);
                }
            }
        }


        private static Dictionary<int, Bitmap> GetImages(Byte[] chrData)
        {
            Dictionary<int, Bitmap> imgs = new Dictionary<int, Bitmap>();
            foreach(int addr in ADDRESSES)
            {
                Bitmap img = new Bitmap(256, 240, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                Graphics g = Graphics.FromImage(img);
                g.Clear(Color.Transparent);
                for(int y = 0; y < 240 / 8; ++y)
                {
                    for(int x = 0; x < 256 / 8; ++x)
                    {
                        int idx = x + y * 256 / 8;
                        Byte data = chrData[addr + idx];
                        if (s_TileArray.ContainsKey(data))
                        {
                            g.DrawImage(s_TileArray[data], new Rectangle(x * 8, y * 8, 8, 8), new Rectangle(0, 0, 8, 8), GraphicsUnit.Pixel);
                        }
                    }
                }
                //img.Save(string.Format("{0:X4}.png", addr), System.Drawing.Imaging.ImageFormat.Png);
                imgs.Add(addr, img);
            }
            return imgs;
        }
    }
}
