using System.Collections.Generic;
using OlegHcp.NodeBased.Service;
using OlegHcpEditor.Engine;
using OlegHcpEditor.Window.NodeBased;
using UnityEditor;

namespace OlegHcpEditor.NodeBased
{
    public class GraphPanelDrawer
    {
        private SerializedGraph _serializedGraph;
        private ICollection<string> _ignoredFields;

        protected SerializedObject SerializedObject => _serializedGraph.SerializedObject;
        protected RawGraph GraphAsset => _serializedGraph.GraphAsset;

        internal void SetUp(SerializedGraph serializedGraph, ICollection<string> ignoredFields)
        {
            _serializedGraph = serializedGraph;
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
