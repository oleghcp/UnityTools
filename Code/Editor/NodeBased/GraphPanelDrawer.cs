using System.Collections.Generic;
using OlegHcpEditor.Engine;
using UnityEditor;

namespace OlegHcpEditor.NodeBased
{
    public class GraphPanelDrawer
    {
        private ICollection<string> _ignoredFields;

        protected SerializedObject SerializedObject { get; private set; }

        internal void SetUp(SerializedObject serializedObject, ICollection<string> ignoredFields)
        {
            SerializedObject = serializedObject;
            _ignoredFields = ignoredFields;
        }

        internal void Draw(float width)
        {
            EditorGUIUtility.labelWidth = width * 0.5f;

            OnGui();
        }

        protected virtual void OnGui()
        {
            foreach (SerializedProperty item in SerializedObject.EnumerateProperties())
            {
                if (!IsServiceField(item))
                    EditorGUILayout.PropertyField(item, true);
            }
        }

        protected bool IsServiceField(SerializedProperty property)
        {
            return _ignoredFields.Contains(property.name);
        }
    }
}
