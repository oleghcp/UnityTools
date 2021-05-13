using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityUtility.IdGenerating;
using UnityUtility.MathExt;
using UnityUtility.NodeBased;
using UnityObject = UnityEngine.Object;

namespace UnityUtilityEditor.Window.NodeBased.Stuff
{
    internal class GraphAssetEditor
    {
        public const float PANEL_WIDTH = 300f;
        private const float MIN_NODE_WIDTH = 200f;
        private const float MAX_NODE_WIDTH = 400f;
        private const float NODE_WIDTH_STEP = 1f;

        private RawGraph _graphAsset;
        private SerializedObject _serializedObject;
        private SerializedProperty _nodesProperty;

        private float _nodeWidth;
        private Vector2 _scrollPos;
        private IntIdGenerator _idGenerator;
        private Func<Type> _getNodeType;
        private Func<Type> _getTransitionType;

        public RawGraph GraphAsset => _graphAsset;
        public Type NodeType => _getNodeType();
        public Type TransitionType => _getTransitionType();
        public float NodeWidth => _nodeWidth;

        public RawNode RootNode
        {
            get
            {
                if (_nodesProperty.arraySize == 0)
                    return null;

                return _nodesProperty.GetArrayElementAtIndex(0).objectReferenceValue as RawNode;
            }
        }

        public GraphAssetEditor(RawGraph graphAsset)
        {
            _graphAsset = graphAsset;
            _getNodeType = (Func<Type>)Delegate.CreateDelegate(typeof(Func<Type>), graphAsset, DummyGrapth.GetNodeTypeMethodName);
            _getTransitionType = (Func<Type>)Delegate.CreateDelegate(typeof(Func<Type>), graphAsset, DummyGrapth.GetTransitionTypeMethodName);
            _serializedObject = new SerializedObject(graphAsset);
            _nodesProperty = _serializedObject.FindProperty(RawGraph.ArrayFieldName);
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

        private void InitAndSaveCreatedNode(Vector2 position, RawNode newNodeAsset)
        {
            int newId = _idGenerator.GetNewId();

            newNodeAsset.name = $"Node {newId}";
            newNodeAsset.Id = newId;
            newNodeAsset.Owner = _graphAsset;
            newNodeAsset.Position = position;

            AssetDatabase.AddObjectToAsset(newNodeAsset, _graphAsset);
            AssetDatabase.SaveAssets();

            _serializedObject.FindProperty(RawGraph.IdGeneratorFieldName).intValue = newId;
            _nodesProperty.PlaceArrayElement().objectReferenceValue = newNodeAsset;
            _serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private bool IsServiceField(SerializedProperty property)
        {
            string fieldName = property.propertyPath;

            return fieldName == EditorUtilityExt.SCRIPT_FIELD ||
                   fieldName == RawGraph.ArrayFieldName ||
                   fieldName == RawGraph.IdGeneratorFieldName ||
                   fieldName == RawGraph.WidthFieldName;
        }
    }
}
