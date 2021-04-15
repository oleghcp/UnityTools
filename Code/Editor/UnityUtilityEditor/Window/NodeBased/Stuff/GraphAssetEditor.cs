using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityUtility.IdGenerating;
using UnityUtility.MathExt;
using UnityUtility.NodeBased;
using UnityUtilityEditor.NodeBased;
using UnityObject = UnityEngine.Object;

namespace UnityUtilityEditor.Window.NodeBased.Stuff
{
    internal class GraphAssetEditor
    {
        private const float MIN_NODE_WIDTH = 200f;
        private const float MAX_NODE_WIDTH = 400f;
        private const float NODE_WIDTH_STEP = 1f;

        private Graph _graphAsset;
        private SerializedObject _serializedObject;
        private SerializedProperty _nodesProperty;
        private SerializedProperty _cameraPositionProperty;

        private float _nodeWidth;
        private IntIdGenerator _idGenerator;
        private Func<Type> _getNodeType;
        private Func<Type> _getTransitionType;

        public Graph GraphAsset => _graphAsset;
        public Type NodeType => _getNodeType();
        public Type TransitionType => _getTransitionType();
        public float NodeWidth => _nodeWidth;
        public Vector2 CameraPosition => _cameraPositionProperty.vector2Value;

        public RawNode RootNode
        {
            get
            {
                if (_nodesProperty.arraySize == 0)
                    return null;

                return _nodesProperty.GetArrayElementAtIndex(0).objectReferenceValue as RawNode;
            }
        }

        public GraphAssetEditor(Graph graphAsset)
        {
            _graphAsset = graphAsset;
            _getNodeType = (Func<Type>)Delegate.CreateDelegate(typeof(Func<Type>), graphAsset, DummyGrapth.GetNodeTypeMethodName);
            _getTransitionType = (Func<Type>)Delegate.CreateDelegate(typeof(Func<Type>), graphAsset, DummyGrapth.GetTransitionTypeMethodName);
            _serializedObject = new SerializedObject(graphAsset);
            _nodesProperty = _serializedObject.FindProperty(Graph.ArrayFieldName);
            _cameraPositionProperty = _serializedObject.FindProperty(Graph.CameraPositionFieldName);
            _idGenerator = new IntIdGenerator(_serializedObject.FindProperty(Graph.IdGeneratorFieldName).intValue);
            _nodeWidth = _serializedObject.FindProperty(Graph.WidthFieldName).floatValue.Clamp(MIN_NODE_WIDTH, MAX_NODE_WIDTH);
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

        private void InitAndSaveCreatedNode(Vector2 position, RawNode newNodeAsset)
        {
            int newId = _idGenerator.GetNewId();

            newNodeAsset.name = $"Node {newId}";
            newNodeAsset.Id = newId;
            newNodeAsset.Owner = _graphAsset;
            newNodeAsset.Position = position;

            AssetDatabase.AddObjectToAsset(newNodeAsset, _graphAsset);
            AssetDatabase.SaveAssets();

            _serializedObject.FindProperty(Graph.IdGeneratorFieldName).intValue = newId;
            _nodesProperty.PlaceArrayElement().objectReferenceValue = newNodeAsset;
            _serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        public void DestroyNode(RawNode nodeAsset)
        {
            int index = _nodesProperty.GetArrayElement(out var element, item => item.objectReferenceValue == nodeAsset);
            element.objectReferenceValue = null;
            _nodesProperty.DeleteArrayElementAtIndex(index);

            if (_nodesProperty.arraySize == 0)
            {
                _idGenerator = new IntIdGenerator();
                _serializedObject.FindProperty(Graph.IdGeneratorFieldName).intValue = 0;
            }

            _serializedObject.ApplyModifiedPropertiesWithoutUndo();
            UnityObject.DestroyImmediate(nodeAsset, true);
            AssetDatabase.SaveAssets();
        }

        public void Save(Vector2 cameraPosition)
        {
            _cameraPositionProperty.vector2Value = cameraPosition;
            _serializedObject.FindProperty(Graph.WidthFieldName).floatValue = _nodeWidth;
            _serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
