#if UNITY_2019_3_OR_NEWER
using UnityEditor;

namespace UnityUtilityEditor.Window.NodeBased
{
    public abstract class NodeDrawer
    {
        private string _label = "{...}";

        protected virtual string ShortDrawingView => _label;

        public void OnGui(SerializedProperty property, float width, bool enabled)
        {
            if (enabled)
            {
                OnGui(property, width);
                return;
            }

            EditorGUILayout.LabelField(ShortDrawingView);
        }

        public float GetHeight(SerializedProperty property, bool enabled)
        {
            if (enabled)
                return GetHeight(property);

            return EditorGUIUtility.singleLineHeight;
        }


        protected abstract void OnGui(SerializedProperty property, float width);
        protected abstract float GetHeight(SerializedProperty property);
    }
}
#endif
