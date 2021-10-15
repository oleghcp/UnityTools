using UnityEditor;
using UnityEngine;
using UnityUtility.Inspector;

namespace Project
{
    [CustomPropertyDrawer(typeof(FolderRequiredAttribute))]
    public class FolderRequiredDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!(property.objectReferenceValue is DefaultAsset defaultAsset))
            {
                Rect rect = EditorGUI.PrefixLabel(position, label);
                EditorGUI.LabelField(rect, $"Use for {nameof(DefaultAsset)}.");
                return;
            }

            if (property.objectReferenceValue != null && !defaultAsset.IsFolder())
            {
                property.objectReferenceValue = null;
            }

            EditorGUI.PropertyField(position, property, label);
        }
    }
}
