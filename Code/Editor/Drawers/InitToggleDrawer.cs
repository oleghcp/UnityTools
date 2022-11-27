#if UNITY_2019_3_OR_NEWER
using System;
using UnityEditor;
using UnityEngine;
using UnityUtility.Inspector;

namespace UnityUtilityEditor.Drawers
{
    [CustomPropertyDrawer(typeof(InitToggleAttribute))]
    internal class InitToggleDrawer : SerializeReferenceDrawer
    {
        protected override void DrawContent(in Rect position, SerializedProperty property)
        {
            Type type = EditorUtilityExt.GetFieldType(this);

            if (type.IsAbstract || type.IsInterface)
            {
                GUI.Label(position, "Use non-abstract type.");
                return;
            }

            bool inited = property.HasManagedReferenceValue();
            int level = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            bool switched = EditorGUI.Toggle(position, inited);
            EditorGUI.indentLevel = level;

            if (switched != inited)
            {
                property.serializedObject.Update();
                property.managedReferenceValue = switched ? Activator.CreateInstance(type) : null;
                property.isExpanded = false;
                property.serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
#endif
