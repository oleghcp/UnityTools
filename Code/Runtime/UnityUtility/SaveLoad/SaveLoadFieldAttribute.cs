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
        public string Key { get; set; }
        internal FieldInfo Field { get; set; }
        internal object DefValue { get; }

        public SaveLoadFieldAttribute() { }

        public SaveLoadFieldAttribute(object defValue)
        {
            DefValue = defValue;
        }
    }
}
