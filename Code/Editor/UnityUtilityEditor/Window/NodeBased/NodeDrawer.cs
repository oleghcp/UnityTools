#if UNITY_2019_3_OR_NEWER
using UnityEditor;

namespace UnityUtilityEditor.Window.NodeBased
{
    public abstract class NodeDrawer
    {
        public abstract void OnGui(SerializedProperty property);
        public abstract float GetHeight(SerializedProperty property);
    }
}
#endif
