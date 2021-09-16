using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityUtility;
using UnityUtility.IdGenerating;
using UnityUtility.MathExt;
using UnityUtility.NodeBased;
using UnityObject = UnityEngine.Object;

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
        public Type NodeType => _graphAsset.GetNodeType();
        public float NodeWidth => _nodeWidth;

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
            _serializedObject.Update();

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

            _serializedObject.ApplyModifiedProperties();
        }

        public IEnumerable<RawNode> ParseList()
        {
            return _nodesProperty.EnumerateArrayElements()
                                 .Select(item => item.objectReferenceValue as RawNode);
        }

        public void ChangeNodeWidth(int dir)
        {
            _nodeWidth = (_nodeWidth + (NODE_WIDTH_STEP * dir)).Clamp(MIN_NODE_WIDTH, MAX_NODE_WIDTH);
            _serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        public void SetAsRoot(RawNode nodeAsset)
        {
            int index = _nodesProperty.GetArrayElement(out _, item => item.objectReferenceValue == nodeAsset);
            _nodesProperty.MoveArrayElement(index, 0);
            _serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        public RawNode CreateNode(Vector2 position, Type type)
        {
            RawNode newNodeAsset = ScriptableObject.CreateInstance(type) as RawNode;
            InitAndSaveCreatedNode(position, newNodeAsset);
            return newNodeAsset;
        }

        public RawNode CreateNode(Vector2 position, RawNode sourceNode)
        {
            RawNode newNodeAsset = sourceNode.Install();
            InitAndSaveCreatedNode(position, newNodeAsset);
            return newNodeAsset;
        }

        public void DestroyNode(RawNode nodeAsset)
        {
            int index = _nodesProperty.GetArrayElement(out var element, item => item.objectReferenceValue == nodeAsset);
            element.objectReferenceValue = null;
            _nodesProperty.DeleteArrayElementAtIndex(index);

            if (_nodesProperty.arraySize == 0)
            {
                _idGenerator = new IntIdGenerator();
                _serializedObject.FindProperty(RawGraph.IdGeneratorFieldName).intValue = 0;
            }

            _serializedObject.ApplyModifiedPropertiesWithoutUndo();
            UnityObject.DestroyImmediate(nodeAsset, true);
            AssetDatabase.SaveAssets();
        }

        public void Save()
        {
            _serializedObject.FindProperty(RawGraph.WidthFieldName).floatValue = _nodeWidth;
            _serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetDefaultNodeName(RawNode nodeAsset)
        {
            return nodeAsset.GetType().Name;
        }

        private void InitAndSaveCreatedNode(Vector2 position, RawNode newNodeAsset)
        {
            newNodeAsset.Id = _idGenerator.GetNewId();
            newNodeAsset.Owner = _graphAsset;
            newNodeAsset.Position = position;

            if (newNodeAsset.ServiceNode())
            {
                if (newNodeAsset is HubNode)
                    newNodeAsset.name = "Hub";
                else if (newNodeAsset is ExitNode)
                    newNodeAsset.name = "Exit";
                else
                    throw new UnsupportedValueException(newNodeAsset.GetType().Name);
            }
            else
            {
                newNodeAsset.name = GetDefaultNodeName(newNodeAsset);
            }

            AssetDatabase.AddObjectToAsset(newNodeAsset, _graphAsset);
            AssetDatabase.SaveAssets();

            _serializedObject.FindProperty(RawGraph.IdGeneratorFieldName).intValue = newNodeAsset.Id;
            _nodesProperty.PlaceArrayElement().objectReferenceValue = newNodeAsset;
            _serializedObject.ApplyModifiedPropertiesWithoutUndo();
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
