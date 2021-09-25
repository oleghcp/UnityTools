using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace UnityUtility.Shooting
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct ProjectileCaster
    {
        [SerializeField]
        private LayerMask _hitMask;
        [SerializeField, Min(0f)]
        private float _castBounds;
        [SerializeField]
        private bool _highPrecision;

        public bool Cast(in Vector3 source, in Vector3 direction, float distance, out RaycastHit hitInfo)
        {
            if (_castBounds > float.Epsilon)
            {
                bool hit = Physics.SphereCast(source, _castBounds, direction, out hitInfo, distance, _hitMask);

                if (!_highPrecision)
                    return hit;
                else if (hit)
                    return true;
            }

            return Physics.Raycast(source, direction, out hitInfo, distance, _hitMask);
        }
    }
}
