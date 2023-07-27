using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace UnityUtility.Text
{
    public class StrCmpLogicalComparer : IComparer<string>, IComparer
    {
        [DllImport("Shlwapi.dll", CharSet = CharSet.Unicode)]
        private static extern int StrCmpLogicalW(string x, string y);

        public int Compare(string x, string y)
        {
            return StrCmpLogicalW(x, y);
        }

        public int Compare(object x, object y)
        {
            return StrCmpLogicalW((string)x, (string)y);
        }
    }
}
