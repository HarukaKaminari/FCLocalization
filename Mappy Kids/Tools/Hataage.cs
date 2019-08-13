using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslationOrganizer
{
    class Hataage
    {
        private static char[] jpChars = new char[] { 'あ', 'か', 'し', 'ろ', '下', '上', 'げ', 'て', 'な', 'い', 'で', 'る', };
        private static char[] cnChars = new char[] { '红', '旗', '白', '布', '下', '起', '放', '要', '不', '要', '且', '举', };
        public static int[] indices = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 9, 10, 8, 11, };

        private static string[] cnTexts = new string[] {
            "红旗要放下",
            "白布要放下",
            "红旗要举起",
            "白布要举起",

            "红旗不要放下",
            "白布不要放下",
            "红旗不要举起",
            "白布不要举起",

            "红旗不要放下且",
            "白布不要放下且",
            "红旗不要举起且",
            "白布不要举起且",
        };
        private static Byte[] opCodes = new Byte[] { 0x00, 0x80, 0x04, 0x84, 0x04, 0x84, 0x00, 0x80, 0x04, 0x84, 0x00, 0x80,};

        private static int startAddr = 0x878F;
        private static int hataageBank = 0xA;

        public static void Start(string ROMFileName)
        {
            Byte[] prgData = Common.GetPRGData(ROMFileName);
            Byte[] chrData = Common.GetCHRData(ROMFileName);

            int absoluteAddr = TableOrganizer.GetAbsoluteAddress(startAddr, hataageBank);

            int curText = 0;
            int offset = 0;
            while (curText < cnTexts.Length)
            {
                prgData[absoluteAddr + offset] = opCodes[curText];
                offset++;

                char[] chars = cnTexts[curText].ToCharArray();
                for (int i = 0; i < chars.Length; ++i)
                {
                    prgData[absoluteAddr + offset] = findChar(chars[i]);
                    offset++;
                }

                prgData[absoluteAddr + offset] = 0xCC;
                offset++;
                prgData[absoluteAddr + offset] = 0xFF;
                offset++;

                curText++;
            }

            Byte[] data = TileCreator.CreateGiantFont(cnChars);
            for(int i = 0; i < data.Length; ++i)
            {
                chrData[0x10000 + i] = data[i];
            }

            Common.PatchROM(ROMFileName, prgData, chrData);
        }

        private static Byte findChar(char c)
        {
            for(int i = 0; i < cnChars.Length; ++i)
            {
                if (cnChars[i] == c)
                    return (Byte)i;
            }
            return 0xFF;
        }
    }
}
