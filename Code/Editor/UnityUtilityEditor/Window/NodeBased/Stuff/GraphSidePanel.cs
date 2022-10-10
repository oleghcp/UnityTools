#if UNITY_2019_3_OR_NEWER
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityUtility.MathExt;
using UnityUtility.NodeBased;

namespace UnityUtilityEditor.Window.NodeBased.Stuff
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

            _width = EditorPrefs.GetFloat(PrefsConstants.SIDE_PANEL_WIDTH_KEY, 300f);
        }

        public void Draw(bool opened, float height, float winWidth, Event e)
        {
            _opened = opened;

            if (!opened)
                return;

            float maxWidth = winWidth * 0.5f;
            _width = _width.CutAfter(maxWidth);
            Rect position = new Rect(0f, 0f, _width, height);

            GUILayout.BeginArea(position);
            _scrollPos.y = EditorGUILayout.BeginScrollView(_scrollPos, EditorStyles.helpBox).y;

            EditorGUIUtility.labelWidth = EditorGUIUtility.labelWidth.CutAfter(_width * 0.5f);

            foreach (SerializedProperty item in _window.SerializedGraph.SerializedObject.EnumerateProperties())
            {
                if (!_ignoredFields.Contains(item.name))
                    EditorGUILayout.PropertyField(item, true);
            }

            EditorGUILayout.EndScrollView();
            GUILayout.EndArea();

            HandleResize(height, maxWidth, e);
        }

        public void Save()
        {
            EditorPrefs.SetFloat(PrefsConstants.SIDE_PANEL_WIDTH_KEY, _width);
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

                        if (resizeZone.Contains(mousePosition) || (_outOfBounds && IsInBounds(mousePosition.x, MIN_WIDTH, maxWidth)))
                        {
                            _dragging = true;
                            _outOfBounds = false;
                        }

                        if (_dragging)
                        {
                            _width = mousePosition.x;

                            if (!IsInBounds(mousePosition.x, MIN_WIDTH, maxWidth))
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsInBounds(float value, float min, float max)
        {
            return value >= min && value <= max;
        }
    }
}
#endif
