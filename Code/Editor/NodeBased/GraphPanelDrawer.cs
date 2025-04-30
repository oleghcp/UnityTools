using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using OlegHcp.CSharp;
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

        public SerializedObject SerializedObject => _serializedGraph.SerializedObject;
        public RawGraph GraphAsset => _serializedGraph.GraphAsset;

        internal static GraphPanelDrawer Create(SerializedGraph serializedGraph, ICollection<string> ignoredFields)
        {
            Type graphType = serializedGraph.GraphAsset.GetType();

            Type drawerType = TypeCache.GetTypesDerivedFrom(typeof(GraphPanelDrawer))
                                       .Where(item => !item.IsAbstract && item.IsDefined(typeof(CustomGraphPanelDrawerAttribute), true))
                                       .FirstOrDefault(item => item.GetCustomAttribute<CustomGraphPanelDrawerAttribute>().GraphType == graphType);
            if (drawerType != null)
            {
                GraphPanelDrawer drawer = (GraphPanelDrawer)drawerType.CreateInstance();
                drawer._serializedGraph = serializedGraph;
                drawer._ignoredFields = ignoredFields;
                return drawer;
            }

            return new GraphPanelDrawer()
            {
                _serializedGraph = serializedGraph,
                _ignoredFields = ignoredFields,
            };
        }

        internal void OnOpen()
        {
            OnOpened();
        }

        internal void OnClose()
        {
            OnClosed();
        }

        internal void Draw(float width)
        {
            EditorGUIUtility.labelWidth = width * 0.5f;

            OnGui();
        }

        protected virtual void OnOpened() { }

        protected virtual void OnClosed() { }

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
