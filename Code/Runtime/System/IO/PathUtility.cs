using System.Runtime.CompilerServices;

namespace System.IO
{
    public static class PathUtility
    {
        public static string GetParentPath(string path, int steps = 1)
        {
            bool separatorChecker(char ch)
            {
                return ch == Path.DirectorySeparatorChar || ch == Path.AltDirectorySeparatorChar;
            }

            return f_getParentPath(path, steps, separatorChecker);
        }

        public static string GetParentPath(string path, char separator, int steps = 1)
        {
            return f_getParentPath(path, steps, ch => ch == separator);
        }

        // -- //

        private static string f_getParentPath(string path, int steps, Func<char, bool> separatorChecker)
        {
            string parent = path;
            int maxLength = path.Length;

            for (int i = 0; i < steps; i++)
            {
                bool terminator = true;
                for (int j = maxLength - 1; j >= 0; j--)
                {
                    if (separatorChecker(parent[j]))
                    {
                        if (!terminator)
                        {
                            maxLength = j;
                            break;
                        }
                    }
                    else { terminator = false; }
                }
            }

            return parent.Substring(0, maxLength);
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
