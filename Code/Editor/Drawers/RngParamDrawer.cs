using UnityEditor;
using UnityEngine;
using UnityUtility.NumericEntities;
using UnityUtilityEditor.Engine;

namespace UnityUtilityEditor.Drawers
{
    [CustomPropertyDrawer(typeof(RngParam))]
    internal class RngParamDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            string name = label.text;
            Rect lineRect = EditorGuiUtility.GetLinePosition(position, 0);

            SerializedProperty rangeProp = property.FindPropertyRelative(RngParam.RangeFieldName);
            Rect contentLineRect = lineRect;
            contentLineRect.xMin += EditorGUIUtility.labelWidth;
            rangeProp.Draw(contentLineRect, EditorGuiUtility.TempContent(string.Empty));

            property.isExpanded = EditorGUI.Foldout(lineRect, property.isExpanded, EditorGuiUtility.TempContent(name), true);

            if (!property.isExpanded)
                return;

            EditorGUI.indentLevel++;
            lineRect = EditorGuiUtility.GetLinePosition(position, 1);
            SerializedProperty paramProp = property.FindPropertyRelative(RngParam.ParamsFieldName);
            paramProp.Draw(lineRect);
            EditorGUI.indentLevel--;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.isExpanded)
                return property.GetHeight(label, true) - EditorGUIUtility.singleLineHeight - EditorGUIUtility.standardVerticalSpacing;

            return EditorGUIUtility.singleLineHeight;
        }
    }
}
