using UnityEditor;
using UnityEngine;

namespace OlegHcpEditor.NodeBased
{
    internal class ServiceNodeDrawer : NodeDrawer
    {
        private readonly string _label;
        private readonly Color _headerColor;

        protected override string ShortDrawingView => _label;

        public ServiceNodeDrawer(string label, in Color headerColor)
        {
            _label = label;
            _headerColor = headerColor;
        }

        protected override void OnGui(SerializedProperty node, float _)
        {
            GUILayout.Label(_label);
        }

        protected override float GetHeight(SerializedProperty node)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        protected override Color GetHeaderColor(bool _)
        {
            return _headerColor;
        }
    }
}
