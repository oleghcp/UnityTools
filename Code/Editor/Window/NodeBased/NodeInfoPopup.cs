using System;
using OlegHcp.NodeBased.Service;
using OlegHcpEditor.Window.NodeBased.NodeDrawing;
using UnityEditor;
using UnityEngine;

namespace OlegHcpEditor.Window.NodeBased
{
    internal class NodeInfoPopup : EditorWindow
    {
        private NodeViewer _nodeEditor;
        private GraphEditorWindow _mainWindow;
        private Type _nodeType;

        private Vector2 _scrollPosition;

        private void OnEnable()
        {
            minSize = new Vector2(250f, 250f);
        }

        public static void Open(NodeViewer nodeEditor, GraphEditorWindow mainWindow)
        {
            NodeInfoPopup window = CreateInstance<NodeInfoPopup>();
            window.titleContent = new GUIContent(nodeEditor.NameProp.stringValue);
            window.SetUp(nodeEditor, mainWindow);
            window.ShowAuxWindow();
        }

        private void SetUp(NodeViewer nodeEditor, GraphEditorWindow mainWindow)
        {
            _nodeEditor = nodeEditor;
            _mainWindow = mainWindow;
            _nodeType = nodeEditor.SystemType;
        }

        private void OnDestroy()
        {
            if (_mainWindow != null)
                _mainWindow.Focus();
        }

        private void OnGUI()
        {
            _nodeEditor.NodeProp.serializedObject.Update();

            EditorGUILayout.Space(2f);
            _scrollPosition.y = EditorGUILayout.BeginScrollView(_scrollPosition, EditorStyles.helpBox).y;

            if (_nodeEditor.Type.RealNode())
            {
                EditorGUILayout.LabelField(_nodeType.FullName);
                EditorGUILayout.LabelField($"Local Id: {_nodeEditor.Id}");

                EditorGUILayout.Space(10f);
            }

            EditorGUILayout.LabelField($"Connections ( {_nodeEditor.TransitionViewers.Count} ):");

            string tab = " -";
            GUILayoutOption tabWidth = GUILayout.Width(10f);
            GUILayoutOption buttonWidth = GUILayout.Width(70f);

            foreach (TransitionViewer item in _nodeEditor.TransitionViewers)
            {
                bool stop = false;

                EditorGUILayout.BeginHorizontal();
                GUILayout.Label(tab, tabWidth);
                GUILayout.Label(item.Destination.Node.NameProp.stringValue);
                if (GUILayout.Button("Remove", buttonWidth))
                {
                    _nodeEditor.RemoveTransition(item);
                    stop = true;
                }
                EditorGUILayout.EndHorizontal();

                if (stop)
                    break;
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.Space(2f);

            _nodeEditor.NodeProp.serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
