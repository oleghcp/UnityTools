#if INCLUDE_PHYSICS || INCLUDE_PHYSICS_2D
using UnityEditor;
using UnityEngine;
using UnityUtility.Shooting;
using UnityUtilityEditor.Engine;

namespace UnityUtilityEditor.Drawers.Shooting
{
    [CustomPropertyDrawer(typeof(DragOptions))]
    internal class DragOptionsDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            const string dragWord = "Drag";
            string methodPropName = nameof(DragOptions.Method);
            string valuePropName = nameof(DragOptions.Value);

            SerializedProperty methodProp = property.FindPropertyRelative(methodPropName);
            SerializedProperty valueProp = property.FindPropertyRelative(valuePropName);

            Rect linePos = position;
            linePos.height = EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(linePos, methodProp, EditorGuiUtility.TempContent($"{dragWord} {methodPropName}"));

            if (methodProp.enumValueIndex != 0)
            {
                linePos = EditorGuiUtility.GetLinePosition(position, 1);
                EditorGUI.PropertyField(linePos, valueProp, EditorGuiUtility.TempContent($"{dragWord} {valuePropName}"));
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.FindPropertyRelative(nameof(DragOptions.Method)).enumValueIndex != 0)
                return EditorGUIUtility.singleLineHeight * 2f + EditorGUIUtility.standardVerticalSpacing;

            return EditorGUIUtility.singleLineHeight;
        }
    }
}
#endif
