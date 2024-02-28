using System;
using System.Reflection;

namespace OlegHcp.SaveLoad
{
    /// <summary>
    /// Mark non-static fields which you want to save and load.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public sealed class SaveLoadFieldAttribute : Attribute
    {
        internal FieldInfo Field { get; set; }
        internal object DefaultValue { get; }

        public string Key { get; set; }

        public SaveLoadFieldAttribute() { }

        public SaveLoadFieldAttribute(object defaultValue)
        {
            DefaultValue = defaultValue;
        }
    }
}
