using UnityEditor;
using UnityEngine;
using UnityUtility.NumericEntities;
using UnityUtility.Tools;
using UnityUtilityEditor.Engine;
using static UnityUtility.NumericEntities.RngParam;

namespace UnityUtilityEditor.Drawers
{
    [CustomPropertyDrawer(typeof(RngParam))]
    internal class RngParamDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Draw(position, property, label);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return GetHeight(property, label);
        }

        public static void Draw(in Rect position, SerializedProperty property, GUIContent label)
        {
            string name = label.text;
            Rect lineRect = EditorGuiUtility.GetLinePosition(position, 0);

            property.FindPropertyRelative(RngParam.RangeFieldName)
                    .Draw(lineRect, EditorGuiUtility.TempContent(Helper.SPACE));
            EditorGUI.PrefixLabel(lineRect, EditorGuiUtility.TempContent(name));
            property.isExpanded = EditorGUI.Foldout(lineRect, property.isExpanded, GUIContent.none, true);

            if (!property.isExpanded)
                return;

            EditorGUI.indentLevel++;
            lineRect = EditorGuiUtility.GetLinePosition(position, 1);
            SerializedProperty paramProp = property.FindPropertyRelative(RngParam.ParamsFieldName);
            paramProp.Draw(lineRect);
            EditorGUI.indentLevel--;
        }

        public static float GetHeight(SerializedProperty property, GUIContent label)
        {
            if (property.isExpanded)
                return property.GetHeight(label, true) - EditorGUIUtility.singleLineHeight - EditorGUIUtility.standardVerticalSpacing;

            return EditorGUIUtility.singleLineHeight;
        }
    }

    [CustomPropertyDrawer(typeof(Option))]
    internal class RngParamOptionDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty modeProp = property.FindPropertyRelative(Option.ModeFieldName);

            Rect lineRect = EditorGuiUtility.GetLinePosition(position, 0);
            EditorGUI.PropertyField(lineRect, modeProp);

            if (modeProp.enumValueIndex == 0)
                return;

            SerializedProperty intProp = property.FindPropertyRelative(Option.IntensityFieldName);
            lineRect = EditorGuiUtility.GetLinePosition(position, 1);
            EditorGUI.PropertyField(lineRect, intProp);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            using (SerializedProperty modeProp = property.FindPropertyRelative(Option.ModeFieldName))
            {
                if (modeProp.enumValueIndex == 0)
                    return EditorGUIUtility.singleLineHeight;
            }

            return EditorGUIUtility.singleLineHeight * 2f + EditorGUIUtility.standardVerticalSpacing;
        }
    }
}
