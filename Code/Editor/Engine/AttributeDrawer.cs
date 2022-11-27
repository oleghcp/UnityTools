using UnityEditor;
using UnityEngine;

namespace UnityUtilityEditor.Engine
{
    public abstract class AttributeDrawer<TAttribute> : PropertyDrawer where TAttribute : PropertyAttribute
    {
#pragma warning disable IDE1006
        public new TAttribute attribute => base.attribute as TAttribute;
#pragma warning restore IDE1006
    }
}
