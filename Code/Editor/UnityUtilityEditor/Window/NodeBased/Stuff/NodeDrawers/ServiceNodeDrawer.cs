#if UNITY_2019_3_OR_NEWER
using UnityEditor;

namespace UnityUtilityEditor.Window.NodeBased.Stuff.NodeDrawers
{
    internal class ServiceNodeDrawer : RegularNodeDrawer
    {
        private string _label;

        protected override string ShortDrawingView => _label;

        public ServiceNodeDrawer(GraphMap map, string label) : base(map)
        {
            _label = label;
        }

        protected override void OnGui(SerializedProperty property)
        {
            EditorGUILayout.LabelField(_label);
        }
    }
}
#endif
