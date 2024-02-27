using System;
using System.Collections;
using System.Collections.Generic;

namespace OlegHcp.Strings
{
    [Serializable]
    public abstract class StringComparer : IComparer, IComparer<string>
    {
        public int Compare(object x, object y)
        {
            return Compare((string)x, (string)y);
        }

        public abstract int Compare(string x, string y);
    }
}
