using System;
using System.Reflection;

namespace UnityUtility.SaveLoad
{
    /// <summary>
    /// Mark non-static fields which you want to save and load.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class SaveLoadFieldAttribute : Attribute
    {
        internal object DefValue;

        internal string Key;
        internal FieldInfo Field;

        public SaveLoadFieldAttribute(object defValue = null)
        {
            DefValue = defValue;
        }
    }
}
