using OlegHcp.Engine;
using UnityEngine;

namespace OlegHcp.Strings
{
    public static class StringUtility
    {
        public static readonly string Space = " ";
        public static readonly string Tab = "    ";
        public static readonly string Alphanumeric = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        public static string Colorize(string self, in Color color)
        {
            return $"<color={color.ToStringHexRGB()}>{self}</color>";
        }
    }
}
