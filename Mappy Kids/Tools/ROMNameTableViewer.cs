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
        private static readonly int[] ADDRESSES = { 0x18000, 0x18400 };
        // Tile图
        private static Bitmap s_ImgTile;
        // Tile表
        private static Dictionary<Byte, Bitmap> s_TileArray;
        // 新Nametable
        private static Byte[] s_NewNameTable;
        // PatternTable所在位置
        private static readonly int[] PAT_ADDR = { 0x20000, 0x20400 };

        public static void Start(string ROMFileName, string TileFileName)
        {
            Byte[] chrData = Common.GetCHRData(ROMFileName);
            s_ImgTile = new Bitmap(TileFileName);
            Tile2Array();
            GetImages(chrData);
            Patch(ROMFileName, "", true);
            Patch(ROMFileName, "", false);
        }

        /// <summary>
        /// 将新的剧情图tile写入ROM，并且刷新对应的Nametable
        /// </summary>
        /// <param name="ROMFileName">ROM文件名</param>
        /// <param name="imgFileName">剧情图文件名</param>
        /// <param name="is1P">1P还是2P？</param>
        private static void Patch(string ROMFileName, string imgFileName, bool is1P)
        {
            Byte[] chrData = Common.GetCHRData(ROMFileName);
            bool isSucceeded = GenerateTilesAndNameTableFromImage(imgFileName);
            if (isSucceeded)
            {
                // 刷新NameTable
                for(int i = 0; i < 256 / 8 * 240 / 8; ++i)
                {
                    chrData[ADDRESSES[is1P ? 0 : 1] + i] = s_NewNameTable[i];
                }
                // 创建新tileImg
                Bitmap newTileImg = CreateNewTileImage();
                // 刷新PatternTable
                Byte[] patternTable = TileCreator.Bitmap2Data(newTileImg);
                for(int i = 0; i < patternTable.Length; ++i)
                {
                    chrData[PAT_ADDR[is1P ? 0 : 1] + i] = patternTable[i];
                }
            }
            else
            {
                Console.WriteLine("Fatal ERROR! Too many tiles!");
                return;
            }
            Common.PatchROM(ROMFileName, null, chrData);
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

        /// <summary>
        /// 获得所有ROMNametable图像
        /// </summary>
        /// <param name="chrData"></param>
        /// <returns></returns>
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

        private static bool GenerateTilesAndNameTableFromImage(string imgFileName)
        {
            // 先删除Tile列表中0x80~0xFF的Tile
            for(Byte i = 0x80; i <= 0xFF; ++i)
            {
                if(s_TileArray.ContainsKey(i))
                    s_TileArray.Remove(i);
            }
            // 新Nametable
            s_NewNameTable = new Byte[256 / 8 * 240 / 8];
            // 取剧情图
            Bitmap img = new Bitmap(imgFileName);
            // 分割
            for(int y = 0; y < 240 / 8; ++y)
            {
                for(int x = 0; x < 256 / 8; ++x)
                {
                    Bitmap tile = new Bitmap(8, 8, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                    Graphics g = Graphics.FromImage(tile);
                    g.DrawImage(img, new Rectangle(0, 0, 8, 8), new Rectangle(x * 8, y * 8, 8, 8), GraphicsUnit.Pixel);
                    // 将该tile添加到Tile列表中。如果已经存在相同的tile则不再重复添加
                    int idx = GetIndexFromTheTileArray(tile);
                    if(idx < 256)
                    {
                        // 新tile
                        if(idx == s_TileArray.Count)
                            s_TileArray.Add((Byte)idx, tile);
                        // nametable
                        s_NewNameTable[x + y * 256 / 8] = (Byte)idx;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        
        /// <summary>
        /// 获得该tile在tile列表中的索引。如果这个tile不在tile列表中，则返回tile列表的长度，代表需要添加到这个位置
        /// 注意，如果返回值>256，则说明超出了tile列表上限
        /// </summary>
        /// <param name="tile">待检测的tile</param>
        /// <returns>索引</returns>
        private static int GetIndexFromTheTileArray(Bitmap tile)
        {
            foreach(KeyValuePair<Byte, Bitmap> kv in s_TileArray)
            {
                for (int y = 0; y < 8; ++y)
                {
                    for (int x = 0; x < 8; ++x)
                    {
                        Color a = kv.Value.GetPixel(x, y);
                        Color b = tile.GetPixel(x, y);
                        if (a.ToArgb() == b.ToArgb())
                        {
                            return kv.Key;
                        }
                    }
                }
            }
            return s_TileArray.Count;
        }

        private static Bitmap CreateNewTileImage()
        {
            Bitmap ret = new Bitmap(128, 64, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            Graphics g = Graphics.FromImage(ret);
            g.Clear(Color.FromArgb(TileCreator.COLOR0));
            for(int i = 0x80; i < 0x100; ++i)
            {
                if (s_TileArray.ContainsKey((Byte)i))
                {
                    g.DrawImage(s_TileArray[(Byte)i], new Rectangle((i & 15) * 8, (i - 0x80) >> 4, 8, 8), new Rectangle(0, 0, 8, 8), GraphicsUnit.Pixel);
                }
            }
            return ret;
        }
    }
}
