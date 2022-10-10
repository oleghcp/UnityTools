#if UNITY_2019_3_OR_NEWER
using UnityEditor;

namespace UnityUtilityEditor.Window.NodeBased.Stuff.NodeDrawers
{
    internal class ServiceNodeDrawer : RegularNodeDrawer
    {
        private string _label;

        public ServiceNodeDrawer(GraphMap map, string label) : base(map)
        {
            _label = label;
        }

        public override void OnGui(SerializedProperty property)
        {
            EditorGUILayout.LabelField(_label);
        }
    }
}
#endif
