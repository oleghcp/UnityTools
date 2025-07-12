using UnityEngine;

namespace OlegHcp.Strings
{
    public static class StringExtensions
    {
        public static string Dyed(this string self, in Color color)
        {
            return StringUtility.Dye(self, color);
        }
    }
}
