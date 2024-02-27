using UnityEditor;
using UnityEngine;
using OlegHcp.Inspector;
using OlegHcpEditor.Engine;

namespace OlegHcpEditor.Drawers.Attributes
{
    [CustomPropertyDrawer(typeof(FolderRequiredAttribute))]
    internal class FolderRequiredDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.PropertyField(position, property, label);

            if (property.objectReferenceValue == null)
                return;

            if (property.objectReferenceValue.IsFolder())
                return;

            property.objectReferenceValue = null;
            Debug.Log("Field can refer to a folder asset only.");
        }
    }
}
