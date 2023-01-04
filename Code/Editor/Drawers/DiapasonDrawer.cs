using UnityEditor;
using UnityEngine;
using UnityUtility.Mathematics;
using UnityUtility.NumericEntities;

namespace UnityUtilityEditor.Drawers
{
    [CustomPropertyDrawer(typeof(Diapason))]
    internal class DiapasonDrawer : PropertyDrawer
    {
        private GUIContent[] _subLabels = { new GUIContent(Diapason.MinFieldName), new GUIContent(Diapason.MaxFieldName) };

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position = EditorGUI.PrefixLabel(position, label);

            SerializedProperty min = property.FindPropertyRelative(_subLabels[0].text);
            SerializedProperty max = property.FindPropertyRelative(_subLabels[1].text);

            float[] array = { min.floatValue, max.floatValue };

            EditorGUI.MultiFloatField(position, _subLabels, array);

            min.floatValue = array[0];
            max.floatValue = array[1].ClampMin(array[0]);
        }
    }

    [CustomPropertyDrawer(typeof(DiapasonInt))]
    internal class DiapasonIntDrawer : PropertyDrawer
    {
        private GUIContent[] _subLabels = { new GUIContent(DiapasonInt.FromFieldName), new GUIContent(DiapasonInt.BeforeFieldName) };

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position = EditorGUI.PrefixLabel(position, label);

            SerializedProperty min = property.FindPropertyRelative(_subLabels[0].text);
            SerializedProperty max = property.FindPropertyRelative(_subLabels[1].text);

            int[] array = { min.intValue, max.intValue };

            EditorGUI.MultiIntField(position, _subLabels, array);

            min.intValue = array[0];
            max.intValue = array[1].ClampMin(array[0] + 1);
        }
    }
}
