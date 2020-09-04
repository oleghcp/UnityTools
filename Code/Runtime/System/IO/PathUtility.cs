using System.Runtime.CompilerServices;
using UnityUtility.MathExt;
using UnityUtilityTools;

namespace System.IO
{
    public static class PathUtility
    {
        public static string GetName(string path)
        {
            int separatorIndex = -1;
            bool terminator = true;

            for (int i = path.Length - 1; i >= 0; i--)
            {
                if (f_isSeparator(path[i]))
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
            int length = f_isSeparator(path[path.Length - 1]) ?
                         path.Length - startIndex - 1 :
                         path.Length - startIndex;

            return path.Substring(startIndex, length);
        }

        public static string GetParentPath(string path, int steps = 1)
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
                    if (f_isSeparator(parent[j]) && !terminator)
                        break;

                    terminator = false;
                    j--;
                }

                maxLength = j;
            }

            return parent.Substring(0, maxLength.CutBefore(0));
        }

        private static bool f_isSeparator(char ch)
        {
            return ch == Path.DirectorySeparatorChar || ch == Path.AltDirectorySeparatorChar;
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
