using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityUtility.IdGenerating;
using UnityUtility.MathExt;
using UnityUtility.NodeBased;
using UnityObject = UnityEngine.Object;

#if UNITY_2019_3_OR_NEWER
namespace UnityUtilityEditor.Window.NodeBased.Stuff
{
    internal class GraphAssetEditor
    {
        private const float DEF_NODE_WIDTH = 200f;
        private const float MIN_NODE_WIDTH = 200f;
        private const float MAX_NODE_WIDTH = 400f;
        private const float NODE_WIDTH_STEP = 1f;

        private Graph _graphAsset;
        private SerializedObject _serializedObject;
        private SerializedProperty _nodesProperty;
        private SerializedProperty _idProperty;
        private SerializedProperty _widthProperty;
        private SerializedProperty _cameraPositionProperty;

        private IntIdGenerator _idGenerator;
        private Func<Type> _getNodeType;
        private Func<Type> _getTransitionType;

        public Graph GraphAsset => _graphAsset;
        public Node RootNode => _graphAsset.RootNode;
        public Type NodeType => _getNodeType();
        public Type TransitionType => _getTransitionType();
        public float NodeWidth => _widthProperty.floatValue;
        public Vector2 CameraPosition
        {
            get => _cameraPositionProperty.vector2Value;
            set
            {
                _cameraPositionProperty.vector2Value = value;
                _serializedObject.ApplyModifiedPropertiesWithoutUndo();
            }
        }

        public GraphAssetEditor(Graph graphAsset)
        {
            _graphAsset = graphAsset;
            _getNodeType = (Func<Type>)Delegate.CreateDelegate(typeof(Func<Type>), _graphAsset, DummyGrapth.GetNodeTypeMethodName);
            _getTransitionType = (Func<Type>)Delegate.CreateDelegate(typeof(Func<Type>), _graphAsset, DummyGrapth.GetTransitionTypeMethodName);
            _serializedObject = new SerializedObject(graphAsset);
            _nodesProperty = _serializedObject.FindProperty(Graph.ArrayFieldName);
            _idProperty = _serializedObject.FindProperty(Graph.IdGeneratorFieldName);
            _widthProperty = _serializedObject.FindProperty(Graph.WidthFieldName);
            _cameraPositionProperty = _serializedObject.FindProperty(Graph.CameraPositionFieldName);
            _idGenerator = new IntIdGenerator(_idProperty.intValue);

            if (_widthProperty.floatValue == 0)
            {
                _widthProperty.floatValue = DEF_NODE_WIDTH;
                _serializedObject.ApplyModifiedPropertiesWithoutUndo();
            }
        }

        public IEnumerable<Node> ParseList()
        {
            return _nodesProperty.EnumerateArrayElements()
                                 .Select(item => item.objectReferenceValue as Node);
        }

        public void ChangeNodeWidth(int dir)
        {
            _widthProperty.floatValue = (_widthProperty.floatValue + (NODE_WIDTH_STEP * dir)).Clamp(MIN_NODE_WIDTH, MAX_NODE_WIDTH);
            _serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        public void SetAsRoot(Node nodeAsset)
        {
            int index = _nodesProperty.GetArrayElement(out _, item => item.objectReferenceValue == nodeAsset);
            _nodesProperty.MoveArrayElement(index, 0);
            _serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        public Node CreateNode(Vector2 position, Type type)
        {
            Node newNodeAsset = ScriptableObject.CreateInstance(type) as Node;
            InitAndSaveCreatedNode(position, newNodeAsset);
            return newNodeAsset;
        }

        public Node CreateNode(Vector2 position, Node sourceNode)
        {
            Node newNodeAsset = sourceNode.Install();
            InitAndSaveCreatedNode(position, newNodeAsset);
            return newNodeAsset;
        }

        private void InitAndSaveCreatedNode(Vector2 position, Node newNodeAsset)
        {
            int newId = _idGenerator.GetNewId();

            newNodeAsset.name = $"Node {newId}";
            newNodeAsset.Id = newId;
            newNodeAsset.Owner = _graphAsset;
            newNodeAsset.Position = position;

            AssetDatabase.AddObjectToAsset(newNodeAsset, _graphAsset);
            AssetDatabase.SaveAssets();

            _idProperty.intValue = newId;
            _nodesProperty.PlaceArrayElement().objectReferenceValue = newNodeAsset;
            _serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        public Transition CreateTransition()
        {
            return Activator.CreateInstance(_getTransitionType()) as Transition;
        }

        public void DestroyNode(Node nodeAsset)
        {
            int index = _nodesProperty.GetArrayElement(out var element, item => item.objectReferenceValue == nodeAsset);
            element.objectReferenceValue = null;
            _nodesProperty.DeleteArrayElementAtIndex(index);

            if (_nodesProperty.arraySize == 0)
            {
                _idGenerator = new IntIdGenerator();
                _idProperty.intValue = 0;
                _cameraPositionProperty.vector2Value = default;
            }

            _serializedObject.ApplyModifiedPropertiesWithoutUndo();
            UnityObject.DestroyImmediate(nodeAsset, true);
            AssetDatabase.SaveAssets();
        }

        private abstract class DummyGrapth : Graph<Node, DummyTransition> { }
        private sealed class DummyTransition : Transition<Node> { }
    }
}
#endif
