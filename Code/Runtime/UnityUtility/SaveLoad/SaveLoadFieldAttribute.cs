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
        private string m_defValString;
        internal Bytes DefValSimple;

        internal string Key;
        internal FieldInfo Field;

        internal string DefValString
        {
            get { return m_defValString ?? string.Empty; }
        }

        public SaveLoadFieldAttribute() { }

        public SaveLoadFieldAttribute(string defValue)
        {
            m_defValString = defValue;
        }

        public SaveLoadFieldAttribute(int defValue)
        {
            DefValSimple = defValue;
        }

        public SaveLoadFieldAttribute(float defValue)
        {
            DefValSimple = defValue;
        }

        public SaveLoadFieldAttribute(bool defValue)
        {
            DefValSimple = defValue;
        }
    }
}
