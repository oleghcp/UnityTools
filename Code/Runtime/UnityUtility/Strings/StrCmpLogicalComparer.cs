#if  UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
using System;
using System.Runtime.InteropServices;

namespace UnityUtility.Strings
{
    [Serializable]
    public class StrCmpLogicalComparer : StringComparer
    {
        [DllImport("Shlwapi.dll", CharSet = CharSet.Unicode)]
        private static extern int StrCmpLogicalW(string x, string y);

        public override int Compare(string x, string y)
        {
            return StrCmpLogicalW(x, y);
        }
    }
}
#endif
