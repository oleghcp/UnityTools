using System;
using UnityEditor;
using UnityEngine;
using UnityUtility.CSharp;
using UnityUtility.Inspector;
using UnityUtilityEditor.Engine;
using UnityObject = UnityEngine.Object;

namespace Drawers
{
    [CustomPropertyDrawer(typeof(CertainTypesAttribute))]
    public class CertainTypesDrawer : AttributeDrawer<CertainTypesAttribute>
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Type type = EditorUtilityExt.GetFieldType(this);
            if (!type.IsAssignableTo(typeof(UnityObject)))
            {
                EditorGui.ErrorLabel(position, label, $"Use {nameof(CertainTypesAttribute)} with {nameof(UnityEngine)}.{nameof(UnityEngine.Object)}.");
                return;
            }

            UnityObject prevValue = property.objectReferenceValue;
            property.Draw(position, label);

            if (property.objectReferenceValue == null)
                return;

            Type tryType = property.objectReferenceValue.GetType();
            if (!Inherited(tryType))
            {
                property.objectReferenceValue = prevValue;
                Debug.LogWarning($"Cannot assign {tryType}. It is not specified by {nameof(CertainTypesAttribute)}.");
            }
        }

        private bool Inherited(Type assignedType)
        {
            for (int i = 0; i < attribute.Types.Length; i++)
            {
                if (assignedType.IsAssignableTo(attribute.Types[i]))
                    return true;
            }

            return false;
        }
    }
}
