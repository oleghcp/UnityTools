using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityUtility.NodeBased;
using UnityUtilityEditor.Window.NodeBased.Stuff;

namespace UnityUtilityEditor.Window.NodeBased
{
    internal class NodeInfoWindow : EditorWindow
    {
        private NodeViewer _nodeEditor;
        private GraphEditorWindow _mainWindow;
        private string[] _nextNodeNames;
        private Type _nodeType;

        private Vector2 _scrollPosition;

        private void OnEnable()
        {
            minSize = new Vector2(250f, 250f);
        }

        public static void Open(NodeViewer nodeEditor, GraphEditorWindow mainWindow)
        {
            NodeInfoWindow window = CreateInstance<NodeInfoWindow>();
            window.titleContent = new GUIContent(nodeEditor.FindSubProperty(RawNode.NameFieldName).stringValue);
            window.SetUp(nodeEditor, mainWindow);
            window.ShowAuxWindow();
        }

        private void SetUp(NodeViewer nodeEditor, GraphEditorWindow mainWindow)
        {
            _nodeEditor = nodeEditor;
            _mainWindow = mainWindow;
            _nodeType = nodeEditor.SystemType;

            var dict = mainWindow.GraphAssetEditor
                                 .NodesProperty
                                 .EnumerateArrayElements()
                                 .Select(item => (item.FindPropertyRelative(RawNode.IdFieldName).intValue, item.FindPropertyRelative(RawNode.NameFieldName).stringValue))
                                 .ToDictionary(key => key.Item1, value => value.Item2);

            _nextNodeNames = nodeEditor.FindSubProperty(RawNode.ArrayFieldName)
                                       .EnumerateArrayElements()
                                       .Select(item => dict[item.FindPropertyRelative(Transition.NodeIdFieldName).intValue])
                                       .ToArray();
        }

        private void OnDestroy()
        {
            if (_mainWindow != null)
                _mainWindow.Focus();
        }

        private void OnGUI()
        {
            EditorGUILayout.Space(2f);
            _scrollPosition.y = EditorGUILayout.BeginScrollView(_scrollPosition, EditorStyles.helpBox).y;

            if (_nodeEditor.Type.RealNode())
            {
                EditorGUILayout.LabelField(_nodeType.FullName);
                EditorGUILayout.LabelField($"Local Id: {_nodeEditor.Id}");

                EditorGUILayout.Space(10f);
            }

            EditorGUILayout.LabelField($"Connected Nodes: {_nextNodeNames.Length}");

            for (int i = 0; i < _nextNodeNames.Length; i++)
            {
                EditorGUILayout.LabelField($" - {_nextNodeNames[i]}");
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.Space(2f);
        }
    }
}
