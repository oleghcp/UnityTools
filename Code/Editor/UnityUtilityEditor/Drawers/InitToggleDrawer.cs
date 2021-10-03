#if UNITY_2019_3_OR_NEWER
using System;
using UnityEditor;
using UnityEngine;
using UnityUtility;

namespace UnityUtilityEditor.Drawers
{
    [CustomPropertyDrawer(typeof(InitToggleAttribute))]
    internal class InitToggleDrawer : SerializeReferenceDrawer<InitToggleAttribute>
    {
        protected override void DrawContent(Rect position, SerializedProperty property)
        {
            float shift = EditorGUIUtility.labelWidth + EditorGUIUtility.standardVerticalSpacing;

            position.x += shift;
            position.width -= shift;
            position.height = EditorGUIUtility.singleLineHeight;

            bool inited = !property.managedReferenceFullTypename.IsNullOrEmpty();

            bool switched = EditorGUI.Toggle(position, inited);

            if (switched != inited)
            {
                property.serializedObject.Update();

                if (switched)
                {
                    Type fieldType = EditorUtilityExt.GetTypeFromSerializedPropertyTypename(property.managedReferenceFieldTypename);
                    property.managedReferenceValue = Activator.CreateInstance(fieldType);
                }
                else
                {
                    property.managedReferenceValue = null;
                }

                property.serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
#endif
