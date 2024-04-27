using UnityEngine;

namespace OlegHcp
{
    public enum ColorCode
    {
        Yellow,
        Grey,
        Magenta,
        Cyan,
        Red,
        Black,
        White,
        Blue,
        Sky,
        Lime,
        Green,
        Maroon,
        Cherry,
        Olive,
        Navy,
        Teal,
        Purple,
        Silver,
        Orange,
        Violet,
        Random,
    }

    public static class ColorCodeExtensions
    {
        public static Color ToColor(this ColorCode self)
        {
            switch (self)
            {
                case ColorCode.Yellow: return Colours.Yellow;
                case ColorCode.Grey: return Colours.Grey;
                case ColorCode.Magenta: return Colours.Magenta;
                case ColorCode.Cyan: return Colours.Cyan;
                case ColorCode.Red: return Colours.Red;
                case ColorCode.Black: return Colours.Black;
                case ColorCode.White: return Colours.White;
                case ColorCode.Blue: return Colours.Blue;
                case ColorCode.Sky: return Colours.Sky;
                case ColorCode.Lime: return Colours.Lime;
                case ColorCode.Green: return Colours.Green;
                case ColorCode.Maroon: return Colours.Maroon;
                case ColorCode.Cherry: return Colours.Cherry;
                case ColorCode.Olive: return Colours.Olive;
                case ColorCode.Navy: return Colours.Navy;
                case ColorCode.Teal: return Colours.Teal;
                case ColorCode.Purple: return Colours.Purple;
                case ColorCode.Silver: return Colours.Silver;
                case ColorCode.Orange: return Colours.Orange;
                case ColorCode.Violet: return Colours.Violet;
                case ColorCode.Random: return Colours.Random;
                default: throw new UnsupportedValueException(self);
            }
        }
    }
}
