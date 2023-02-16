using System;
using System.IO;
using UnityUtility.CSharp;
using UnityUtility.Mathematics;
using UnityUtility.Tools;

namespace UnityUtility.IO
{
    public static class PathUtility
    {
        public static string GetName(string path)
        {
            return GetName(path, IsSeparator);
        }

        public static string GetName(string path, char separator)
        {
            return GetName(path, ch => ch == separator);
        }

        public static string GetParentPath(string path, int steps = 1)
        {
            return GetParentPath(path, steps, IsSeparator);
        }

        public static string GetParentPath(string path, char separator, int steps = 1)
        {
            return GetParentPath(path, steps, ch => ch == separator);
        }

        public static string SkipRootSteps(string path, int steps = 1)
        {
            return SkipRootSteps(path, steps, IsSeparator);
        }

        public static string SkipRootSteps(string path, char separator, int steps = 1)
        {
            return SkipRootSteps(path, steps, ch => ch == separator);
        }

        private static bool IsSeparator(char ch)
        {
            return ch == Path.DirectorySeparatorChar || ch == Path.AltDirectorySeparatorChar;
        }

        private static string GetName(string path, Predicate<char> separatorCheck)
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
            int length = separatorCheck(path.FromEnd(0)) ? path.Length - startIndex - 1
                                                         : path.Length - startIndex;
            return path.Substring(startIndex, length);
        }

        private static string GetParentPath(string path, int steps, Predicate<char> separatorCheck)
        {
            if (steps < 0)
                throw ThrowErrors.NegativeParameter(nameof(path));

            int maxLength = path.Length;

            for (int i = 0; i < steps; i++)
            {
                bool terminator = true;

                int j = maxLength - 1;
                while (j >= 0)
                {
                    if (separatorCheck(path[j]) && !terminator)
                        break;

                    terminator = false;
                    j--;
                }

                maxLength = j;
            }

            return path.Substring(0, maxLength.ClampMin(0));
        }

        private static string SkipRootSteps(string path, int steps, Predicate<char> separatorCheck)
        {
            if (steps < 0)
                throw ThrowErrors.NegativeParameter(nameof(path));

            int j = 0;
            for (int i = 0; i < steps; i++)
            {
                while (j < path.Length)
                {
                    if (separatorCheck(path[j++]))
                        break;
                }
            }

            return path.Substring(j);
        }
    }
}
