using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using EclipseLevelInCharacterSelection.assets;

namespace EclipseLevelInCharacterSelection
{
    internal static class BitMaps
    {
        public static void AddToTexture(this Texture2D texture, int[,] map, Color32 color, int offsetx, int offsety)
        {
            for (int x = 0; x < map.GetLength(0); x++)
            {
                for (int y = 0; y < map.GetLength(1); y++)
                {
                    if (map[map.GetLength(1) - 1 - y, x] == 1)
                    {
                        texture.SetPixel(x + offsetx, y + offsety, color);
                    }
                }
            }
        }
        
        public static void AddToTexture(this Texture2D texture, (byte, byte, byte, byte)[,] map, int offsetx, int offsety)
        {
            for (int x = 0; x < map.GetLength(0); x++)
            {
                for (int y = 0; y < map.GetLength(1); y++)
                {
                    (byte, byte, byte, byte) colorValues = map[map.GetLength(1) - 1 - y, x];
                    Color32 color = new Color32(colorValues.Item1, colorValues.Item2, colorValues.Item3, colorValues.Item4);
                    texture.SetPixel(x + offsetx, y + offsety, color);
                }
            }
        }

        public static void AddNumberToTexture(this Texture2D texture, int number)
        {
            int offsety = 0;
            int offsetx = 0;
            switch (number)
            {
                case 1:
                    texture.AddToTexture(Numbers.black1, new Color32(100, 100, 100, 255), offsetx, offsety);
                    texture.AddToTexture(Numbers.blue1, new Color32(156, 196, 233, 255), offsetx, offsety);
                    break;
                case 2:
                    texture.AddToTexture(Numbers.black2, new Color32(100, 100, 100, 255), offsetx, offsety);
                    texture.AddToTexture(Numbers.blue2, new Color32(156, 196, 233, 255), offsetx, offsety);
                    break;
                case 3:
                    texture.AddToTexture(Numbers.black3, new Color32(100, 100, 100, 255), offsetx, offsety);
                    texture.AddToTexture(Numbers.blue3, new Color32(156, 196, 233, 255), offsetx, offsety);
                    break;
                case 4:
                    texture.AddToTexture(Numbers.black4, new Color32(100, 100, 100, 255), offsetx, offsety);
                    texture.AddToTexture(Numbers.blue4, new Color32(156, 196, 233, 255), offsetx, offsety);
                    break;
                case 5:
                    texture.AddToTexture(Numbers.black5, new Color32(100, 100, 100, 255), offsetx, offsety);
                    texture.AddToTexture(Numbers.blue5, new Color32(156, 196, 233, 255), offsetx, offsety);
                    break;
                case 6:
                    texture.AddToTexture(Numbers.black6, new Color32(100, 100, 100, 255), offsetx, offsety);
                    texture.AddToTexture(Numbers.blue6, new Color32(156, 196, 233, 255), offsetx, offsety);
                    break;
                case 7:
                    texture.AddToTexture(Numbers.black7, new Color32(100, 100, 100, 255), offsetx, offsety);
                    texture.AddToTexture(Numbers.blue7, new Color32(156, 196, 233, 255), offsetx, offsety);
                    break;
                case 8:
                    texture.AddToTexture(Numbers.black8, new Color32(100, 100, 100, 255), offsetx, offsety);
                    texture.AddToTexture(Numbers.blue8, new Color32(156, 196, 233, 255), offsetx, offsety);
                    break;
            }
        }
        
        public static void AddBigNumberToTexture(this Texture2D texture, int number)
        {
            int offsety = 0;
            int offsetx = 0;
            switch (number)
            {                   
                case 1:
                    //should be default, but i like it this way
                    texture.AddToTexture(NumbersBig.colors1, offsetx, offsety);
                    break;
                case 2:
                    texture.AddToTexture(NumbersBig.colors2, offsetx, offsety);
                    break;
                case 3:
                    texture.AddToTexture(NumbersBig.colors3, offsetx, offsety);
                    break;
                case 4:
                    texture.AddToTexture(NumbersBig.colors4, offsetx, offsety);
                    break;
                case 5:
                    texture.AddToTexture(NumbersBig.colors5, offsetx, offsety);
                    break;
                case 6:
                    texture.AddToTexture(NumbersBig.colors6, offsetx, offsety);
                    break;
                case 7:
                    texture.AddToTexture(NumbersBig.colors7, offsetx, offsety);
                    break;
                case 8:
                    texture.AddToTexture(NumbersBig.colors8, offsetx, offsety);
                    break;
            }
        }
    }
}
