using UnityEngine;

namespace OlegHcpEditor.Window.NodeBased.NodeDrawing
{
    internal enum PortType : byte { In, Out }

    internal class PortViewer
    {
        private const float X_OFFSET = 8f;

        private PortType _type;
        private GraphMap _map;
        private NodeViewer _node;
        private GUIStyle _style;
        private Rect _screenRect;
        private int _screenRectVersion;

        public NodeViewer Node => _node;
        public PortType Type => _type;

        public Rect ScreenRect
        {
            get
            {
                if (_screenRectVersion != _map.Window.OnGuiCounter)
                {
                    Rect nodeScreenRect = _node.ScreenRect;
                    _screenRect.y = nodeScreenRect.y + (nodeScreenRect.height * 0.5f) - _screenRect.height * 0.5f;

                    switch (_type)
                    {
                        case PortType.In:
                            _screenRect.x = nodeScreenRect.x - _screenRect.width + X_OFFSET;
                            break;

                        case PortType.Out:
                            _screenRect.x = nodeScreenRect.x + nodeScreenRect.width - X_OFFSET;
                            break;
                    }

                    _screenRectVersion = _map.Window.OnGuiCounter;
                }

                return _screenRect;
            }
        }

        public PortViewer(NodeViewer node, PortType type, GraphMap map)
        {
            _node = node;
            _type = type;
            _map = map;
            _screenRect = new Rect(0, 0, 16f, 24f);
            _style = type == PortType.In ? GraphEditorStyles.Styles.InPort : GraphEditorStyles.Styles.OutPort;
        }

        public void Draw()
        {
            if (GUI.Button(ScreenRect, GraphEditorStyles.Styles.RightTriangle, _style))
                _map.OnClickOnPort(this);
        }

        //public void DrawList(IReadOnlyList<TransitionViewer> transitions)
        //{
        //    if (_node.Type == NodeType.Exit)
        //        return;

        //    Rect position = ScreenRect;

        //    position.x += 5f;
        //    position.height = (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * transitions.Count + EditorGUIUtility.standardVerticalSpacing * 2f;
        //    position.y = ScreenRect.center.y - position.height * 0.5f;
        //    position.width = 120f;

        //    GUILayout.BeginArea(position, (string)null, EditorStyles.helpBox);
        //    for (int i = 0; i < transitions.Count; i++)
        //    {
        //        EditorGUILayout.LabelField(transitions[i].Destination.Node.Name);
        //    }
        //    GUILayout.EndArea();
        //}
    }
}
