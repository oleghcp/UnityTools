#if UNITY_2019_3_OR_NEWER
using UnityEditor;

namespace UnityUtilityEditor.Window.NodeBased.Stuff.NodeDrawers
{
    internal class RegularNodeDrawer : NodeDrawer
    {
        private GraphMap _map;

        public RegularNodeDrawer(GraphMap map)
        {
            _map = map;
        }

        public override void OnGui(SerializedProperty property)
        {
            foreach (SerializedProperty item in property.EnumerateInnerProperties())
            {
                if (!_map.IsServiceField(item))
                    EditorGUILayout.PropertyField(item, true);
            }
        }

        public override float GetHeight(SerializedProperty property)
        {
            return EditorGuiUtility.GetDrawHeight(property, _map.IsServiceField);
        }
    }
}
#endif
