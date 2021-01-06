using UnityEngine;

namespace UnityUtility
{
    public class DiapasonAttribute : PropertyAttribute
    {
        public float MinValue { get; }

        public DiapasonAttribute() { }

        public DiapasonAttribute(float minValue)
        {
            MinValue = minValue;
        }
    }
}
