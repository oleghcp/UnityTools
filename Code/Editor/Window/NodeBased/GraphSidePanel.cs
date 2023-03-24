using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityUtility.Mathematics;
using UnityUtility.NodeBased;
using UnityUtilityEditor.Engine;

namespace UnityUtilityEditor.Window.NodeBased
{
    internal class GraphSidePanel
    {
        private const float MIN_WIDTH = 150f;

        private GraphEditorWindow _window;
        private HashSet<string> _ignoredFields;
        private Vector2 _scrollPos;
        private float _width;
        private bool _opened;
        private bool _dragging;
        private string[] _toolbarLabels = { "Properties", "Node" };
        private int _selectedIndex;


        public float Width => _opened ? _width : 0f;

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

        public void Draw(bool opened, float height, float winWidth, Event e)
        {
            _opened = opened;

            if (!opened)
                return;

            float maxWidth = winWidth * 0.5f;
            _width = _width.ClampMax(maxWidth);
            Rect position = new Rect(0f, 0f, _width, height);

            GUILayout.BeginArea(position, EditorStyles.helpBox);
            _selectedIndex = GUILayout.Toolbar(_selectedIndex, _toolbarLabels);
            GUILayout.Space(5f);
            _scrollPos.y = EditorGUILayout.BeginScrollView(_scrollPos).y;
            switch (_selectedIndex)
            {
                case 0: DrawContent(); break;
                case 1: DrawNode(); break;
            }
            EditorGUILayout.EndScrollView();
            GUILayout.EndArea();

            HandleResize(height, maxWidth, e);
        }

        public void Save()
        {
            EditorPrefs.SetFloat(PrefsKeys.SIDE_PANEL_WIDTH, _width);
            EditorPrefs.SetInt(PrefsKeys.SIDE_PANEL_TAB, _selectedIndex);
        }

        private void DrawContent()
        {
            EditorGUIUtility.labelWidth = _width * 0.5f;

            foreach (SerializedProperty item in _window.SerializedGraph.SerializedObject.EnumerateProperties())
            {
                if (!_ignoredFields.Contains(item.name))
                    EditorGUILayout.PropertyField(item, true);
            }
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
                selected.NodeDrawer.OnGui(selected.NodeProp, _width, true);
            }
        }

        private void HandleResize(float height, float maxWidth, Event e)
        {
            const float resizeZoneWidth = 3f;
            Rect resizeZone = new Rect(_width - resizeZoneWidth, 0f, resizeZoneWidth, height);
            EditorGUIUtility.AddCursorRect(resizeZone, MouseCursor.ResizeHorizontal);

            if (e.button == 0)
            {
                switch (e.type)
                {
                    case EventType.MouseUp:
                        _dragging = false;
                        _outOfBounds = false;
                        break;

                    case EventType.MouseDown:
                        if (resizeZone.Contains(e.mousePosition))
                            _dragging = true;
                        break;

                    case EventType.MouseDrag:
                        Vector2 mousePosition = e.mousePosition;

                        if (resizeZone.Contains(mousePosition) || (_outOfBounds && mousePosition.x.IsInBounds(MIN_WIDTH, maxWidth)))
                        {
                            _dragging = true;
                            _outOfBounds = false;
                        }

                        if (_dragging)
                        {
                            _width = mousePosition.x;

                            if (!mousePosition.x.IsInBounds(MIN_WIDTH, maxWidth))
                            {
                                _width = _width.Clamp(MIN_WIDTH, maxWidth);
                                _dragging = false;
                                _outOfBounds = true;
                            }

                            GUI.changed = true;
                        }
                        break;
                }
            }
        }

        private bool _outOfBounds;
    }
}
