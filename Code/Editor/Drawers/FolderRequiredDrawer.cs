using UnityEditor;
using UnityEngine;
using UnityUtility.Inspector;
using UnityUtilityEditor.Engine;

namespace UnityUtilityEditor.Drawers
{
    [CustomPropertyDrawer(typeof(FolderRequiredAttribute))]
    public class FolderRequiredDrawer : PropertyDrawer
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
