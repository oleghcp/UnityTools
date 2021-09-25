using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityUtility.MathExt;

namespace UnityUtility.Shooting
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ProjectileCaster
    {
        [field: SerializeField]
        public LayerMask HitMask { get; set; }
        [SerializeField, Min(0f)]
        private float _castBounds;
        [field: SerializeField]
        public bool HighPrecision { get; set; }

        public float CastBounds
        {
            get => _castBounds;
            set => _castBounds = value.CutBefore(0f);
        }

        internal bool Cast(in Vector3 source, in Vector3 direction, float distance, out RaycastHit hitInfo)
        {
            if (CastBounds > float.Epsilon)
            {
                bool hit = Physics.SphereCast(source, CastBounds, direction, out hitInfo, distance, HitMask);

                if (!HighPrecision)
                    return hit;
                else if (hit)
                    return true;
            }

            return Physics.Raycast(source, direction, out hitInfo, distance, HitMask);
        }
    }
}
