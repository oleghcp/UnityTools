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
        private RawNode[] _list;
        private Type _nodeType;

        private Vector2 _scrollPosition;

        private void OnEnable()
        {
            minSize = new Vector2(250f, 250f);
        }

        public static void Open(NodeViewer nodeEditor, GraphEditorWindow mainWindow)
        {
            NodeInfoWindow window = CreateInstance<NodeInfoWindow>();
            window.titleContent = new GUIContent(nodeEditor.NodeAsset.name);
            window.SetUp(nodeEditor, mainWindow);
            window.ShowAuxWindow();
        }

        private void SetUp(NodeViewer nodeEditor, GraphEditorWindow mainWindow)
        {
            _nodeEditor = nodeEditor;
            _mainWindow = mainWindow;
            _nodeType = nodeEditor.NodeAsset.GetType();

            _list = nodeEditor.ParseTransitionsList()
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

            if (_nodeEditor.NodeAsset.RealNode())
            {
                EditorGUILayout.LabelField(_nodeType.FullName);
                EditorGUILayout.LabelField($"Local Id: {_nodeEditor.NodeAsset.LocalId}");

                EditorGUILayout.Space(10f);
            }

            EditorGUILayout.LabelField($"Connected Nodes: {_list.Length}");

            foreach (var item in _list)
            {
                EditorGUILayout.LabelField($" - {item.name}");
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.Space(2f);
        }
    }
}
