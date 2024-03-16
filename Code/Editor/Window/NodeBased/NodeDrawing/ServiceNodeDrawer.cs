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

        protected override void OnGui(SerializedProperty property, float width)
        {
            EditorGUILayout.LabelField(_label);
        }

        protected override float GetHeight(SerializedProperty property)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        protected override Color GetHeaderColor(bool _)
        {
            return _headerColor;
        }
    }
}
