#if UNITY_2019_3_OR_NEWER
using System;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityUtility.IdGenerating;
using UnityUtility.MathExt;
using UnityUtility.NodeBased;

namespace UnityUtilityEditor.Window.NodeBased.Stuff
{
    internal class GraphAssetEditor
    {
        public const float PANEL_WIDTH = 300f;
        public const float SERV_NODE_WIDTH = 150f;
        private const float MIN_NODE_WIDTH = 200f;
        private const float MAX_NODE_WIDTH = 400f;
        private const float NODE_WIDTH_STEP = 1f;

        private RawGraph _graphAsset;
        private SerializedObject _serializedObject;
        private SerializedProperty _nodesProperty;

        private float _nodeWidth;
        private Vector2 _scrollPos;
        private IntIdGenerator _idGenerator;

        public RawGraph GraphAsset => _graphAsset;
        public SerializedProperty NodesProperty => _nodesProperty;
        public Type NodeType => _graphAsset.GetNodeType();
        public float NodeWidth => _nodeWidth;
        public SerializedObject SerializedObject => _serializedObject;

        public GraphAssetEditor(RawGraph graphAsset)
        {
            _graphAsset = graphAsset;
            _serializedObject = new SerializedObject(graphAsset);
            _nodesProperty = _serializedObject.FindProperty(RawGraph.NodesFieldName);
            _idGenerator = new IntIdGenerator(_serializedObject.FindProperty(RawGraph.IdGeneratorFieldName).intValue);
            _nodeWidth = _serializedObject.FindProperty(RawGraph.WidthFieldName).floatValue.Clamp(MIN_NODE_WIDTH, MAX_NODE_WIDTH);
        }

        public void Draw(in Rect position)
        {
            GUILayout.BeginArea(position);
            _scrollPos.y = EditorGUILayout.BeginScrollView(_scrollPos, EditorStyles.helpBox).y;

            EditorGUILayout.LabelField(_graphAsset.name, EditorStyles.boldLabel);

            EditorGUILayout.Space();

            foreach (SerializedProperty item in _serializedObject.EnumerateProperties())
            {
                if (!IsServiceField(item))
                    EditorGUILayout.PropertyField(item, true);
            }

            EditorGUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        public void ChangeNodeWidth(int dir)
        {
            _nodeWidth = (_nodeWidth + (NODE_WIDTH_STEP * dir)).Clamp(MIN_NODE_WIDTH, MAX_NODE_WIDTH);
        }

        public void SetAsRoot(NodeViewer node)
        {
            _serializedObject.FindProperty(RawGraph.RootNodeFieldName).intValue = node.Id;
        }

        public SerializedProperty CreateNode(Vector2 position, Type type)
        {
            RawNode newNodeAsset = Activator.CreateInstance(type) as RawNode;

            newNodeAsset.Id = _idGenerator.GetNewId();
            newNodeAsset.Owner = _graphAsset;
            newNodeAsset.Position = position;
            newNodeAsset.NodeName = GetDefaultNodeName(type, newNodeAsset.Id);

            _serializedObject.FindProperty(RawGraph.IdGeneratorFieldName).intValue = newNodeAsset.Id;
            SerializedProperty nodeProp = _nodesProperty.AddArrayElement();
            nodeProp.managedReferenceValue = newNodeAsset;

            SerializedProperty rootNodeIdProp = _serializedObject.FindProperty(RawGraph.RootNodeFieldName);
            if (rootNodeIdProp.intValue == 0 && NodeViewer.GetNodeType(type).RealNode())
                rootNodeIdProp.intValue = newNodeAsset.Id;

            return nodeProp;
        }

        public SerializedProperty CloneNode(Vector2 position, int sourceNodeId)
        {
            throw new NotImplementedException();
        }

        public void RemoveNode(int nodeId)
        {
            int index = _nodesProperty.GetArrayElement(out var element, item => item.FindPropertyRelative(RawNode.IdFieldName).intValue == nodeId);
            element.managedReferenceValue = null;
            _nodesProperty.DeleteArrayElementAtIndex(index);

            if (_nodesProperty.arraySize == 0)
            {
                _serializedObject.FindProperty(RawGraph.RootNodeFieldName).intValue = 0;
                _idGenerator = new IntIdGenerator();
                _serializedObject.FindProperty(RawGraph.IdGeneratorFieldName).intValue = 0;
            }
            else
            {
                SerializedProperty rootNodeIdProp = _serializedObject.FindProperty(RawGraph.RootNodeFieldName);
                if (nodeId == rootNodeIdProp.intValue)
                {
                    bool replaced = false;

                    foreach (SerializedProperty item in _nodesProperty.EnumerateArrayElements())
                    {
                        Type type = EditorUtilityExt.GetTypeFromSerializedPropertyTypename(item.managedReferenceFullTypename);
                        if (NodeViewer.GetNodeType(type).RealNode())
                        {
                            rootNodeIdProp.intValue = item.FindPropertyRelative(RawNode.IdFieldName).intValue;
                            replaced = true;
                            break;
                        }
                    }

                    if (!replaced)
                        rootNodeIdProp.intValue = 0;
                }
            }
        }

        public void Save()
        {
            _serializedObject.FindProperty(RawGraph.WidthFieldName).floatValue = _nodeWidth;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetDefaultNodeName(Type type, int id)
        {
            if (type.Is(typeof(ExitNode)))
                return "Exit";

            if (type.Is(typeof(HubNode)))
                return $"Hub {id}";

            return $"{type.Name} {id}";
        }

        private bool IsServiceField(SerializedProperty property)
        {
            string fieldName = property.propertyPath;

            return fieldName == EditorUtilityExt.SCRIPT_FIELD ||
                   fieldName == RawGraph.NodesFieldName ||
                   fieldName == RawGraph.IdGeneratorFieldName ||
                   fieldName == RawGraph.WidthFieldName;
        }
    }
}
#endif
