#if UNITY_2020_1_OR_NEWER
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using OlegHcp;
using OlegHcp.CSharp;
using OlegHcpEditor.Engine;
using UnityObject = UnityEngine.Object;

namespace OlegHcpEditor.Drawers
{
    [CustomPropertyDrawer(typeof(AssetRef<>))]
    internal class AssetRefDrawer : PropertyDrawer
    {
        private string[] _enumNames;
        private int[] _enumValues;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty typeProp = property.FindPropertyRelative(AssetRef<UnityObject>.TypeFieldName);
            SerializedProperty dirRefProp = property.FindPropertyRelative(AssetRef<UnityObject>.DirectRefFieldName);
            SerializedProperty lazyRefProp = property.FindPropertyRelative(AssetRef<UnityObject>.LazyRefFieldName);
#if INCLUDE_ADDRESSABLES
            SerializedProperty asyncRefProp = property.FindPropertyRelative(AssetRef<UnityObject>.AsyncRefFieldName);
#endif

            if (_enumNames == null)
            {
                _enumNames = typeProp.enumDisplayNames;
                _enumValues = Enum.GetValues(typeof(RefType)).Cast<int>().ToArray();
            }

            const float buttonWidth = 50f;

            position = EditorGUI.PrefixLabel(position, label);

            Rect rect = position;
            rect.width = buttonWidth;
            int prevTypeIndex = typeProp.enumValueIndex;
            typeProp.enumValueIndex = EditorGUI.IntPopup(rect, prevTypeIndex, _enumNames, _enumValues);

            int newTypeIndex = typeProp.enumValueIndex;
            if (prevTypeIndex != newTypeIndex)
                getProperty((RefType)prevTypeIndex).ResetToDefault();

            rect = position;
            rect.xMin += buttonWidth + EditorGuiUtility.StandardHorizontalSpacing;

#if INCLUDE_ADDRESSABLES
            if (newTypeIndex == (int)RefType.Async && EditorUtilityExt.GetFieldType(this).GenericTypeArguments[0].IsAssignableTo(typeof(Component)))
            {
                GUI.Label(rect, "Not-components only");
                return;
            }
#endif
            EditorGUI.PropertyField(rect, getProperty((RefType)newTypeIndex), GUIContent.none);

            SerializedProperty getProperty(RefType prevType)
            {
                switch (prevType)
                {
                    case RefType.Simple: return dirRefProp;
                    case RefType.Lazy: return lazyRefProp;
#if INCLUDE_ADDRESSABLES
                    case RefType.Async: return asyncRefProp;
#endif
                    default: throw new UnsupportedValueException(prevType);
                }
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}
#endif
