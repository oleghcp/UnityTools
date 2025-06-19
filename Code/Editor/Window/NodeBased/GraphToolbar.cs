using System;
using System.Collections.Generic;
using System.Linq;
using OlegHcp.CSharp.Collections;
using OlegHcpEditor.Utils;
using OlegHcpEditor.Window.NodeBased.NodeDrawing;
using UnityEditor;
using UnityEngine;

namespace OlegHcpEditor.Window.NodeBased
{
    internal class GraphToolbar
    {
        public const float HEIGHT = 25f;
        private const float HINT_WIDTH = 200f;
        private const float HINT_HEIGHT = 135f;
        private const float HINT_OFFSET = 5f;

        private GraphEditorWindow _window;

        private string[] _transitionViewNames = Enum.GetNames(typeof(TransitionViewType));
        private GUIContent _infoButton = new GUIContent("?", "Info");
        private GUIContent _snapButton = new GUIContent("#", "Grid Snapping");
        private GUIContent _switchNodeDrawingButton = new GUIContent("Short View", "Hide Nodes Content");
        private GUIContent _leftWidthButton = new GUIContent("-", "Node Width");
        private GUIContent _rightWidthButton = new GUIContent("+", "Node Width");
        private GUIContent _nodeWidthLabel = new GUIContent("Width");
        private GUIContent _selectButton = new GUIContent("[ . . . ]", "Select All");
        private GUIContent _moveButton = new GUIContent("→ ● ←", "Move to Root");
        private GUIContent _openSidePanelButton = new GUIContent("< Panel", "Side Panel");
        private GUIContent _closeSidePanelButton = new GUIContent("Panel >", "Side Panel");

        private bool _hintToggle;
        private bool _sidePanelToggle;
        private bool _hideContentToggle;
        private bool _gridSnapToggle;
        private int _transitionViewType;

        public bool HideContentToggle => _hideContentToggle;
        public bool SidePanelToggle => _sidePanelToggle;
        public bool GridToggle => _gridSnapToggle;
        public bool ShowPorts => _transitionViewType == (int)TransitionViewType.Splines;

        public GraphToolbar(GraphEditorWindow window)
        {
            _window = window;
            _sidePanelToggle = EditorPrefs.GetBool(PrefsKeys.SIDE_PANEL);
            _hideContentToggle = EditorPrefs.GetBool(PrefsKeys.HIDE_CONTENT);
            _gridSnapToggle = EditorPrefs.GetBool(PrefsKeys.GRID_SNAPING);
            _transitionViewType = EditorPrefs.GetInt(PrefsKeys.TRANSITION_VIEW_TYPE);
        }

        public void Draw(Event e)
        {
            Vector2 winSize = _window.WinSize;
            Rect rect = new Rect(0f, winSize.y - HEIGHT, winSize.x, HEIGHT);

            GUILayout.BeginArea(rect, (string)null, GraphEditorStyles.Styles.Toolbar);
            GUILayout.FlexibleSpace();
            using (new EditorGUILayout.HorizontalScope())
            {
                DrawContent();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndArea();

            if (_hintToggle)
                DrawHint(winSize);

            bool isKey = e.type == EventType.KeyDown || e.type == EventType.KeyUp || e.type == EventType.MouseDrag;
            if (isKey && rect.Contains(e.mousePosition))
                e.Use();
        }

        public void Save()
        {
            EditorPrefs.SetBool(PrefsKeys.SIDE_PANEL, _sidePanelToggle);
            EditorPrefs.SetBool(PrefsKeys.HIDE_CONTENT, _hideContentToggle);
            EditorPrefs.SetBool(PrefsKeys.GRID_SNAPING, _gridSnapToggle);
            EditorPrefs.SetInt(PrefsKeys.TRANSITION_VIEW_TYPE, _transitionViewType);
        }

        private void DrawContent()
        {
            GUILayout.Space(5f);

            _hintToggle = EditorGuiLayout.ToggleButton(_infoButton, _hintToggle, GUILayout.Width(EditorGuiUtility.SmallButtonWidth));

            GUILayout.Space(10f);

            _transitionViewType = EditorGUILayout.Popup(_transitionViewType, _transitionViewNames, GUILayout.Width(100f));

            GUILayout.Space(10f);

            _hideContentToggle = EditorGuiLayout.ToggleButton(_switchNodeDrawingButton, _hideContentToggle, GUILayout.Width(100f));

            GUILayout.Space(10f);

            _gridSnapToggle = EditorGuiLayout.ToggleButton(_snapButton, _gridSnapToggle, GUILayout.Width(EditorGuiUtility.SmallButtonWidth));

            GUILayout.FlexibleSpace();

            GUILayoutOption nodeWidthButtonSize = GUILayout.Width(30f);

            if (GUILayout.RepeatButton(_leftWidthButton, nodeWidthButtonSize))
            {
                _window.SerializedGraph.ChangeNodeWidth(-1);
                GUI.changed = true;
            }

            GUILayout.Label(_nodeWidthLabel);

            if (GUILayout.RepeatButton(_rightWidthButton, nodeWidthButtonSize))
            {
                _window.SerializedGraph.ChangeNodeWidth(1);
                GUI.changed = true;
            }

            GUILayout.FlexibleSpace();

            const float buttonWidth = 50f;
            IReadOnlyList<NodeViewer> nodeViewers = _window.Map.NodeViewers;

            GUI.enabled = nodeViewers.Count > 0;

            if (GUILayout.Button(_moveButton, GUILayout.Width(buttonWidth)))
            {
                NodeViewer viewer = _window.Map.NodeViewers.FirstOrDefault(item => item.Id == _window.RootNodeId);
                if (viewer != null)
                    _window.Camera.Position = viewer.WorldRect.center;
            }

            if (GUILayout.Button(_selectButton, GUILayout.Width(buttonWidth)))
            {
                nodeViewers.ForEach(item => item.Select(true));
                GUI.changed = true;
            }

            GUI.enabled = true;

            GUILayout.FlexibleSpace();

            GUIContent panelButton = _sidePanelToggle ? _closeSidePanelButton : _openSidePanelButton;
            _sidePanelToggle = EditorGuiLayout.ToggleButton(panelButton, _sidePanelToggle, GUILayout.Width(100f));

            GUILayout.Space(5f);
        }

        private void DrawHint(Vector2 winSize)
        {
            Rect rect = new Rect(HINT_OFFSET,
                                 winSize.y - HINT_HEIGHT - HINT_OFFSET - HEIGHT,
                                 HINT_WIDTH,
                                 HINT_HEIGHT);

            using (new GUILayout.AreaScope(rect, (string)null, EditorStyles.helpBox))
            {
                GUILayout.Label("Use Ctrl for:");
                GUILayout.Label(" - multiple nodes dragging");
                GUILayout.Label(" - transitions deleting");
                GUILayout.Label(" - transitions points dragging");
                GUILayout.Space(10f);
                GUILayout.Label("\"Ctrl + Z\" isn't supported");
                GUILayout.Label("Sorry for that :(");
            }
        }
    }
}
