using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityUtility;

#if UNITY_2019_3_OR_NEWER
namespace UnityUtilityEditor.Window.NodeBased.Stuff
{
    internal class GraphEditorStyles
    {
        private static GraphEditorStyles _styles;

        public GUIStyle Toolbar { get; }
        public GUIStyle Node { get; }
        public GUIStyle SelectedNode { get; }
        public GUIStyle NodeHeader { get; }
        public GUIStyle InPort { get; }
        public GUIStyle OutPort { get; }
        public GUIStyle InfoButton { get; }
        public GUIContent RightTriangle { get; }

        public static GraphEditorStyles Styles => _styles ?? (_styles = new GraphEditorStyles());

        public GraphEditorStyles()
        {
            string path = EditorGUIUtility.isProSkin ? "builtin skins/darkskin/images"
                                                     : "builtin skins/lightskin/images";

            Toolbar = new GUIStyle();
            Toolbar.normal.background = Load($"{path}/toolbar back.png");

            Node = new GUIStyle();
            Node.normal.background = Load($"{path}/node0.png");
            Node.border = new RectOffset(12, 12, 12, 12);

            SelectedNode = new GUIStyle();
            SelectedNode.normal.background = Load($"{path}/node0 on.png");
            SelectedNode.border = new RectOffset(12, 12, 12, 12);

            NodeHeader = new GUIStyle(EditorStyles.boldLabel);
            NodeHeader.normal.background = Load($"{path}/pre button.png");
            NodeHeader.border = new RectOffset(4, 4, 4, 4);
            NodeHeader.contentOffset = new Vector2(2f, 0f);
            NodeHeader.alignment = TextAnchor.MiddleLeft;

            InPort = new GUIStyle();
            InPort.normal.background = Load($"{path}/btn left.png");
            InPort.active.background = Load($"{path}/btn left on.png");
            InPort.alignment = TextAnchor.MiddleCenter;
            InPort.contentOffset = new Vector2(1f, -1f);
            InPort.border = new RectOffset(4, 4, 4, 4);

            OutPort = new GUIStyle();
            OutPort.normal.background = Load($"{path}/btn right.png");
            OutPort.active.background = Load($"{path}/btn right on.png");
            OutPort.alignment = TextAnchor.MiddleCenter;
            OutPort.contentOffset = new Vector2(1f, -1f);
            OutPort.border = new RectOffset(4, 4, 4, 4);

            RightTriangle = new GUIContent(Load($"{path}/trianglepointingright15px.png"));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color GetLineColor()
        {
            return EditorGUIUtility.isProSkin ? Colours.White : Colours.Black;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Texture2D Load(string path)
        {
            return EditorGUIUtility.Load(path) as Texture2D;
        }
    }
}
#endif
