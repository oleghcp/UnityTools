using UU.Controls.ControlStuff;
using UnityEngine;

#pragma warning disable CS0169, CS0649
namespace UU.Controls
{
    [CreateAssetMenu(menuName = "Input (ext.)/Layout Config", fileName = "LayoutConfig")]
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

        public InputType Type
        {
            get { return InputType; }
        }

        public BindLayout ToBindLayout()
        {
            return new BindLayout(this);
        }
    }
}
