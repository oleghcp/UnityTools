#if UNITY_2019_3_OR_NEWER
using UnityEditor;
using UnityEngine;
using UnityUtility;

namespace UnityUtilityEditor.Window.NodeBased
{
    internal class GraphEditorStyles
    {
        private static GraphEditorStyles _styles;

        public GUIStyle Toolbar { get; }
        public GUIStyle NodeRegular { get; }
        public GUIStyle NodeSelected { get; }
        public GUIStyle InPort { get; }
        public GUIStyle OutPort { get; }
        public GUIContent RightTriangle { get; }

        public static GraphEditorStyles Styles => _styles ?? (_styles = new GraphEditorStyles());

        public GraphEditorStyles()
        {
            string path = EditorGuiUtility.BuiltInSkinsPath;

            Toolbar = new GUIStyle();
            Toolbar.normal.background = Load($"{path}toolbar back.png");

            NodeRegular = new GUIStyle
            {
                border = new RectOffset(12, 12, 12, 12),
            };
            NodeRegular.normal.background = Load($"{path}node0.png");

            NodeSelected = new GUIStyle
            {
                border = new RectOffset(12, 12, 12, 12),
            };
            NodeSelected.normal.background = Load($"{path}node0 on.png");

            InPort = new GUIStyle
            {
                alignment = TextAnchor.MiddleCenter,
                border = new RectOffset(4, 4, 4, 4),
                contentOffset = new Vector2(1f, -1f),
            };
            InPort.normal.background = Load($"{path}btn left.png");
            InPort.active.background = Load($"{path}btn left on.png");

            OutPort = new GUIStyle
            {
                alignment = TextAnchor.MiddleCenter,
                contentOffset = new Vector2(1f, -1f),
                border = new RectOffset(4, 4, 4, 4),
            };
            OutPort.normal.background = Load($"{path}btn right.png");
            OutPort.active.background = Load($"{path}btn right on.png");

            RightTriangle = new GUIContent(Load($"{path}trianglepointingright15px.png"));
        }

        public static Color GetLineColor()
        {
            return EditorGUIUtility.isProSkin ? Colours.White : Colours.Black;
        }

        private static Texture2D Load(string path)
        {
            return EditorGUIUtility.Load(path) as Texture2D;
        }
    }
}
#endif
