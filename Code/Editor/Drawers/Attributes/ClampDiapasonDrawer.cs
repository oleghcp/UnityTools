using System;
using UnityEditor;
using UnityEngine;
using UnityUtility.Inspector;
using UnityUtility.Mathematics;
using UnityUtility.NumericEntities;
using UnityUtilityEditor.Engine;

namespace UnityUtilityEditor.Drawers.Attributes
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

            if (type == typeof(RngParam))
            {
                RngParamDrawer.Draw(position, property, label);
                SerializedProperty rangeProp = property.FindPropertyRelative(RngParam.RangeFieldName);
                Diapason range = rangeProp.GetDiapasonValue();
                range.Min = range.Min.Clamp(attribute.Min, attribute.Max);
                range.Max = range.Max.Clamp(attribute.Min, attribute.Max);
                rangeProp.SetDiapasonValue(range);
                return;
            }

            EditorGui.ErrorLabel(position, label, $"Use {nameof(ClampDiapasonAttribute)} with {nameof(Diapason)} or {nameof(RngParam)}.");
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (EditorUtilityExt.GetFieldType(this) == typeof(RngParam))
                return RngParamDrawer.GetHeight(property, label);

            return EditorGUIUtility.singleLineHeight;
        }
    }
}
