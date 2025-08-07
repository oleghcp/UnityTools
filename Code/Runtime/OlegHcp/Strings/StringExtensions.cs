using UnityEngine;

namespace OlegHcp.Strings
{
    public static class StringExtensions
    {
        public static string Colored(this string self, in Color color)
        {
            return StringUtility.Colorize(self, color);
        }
    }
}
