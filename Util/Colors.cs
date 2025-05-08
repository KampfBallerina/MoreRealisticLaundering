using UnityEngine;
using System.Collections.Generic;
namespace MoreRealisticLaundering.Util
{
    public static class ColorUtil
    {
        public static Dictionary<string, Color32> GetColors()
        {
            return new Dictionary<string, Color32>
        {
            { "Dark Blue", new Color32(33, 64, 115, byte.MaxValue) },
            { "Light Blue", new Color32(83, 126, 158, byte.MaxValue) },
            { "Light Purple", new Color32(87, 82, 153, byte.MaxValue / 2) },
            { "Purple", new Color32(108, 35, 114, byte.MaxValue) },
            { "Bright Green", new Color32(85, 198, 30, byte.MaxValue) },
            { "Olive Green", new Color32(67, 99, 33, byte.MaxValue) },
            { "Dark Green", new Color32(91, 127, 9, byte.MaxValue) },
            { "Cyan", new Color32(9, 115, 119, byte.MaxValue) },
            { "Dark Red", new Color32(99, 33, 37, byte.MaxValue) },
            { "Yellow", new Color32(208, 174, 54, byte.MaxValue) },
            { "Orange", new Color32(178, 78, 44, byte.MaxValue) },
            { "Grey", new Color32(49, 49, 49, byte.MaxValue) },
            { "Light Grey", new Color32(90, 90, 90, byte.MaxValue)},
            { "Ultra Light Grey", new Color32(150, 150, 150, byte.MaxValue) },
            { "Dark Grey", new Color32(30, 30, 30, byte.MaxValue) },
            { "Redpurple", new Color32(112, 21, 37, byte.MaxValue) },
            { "White", new Color32(255, 255, 255, byte.MaxValue) },
            { "Black", new Color32(0, 0, 0, byte.MaxValue) }
        };
        }


        public static Color32 GetColor(string colorName)
        {
            var colors = GetColors();
            if (colors.TryGetValue(colorName, out var color))
            {
                return color;
            }
            throw new KeyNotFoundException($"Color '{colorName}' not found.");
        }
    }
}