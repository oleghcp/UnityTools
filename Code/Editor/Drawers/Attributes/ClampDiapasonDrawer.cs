using System;
using OlegHcp.Inspector;
using OlegHcp.NumericEntities;
using UnityEditor;
using UnityEngine;

namespace OlegHcpEditor.Drawers.Attributes
{
    [CustomPropertyDrawer(typeof(ClampDiapasonAttribute))]
    internal class ClampDiapasonDrawer : AttributeDrawer<ClampDiapasonAttribute>
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Type type = EditorUtilityExt.GetFieldType(this);

            if (type == typeof(Diapason))
            {
                DiapasonDrawerHelper.DrawFloat(position, property, label, attribute.Slider, attribute.Min, attribute.Max);
                return;
            }

            if (type == typeof(DiapasonInt))
            {
                DiapasonDrawerHelper.DrawInt(position, property, label, attribute.MinInt, attribute.MaxInt);
                return;
            }

            if (type == typeof(RngParam))
            {
                RngParamDrawer.Draw(position, property, label, attribute.Slider, attribute.Min, attribute.Max);
                return;
            }

            EditorGui.ErrorLabel(position, label, $"Use {nameof(ClampDiapasonAttribute)} with {nameof(Diapason)} or {nameof(RngParam)}.");
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (EditorUtilityExt.GetFieldType(this) != typeof(RngParam))
                return EditorGUIUtility.singleLineHeight;

            return RngParamDrawer.GetHeight(property);
        }
    }
}
