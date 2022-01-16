using System;
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
            if (!EditorUtilityExt.GetFieldType(this).Is(typeof(DefaultAsset)))
            {
                Rect rect = EditorGUI.PrefixLabel(position, label);
                EditorGUI.LabelField(rect, $"Use for {nameof(DefaultAsset)}.");
                return;
            }

            DefaultAsset defaultAsset = (DefaultAsset)property.objectReferenceValue;

            if (property.objectReferenceValue != null && !defaultAsset.IsFolder())
            {
                property.objectReferenceValue = null;
            }

            EditorGUI.PropertyField(position, property, label);
        }
    }
}
