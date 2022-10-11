#if UNITY_2019_3_OR_NEWER
using UnityEditor;

namespace UnityUtilityEditor.Window.NodeBased.Stuff.NodeDrawing
{
    internal class RegularNodeDrawer : NodeDrawer
    {
        private GraphMap _map;

        public RegularNodeDrawer(GraphMap map)
        {
            _map = map;
        }

        protected override void OnGui(SerializedProperty property, float width)
        {
            EditorGUIUtility.labelWidth = width * 0.5f;
            foreach (SerializedProperty item in property.EnumerateInnerProperties())
            {
                if (!_map.IsServiceField(item))
                    EditorGUILayout.PropertyField(item, true);
            }
        }

        protected override float GetHeight(SerializedProperty property)
        {
            return EditorGuiUtility.GetDrawHeight(property, _map.IsServiceField);
        }
    }
}
#endif
