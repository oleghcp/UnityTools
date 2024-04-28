using UnityEditor;
using UnityEngine;

namespace OlegHcpEditor
{
    public class DecoratorDrawer<TAttribute> : DecoratorDrawer where TAttribute : PropertyAttribute
    {
#pragma warning disable IDE1006
        public new TAttribute attribute => base.attribute as TAttribute;
#pragma warning restore IDE1006
    }
}
