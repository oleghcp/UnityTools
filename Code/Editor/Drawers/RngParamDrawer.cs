using OlegHcp.NumericEntities;
using OlegHcp.Strings;
using OlegHcpEditor.Engine;
using UnityEditor;
using UnityEngine;
using static UnityEditor.EditorGUIUtility;

namespace OlegHcpEditor.Drawers
{
    [CustomPropertyDrawer(typeof(RngParam))]
    internal class RngParamDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Draw(position, property, label, false);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return GetHeight(property);
        }

        public static void Draw(in Rect position, SerializedProperty property, GUIContent label, bool slider, float min = float.NegativeInfinity, float max = float.PositiveInfinity)
        {
            string name = label.text;

            Rect lineRect = position;
            lineRect.height = singleLineHeight;

            DiapasonDrawerHelper.DrawFloat(lineRect, property.FindPropertyRelative(RngParam.RangeFieldName), EditorGuiUtility.TempContent(StringUtility.Space), slider, min, max);

            EditorGUI.PrefixLabel(lineRect, EditorGuiUtility.TempContent(name));
            property.isExpanded = EditorGUI.Foldout(lineRect, property.isExpanded, GUIContent.none, true);

            if (!property.isExpanded)
                return;

            EditorGUI.indentLevel++;
            lineRect = position;
            lineRect.yMin += singleLineHeight + standardVerticalSpacing;
            property.FindPropertyRelative(RngParam.ParamsFieldName)
                    .Draw(lineRect);
            EditorGUI.indentLevel--;
        }

        public static float GetHeight(SerializedProperty property)
        {
            if (!property.isExpanded)
                return singleLineHeight;

            using (SerializedProperty paramProp = property.FindPropertyRelative(RngParam.ParamsFieldName))
            {
                return RngParamOptionDrawer.GetHeight(paramProp) + singleLineHeight + standardVerticalSpacing;
            }
        }
    }
}
