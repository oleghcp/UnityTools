using UnityEditor;
using UnityEngine;
using UnityUtility.Mathematics;
using UnityUtility.NumericEntities;
using UnityUtilityEditor.Engine;

namespace UnityUtilityEditor.Drawers
{
    [CustomPropertyDrawer(typeof(Diapason))]
    [CustomPropertyDrawer(typeof(DiapasonInt))]
    internal class DiapasonDrawer : PropertyDrawer
    {
        private GUIContent[] _floatSubLabels = { new GUIContent(Diapason.MinFieldName), new GUIContent(Diapason.MaxFieldName) };
        private GUIContent[] _intSubLabels = { new GUIContent(DiapasonInt.FromFieldName), new GUIContent(DiapasonInt.BeforeFieldName) };

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position = EditorGUI.PrefixLabel(position, label);

            if (EditorUtilityExt.GetFieldType(this) == typeof(Diapason))
            {
                SerializedProperty min = property.FindPropertyRelative(_floatSubLabels[0].text);
                SerializedProperty max = property.FindPropertyRelative(_floatSubLabels[1].text);

                float[] array = { min.floatValue, max.floatValue };

                EditorGUI.MultiFloatField(position, _floatSubLabels, array);

                min.floatValue = array[0];
                max.floatValue = array[1].ClampMin(array[0]);
            }
            else
            {
                SerializedProperty min = property.FindPropertyRelative(_intSubLabels[0].text);
                SerializedProperty max = property.FindPropertyRelative(_intSubLabels[1].text);

                int[] array = { min.intValue, max.intValue };

                EditorGUI.MultiIntField(position, _intSubLabels, array);

                min.intValue = array[0];
                max.intValue = array[1].ClampMin(array[0] + 1);
            }
        }
    }
}
