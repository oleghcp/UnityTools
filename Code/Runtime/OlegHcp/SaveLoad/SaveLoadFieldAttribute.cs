using System;

namespace OlegHcp.SaveLoad
{
    /// <summary>
    /// Mark non-static fields which you want to save and load.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class SaveLoadFieldAttribute : Attribute
    {
        internal string Key { get; set; }

        public SaveLoadFieldAttribute()
        {

        }

        public SaveLoadFieldAttribute(string key)
        {
            Key = key;
        }
    }
}
