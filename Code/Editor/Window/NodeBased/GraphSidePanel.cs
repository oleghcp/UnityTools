using System.Collections.Generic;
using OlegHcp.Mathematics;
using OlegHcp.NodeBased.Service;
using OlegHcpEditor.NodeBased;
using OlegHcpEditor.Utils;
using UnityEditor;
using UnityEngine;

namespace OlegHcpEditor.Window.NodeBased
{
    internal class GraphSidePanel
    {
        private const float MIN_WIDTH = 150f;

        private GraphEditorWindow _window;
        private HashSet<string> _ignoredFields;
        private GraphPanelDrawer _panelDrawer;
        private float _scrollPos;
        private float _width;
        private bool _opened;
        private bool _dragging;
        private string[] _toolbarLabels = { "Graph", "Node" };
        private int _selectedIndex;

        public float Width => _opened ? _width : 0f;
        public GraphPanelDrawer PanelDrawer => _panelDrawer;

        public GraphSidePanel(GraphEditorWindow window)
        {
            _window = window;

            _ignoredFields = new HashSet<string>
            {
                EditorUtilityExt.SCRIPT_FIELD,
                RawGraph.NodesFieldName,
                RawGraph.RootNodeFieldName,
                RawGraph.IdGeneratorFieldName,
                RawGraph.WidthFieldName,
                RawGraph.CommonNodeFieldName,
            };

            _width = EditorPrefs.GetFloat(PrefsKeys.SIDE_PANEL_WIDTH, 300f);
            _selectedIndex = EditorPrefs.GetInt(PrefsKeys.SIDE_PANEL_TAB);
        }

        public void OnOpen()
        {
            _panelDrawer = GraphPanelDrawer.Create(_window.SerializedGraph, _ignoredFields);
            _panelDrawer.OnOpen();
        }

        public void OnClose()
        {
            _panelDrawer.OnClose();
        }

        public void Draw(Event e)
        {
            _opened = _window.SidePanelOpened;

            if (!_opened)
                return;

            float winWidth = _window.position.width;
            float height = _window.position.height - GraphToolbar.HEIGHT;
            float maxWidth = winWidth * 0.5f;
            _width = _width.ClampMax(maxWidth);
            Rect position = new Rect(winWidth - _width, 0f, _width, height);

            GUILayout.BeginArea(position, EditorStyles.helpBox);

            if (_window.FullDrawing)
            {
                _scrollPos = EditorGuiLayout.BeginScrollViewVertical(_scrollPos);
                _panelDrawer.Draw(_width);
            }
            else
            {
                _selectedIndex = GUILayout.Toolbar(_selectedIndex, _toolbarLabels);
                GUILayout.Space(5f);
                _scrollPos = EditorGuiLayout.BeginScrollViewVertical(_scrollPos);
                switch (_selectedIndex)
                {
                    case 0: _panelDrawer.Draw(_width); break;
                    case 1: DrawNode(); break;
                }
            }

            EditorGUILayout.EndScrollView();
            GUILayout.EndArea();

            HandleResize(height, maxWidth, winWidth, e);
        }

        public void Save()
        {
            EditorPrefs.SetFloat(PrefsKeys.SIDE_PANEL_WIDTH, _width);
            EditorPrefs.SetInt(PrefsKeys.SIDE_PANEL_TAB, _selectedIndex);
        }

        private void DrawNode()
        {
            int count = _window.Map.SelectionCount;

            if (count > 1)
            {
                GUILayout.Label("[...]");
                return;
            }

            if (count == 1)
            {
                var selected = _window.Map.GetSelectedNode();
                GUILayout.Label(selected.NameProp.stringValue, EditorStyles.boldLabel);
                selected.NodeDrawer.Draw(selected.NodeProp, _width, true);
            }
        }

        private void HandleResize(float height, float maxWidth, float winWidth, Event e)
        {
            const float resizeZoneWidth = 6f;
            Rect resizeZone = new Rect(winWidth - _width, 0f, resizeZoneWidth, height);
            EditorGUIUtility.AddCursorRect(resizeZone, MouseCursor.ResizeHorizontal);

            if (e.button == 0)
            {
                switch (e.type)
                {
                    case EventType.MouseDown:
                        if (resizeZone.Contains(e.mousePosition))
                            _dragging = true;
                        break;

                    case EventType.MouseDrag:
                        if (_dragging)
                        {
                            _width = winWidth - e.mousePosition.x;
                            _width = _width.Clamp(MIN_WIDTH, maxWidth);
                            GUI.changed = true;
                            e.Use();
                        }
                        break;

                    case EventType.MouseUp:
                        _dragging = false;
                        break;
                }
            }
        }
    }
}
