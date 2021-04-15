using UnityEngine;

namespace UnityUtilityEditor.Window.NodeBased.Stuff
{
    internal enum PortType : byte
    {
        In,
        Out
    }

    internal class PortViewer
    {
        private const float X_OFFSET = 8f;

        private PortType _type;
        private GraphEditorWindow _window;
        private Rect _screenRect;
        private NodeViewer _node;
        private GUIStyle _style;

        public Rect ScreenRect => _screenRect;
        public NodeViewer Node => _node;
        public PortType Type => _type;

        public PortViewer(NodeViewer node, PortType type, GraphEditorWindow window)
        {
            _node = node;
            _type = type;
            _window = window;
            _screenRect = new Rect(0, 0, 16f, 24f);
            _style = type == PortType.In ? GraphEditorStyles.Styles.InPort : GraphEditorStyles.Styles.OutPort;
        }

        public void Draw()
        {
            Rect nodeScreenRect = _node.GetRectInScreen();

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

            if (GUI.Button(_screenRect, GraphEditorStyles.Styles.RightTriangle, _style))
            {
                _window.OnClickOnPort(this);
            }
        }
    }
}
