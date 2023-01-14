using System;
using UnityEditor;
using UnityEngine;
using UnityUtility.Inspector;
using UnityUtility.NumericEntities;
using UnityUtilityEditor.Engine;

namespace UnityUtilityEditor.Drawers
{
    [CustomPropertyDrawer(typeof(ClampDiapasonAttribute))]
    internal class ClampDiapasonDrawer : AttributeDrawer<ClampDiapasonAttribute>
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Type type = EditorUtilityExt.GetFieldType(this);

            if (type == typeof(Diapason))
            {
                DiapasonDrawerHelper.DrawFloat(position, property, label, attribute.Min, attribute.Max);
                return;
            }

            if (type == typeof(DiapasonInt))
            {
                DiapasonDrawerHelper.DrawInt(position, property, label, attribute.MinInt, attribute.MaxInt);
                return;
            }

            EditorGui.ErrorLabel(position, label, $"Use {nameof(ClampDiapasonAttribute)} with {nameof(Diapason)} or {nameof(DiapasonInt)}.");
        }
    }
}
