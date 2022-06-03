using UnityEngine;

namespace UnityEditor
{
    public abstract class AttributeDrawer<TAttribute> : PropertyDrawer where TAttribute : PropertyAttribute
    {
        public new TAttribute attribute => base.attribute as TAttribute;
    }
}
