using UnityEngine;
#if !UNITY_2019_3_OR_NEWER || ENABLE_LEGACY_INPUT_MANAGER
using OlegHcp.Controls.ControlStuff;

namespace OlegHcp.Controls
{
    [CreateAssetMenu(menuName = nameof(OlegHcp) + "/Input/Layout Config", fileName = "LayoutConfig")]
    public sealed class LayoutConfig : ScriptableObject
    {
        [SerializeField, HideInInspector]
        private string _keyEnumType;
        [SerializeField, HideInInspector]
        private string _axisEnumType;

        [SerializeField]
        internal InputType InputType;

        [SerializeField]
        internal int[] KeyIndices;
        [SerializeField]
        internal int[] AxisIndices;
        [SerializeField]
        internal KeyAxes KeyAxes;

        public InputType Type => InputType;

#if UNITY_EDITOR
        internal static string KeyEnumTypeFieldName => nameof(_keyEnumType);
        internal static string AxisEnumTypeFieldName => nameof(_axisEnumType);
        internal static string InputTypeFieldName => nameof(InputType);
        internal static string KeyIndicesFieldName => nameof(KeyIndices);
        internal static string AxisIndicesFieldName => nameof(AxisIndices);
        internal static string KeyAxesFieldName => nameof(KeyAxes);
#endif

        public BindLayout ToBindLayout()
        {
            return new BindLayout(this);
        }
    }
}
#else
namespace OlegHcp.Controls
{
    public sealed class LayoutConfig : ScriptableObject
    { }
}
#endif
