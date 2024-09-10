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
                default: //8+
                    texture.AddToTexture(Numbers.black8, new Color32(100, 100, 100, 255), offsetx, offsety);
                    texture.AddToTexture(Numbers.blue8, new Color32(156, 196, 233, 255), offsetx, offsety);
                    break;
            }
        }
        
        public static void AddBigNumberToTexture(this Texture2D texture, int number)
        {
            int offsety = 0;
            int offsetx = 0;
            int[,] map;
            switch (number)
            {                   
                case 1:
                    map = NumbersBig.colors1;
                    break;
                case 2:
                    map = NumbersBig.colors2;
                    break;
                case 3:
                    map = NumbersBig.colors3;
                    break;
                case 4:
                    map = NumbersBig.colors4;
                    break;
                case 5:
                    map = NumbersBig.colors5;
                    break;
                case 6:
                    map = NumbersBig.colors6;
                    break;
                case 7:
                    map = NumbersBig.colors7;
                    break;
                case 8:
                    map = NumbersBig.colors8;
                    break;
                //if custom modes are possible and modded, I will still only draw an 8
                //if you are a mod autor, find this line and require more then 8 to be drawn,
                //AND have time to write an Issue, I might find the time to update this code.
                default: 
                    map = NumbersBig.colors8;
                    break;
            }
            for (int x = 0; x < map.GetLength(0); x++)
            {
                for (int y = 0; y < map.GetLength(1); y++)
                {
                    //god do I hate Y Axis inversions
                    int colorValue = map[map.GetLength(1) - 1 - y, x];
                    Color32 color = NumbersBig.colors[colorValue];
                    if (color.a != 0)
                    {
                        texture.SetPixel(x + offsetx, y + offsety, color);
                    }
                }
            }            
        }
    }
}
