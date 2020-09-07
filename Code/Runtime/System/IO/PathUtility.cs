using System.Runtime.CompilerServices;
using UnityUtility.MathExt;
using UnityUtilityTools;

namespace System.IO
{
    public static class PathUtility
    {
        public static string GetName(string path)
        {
            return f_getName(path, f_isSeparator);
        }

        public static string GetName(string path, char separator)
        {
            return f_getName(path, ch => ch == separator);
        }

        public static string GetParentPath(string path, int steps = 1)
        {
            return f_getParentPath(path, steps, f_isSeparator);
        }

        public static string GetParentPath(string path, char separator, int steps = 1)
        {
            return f_getParentPath(path, steps, ch => ch == separator);
        }

        private static bool f_isSeparator(char ch)
        {
            return ch == Path.DirectorySeparatorChar || ch == Path.AltDirectorySeparatorChar;
        }

        private static string f_getName(string path, Predicate<char> separatorCheck)
        {
            int separatorIndex = -1;
            bool terminator = true;

            for (int i = path.Length - 1; i >= 0; i--)
            {
                if (separatorCheck(path[i]))
                {
                    if (terminator)
                        continue;

                    separatorIndex = i;
                    break;
                }
                else
                {
                    terminator = false;
                }
            }

            int startIndex = separatorIndex + 1;
            int length = separatorCheck(path.LastChar()) ?
                         path.Length - startIndex - 1 :
                         path.Length - startIndex;

            return path.Substring(startIndex, length);
        }

        private static string f_getParentPath(string path, int steps, Predicate<char> separatorCheck)
        {
            if (steps < 0)
                throw Errors.NegativeParameter(nameof(path));

            string parent = path;
            int maxLength = path.Length;

            for (int i = 0; i < steps; i++)
            {
                bool terminator = true;

                int j = maxLength - 1;
                while (j >= 0)
                {
                    if (separatorCheck(parent[j]) && !terminator)
                        break;

                    terminator = false;
                    j--;
                }

                maxLength = j;
            }

            return parent.Substring(0, maxLength.CutBefore(0));
        }
    }

    public static class IOExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetParentPath(this DirectoryInfo self, int steps = 1)
        {
            return PathUtility.GetParentPath(self.FullName, steps);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetParentPath(this FileInfo self, int steps = 1)
        {
            return PathUtility.GetParentPath(self.FullName, steps);
        }
    }
}
