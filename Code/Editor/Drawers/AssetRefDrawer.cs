#if UNITY_2020_1_OR_NEWER
using System;
using UnityEditor;
using UnityEngine;
using UnityUtility;
using UnityUtilityEditor.Engine;
using UnityObject = UnityEngine.Object;

namespace UnityUtilityEditor.Drawers
{
    [CustomPropertyDrawer(typeof(AssetRef<>))]
    public class AssetRefDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty flagProp = property.FindPropertyRelative(AssetRef<UnityObject>.FlagFieldName);
            SerializedProperty dirRefProp = property.FindPropertyRelative(AssetRef<UnityObject>.DirectRefFieldName);
            SerializedProperty lazyRefProp = property.FindPropertyRelative(AssetRef<UnityObject>.LazyRefFieldName);

            const float buttonText = 40f;
            Type fieldType = EditorUtilityExt.GetFieldType(this);

            position = EditorGUI.PrefixLabel(position, label);

            Rect rect = position;
            rect.width = buttonText;
            bool prevFlagValue = flagProp.boolValue;
            flagProp.boolValue = EditorGui.ToggleButton(rect, "Lazy", prevFlagValue);

            bool lazy = flagProp.boolValue;
            if (prevFlagValue != lazy)
            {
                SerializedProperty source = lazy ? dirRefProp : lazyRefProp;
                SerializedProperty dest = lazy ? lazyRefProp : dirRefProp;

                dest.objectReferenceValue = source.objectReferenceValue;
                source.objectReferenceValue = null;
            }

            rect = position;
            rect.xMin += buttonText + EditorGuiUtility.StandardHorizontalSpacing;
            SerializedProperty drawnProp = lazy ? lazyRefProp : dirRefProp;
            drawnProp.objectReferenceValue = EditorGUI.ObjectField(rect, drawnProp.objectReferenceValue, fieldType.GenericTypeArguments[0], false);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}
#endif
