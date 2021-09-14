using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UnityUtilityEditor.Window.NodeBased.Stuff
{
    internal class GraphToolbar
    {
        public const float HEIGHT = 25f;
        private const float HINT_WIDTH = 200f;
        private const float HINT_HEIGHT = 140f;
        private const float HINT_OFFSET = 5f;

        private GraphEditorWindow _window;

        private GUIContent _selectButton = new GUIContent("[ . . . ]", "Select All");
        private GUIContent _alignButton = new GUIContent("■ □ ■", "Align Selected");
        private GUIContent _moveButton = new GUIContent("● ←", "Move to Root");
        private GUIContent _snapButton = new GUIContent("#", "Grid Snapping");
        private GUIContent _leftWidthButton = new GUIContent("<", "Node Width");
        private GUIContent _rightWidthButton = new GUIContent(">", "Node Width");

        private bool _hintToggle;
        private bool _propertiesToggle;
        private bool _gridToggle;

        public bool GridToggle => _gridToggle;
        public bool PropertiesToggle => _propertiesToggle;

        public GraphToolbar(GraphEditorWindow window)
        {
            _window = window;
            _gridToggle = EditorPrefs.GetBool(PrefsConstants.GRID_SNAPING_KEY);
        }

        public void Draw()
        {
            Vector2 winSize = _window.WinSize;
            Rect rect = new Rect(0f, winSize.y - HEIGHT, winSize.x, HEIGHT);

            GUILayout.BeginArea(rect, (string)null, GraphEditorStyles.Styles.Toolbar);
            GUILayout.FlexibleSpace();
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Space(5f);
                DrawLeft();
                GUILayout.FlexibleSpace();
                DrawMiddle();
                GUILayout.FlexibleSpace();
                DrawRigt();
                GUILayout.Space(5f);
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndArea();

            if (_hintToggle)
                DrawHint(winSize);
        }

        public void Save()
        {
            EditorPrefs.SetBool(PrefsConstants.GRID_SNAPING_KEY, _gridToggle);
        }

        private void DrawLeft()
        {
            _propertiesToggle = GUILayout.Toggle(_propertiesToggle, "Properties", GUI.skin.button, GUILayout.Width(100f));
        }

        private void DrawMiddle()
        {
            const float buttonWidth = 50f;
            IReadOnlyList<NodeViewer> nodeViewers = _window.NodeViewers;

            GUI.enabled = nodeViewers.Count > 0;

            if (GUILayout.Button(_selectButton, GUILayout.Width(buttonWidth)))
                nodeViewers.ForEach(item => item.Select(true));

            if (GUILayout.Button(_alignButton, GUILayout.Width(buttonWidth)))
            {
                Vector2? pos = null;
                foreach (NodeViewer item in nodeViewers)
                {
                    Rect nodeRect = item.WorldRect;

                    if (item.IsSelected)
                    {
                        if (pos.HasValue)
                            item.Position = pos.Value;
                        else
                            pos = nodeRect.position;

                        Vector2 newPos = pos.Value;
                        newPos.x += nodeRect.width + 20f;
                        pos = newPos;
                    }
                }
            }

            if (GUILayout.Button(_moveButton, GUILayout.Width(buttonWidth)))
            {
                NodeViewer viewer = _window.NodeViewers.First(item => item.NodeAsset == _window.RootNode);
                _window.Camera.Position = viewer.WorldRect.center;
            }

            GUI.enabled = true;
        }

        private void DrawRigt()
        {
            GUILayout.Space(10f);

            _gridToggle = GUILayout.Toggle(_gridToggle, _snapButton, GUI.skin.button, GUILayout.Width(EditorGuiUtility.SmallButtonWidth));

            GUILayout.Space(10f);

            bool minusBtn = GUILayout.RepeatButton(_leftWidthButton, GUILayout.Width(EditorGuiUtility.SmallButtonWidth));
            bool plusBtn = GUILayout.RepeatButton(_rightWidthButton, GUILayout.Width(EditorGuiUtility.SmallButtonWidth));

            if (minusBtn)
                _window.GraphAssetEditor.ChangeNodeWidth(-1);

            if (plusBtn)
                _window.GraphAssetEditor.ChangeNodeWidth(1);

            GUILayout.Space(10f);

            _hintToggle = GUILayout.Toggle(_hintToggle, "?", GUI.skin.button, GUILayout.Width(EditorGuiUtility.SmallButtonWidth));

            if (minusBtn || plusBtn)
                GUI.changed = true;
        }

        private void DrawHint(Vector2 winSize)
        {
            Rect rect = new Rect(winSize.x - HINT_WIDTH - HINT_OFFSET,
                                 winSize.y - HINT_HEIGHT - HINT_OFFSET - HEIGHT,
                                 HINT_WIDTH,
                                 HINT_HEIGHT);

            using (new GUILayout.AreaScope(rect, (string)null, EditorStyles.helpBox))
            {
                EditorGUILayout.LabelField("Use Ctrl for:");
                EditorGUILayout.LabelField(" - multiple nodes dragging");
                EditorGUILayout.LabelField(" - transitions deleting");
                EditorGUILayout.LabelField(" - transitions points dragging");
                EditorGUILayout.Space(10f);
                EditorGUILayout.LabelField("\"Ctrl + Z\" doesn't work");
                EditorGUILayout.LabelField("Sorry for that =(");
            }
        }
    }
}
