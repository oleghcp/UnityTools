#if UNITY_EDITOR
using System;
using UnityEngine;

namespace UnityUtility.Inspector
{
    [AttributeUsage(AttributeTargets.Field)]
    public class FolderRequiredAttribute : PropertyAttribute { }
}
#endif
