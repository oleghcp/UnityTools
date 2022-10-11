#if UNITY_2019_3_OR_NEWER
using UnityEditor;

namespace UnityUtilityEditor.Window.NodeBased.Stuff.NodeDrawing
{
    internal class ServiceNodeDrawer : NodeDrawer
    {
        private string _label;

        protected override string ShortDrawingView => _label;

        public ServiceNodeDrawer(string label)
        {
            _label = label;
        }

        protected override void OnGui(SerializedProperty property, float width)
        {
            EditorGUILayout.LabelField(_label);
        }

        protected override float GetHeight(SerializedProperty property)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}
#endif
