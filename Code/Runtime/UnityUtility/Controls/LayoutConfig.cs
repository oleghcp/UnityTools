using UnityEngine;
using UnityUtility.Controls.ControlStuff;

#pragma warning disable CS0169, CS0649
namespace UnityUtility.Controls
{
    [CreateAssetMenu(menuName = nameof(UnityUtility) + " (ext.)/Input/Layout Config", fileName = "LayoutConfig")]
    public sealed class LayoutConfig : ScriptableObject
    {
#if UNITY_EDITOR
        [SerializeField, HideInInspector]
        private string _keyEnumType;
        [SerializeField, HideInInspector]
        private string _axisEnumType;
#endif

        [SerializeField]
        internal InputType InputType;

        [SerializeField]
        internal int[] KeyIndices;
        [SerializeField]
        internal int[] AxisIndices;
        [SerializeField]
        internal KeyAxes KeyAxes;

        public InputType Type
        {
            get { return InputType; }
        }

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
