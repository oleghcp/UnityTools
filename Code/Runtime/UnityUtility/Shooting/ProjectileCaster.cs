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

        public bool Cast(Vector3 start, Vector3 end, out RaycastHit hitInfo)
        {
            Vector3 direction = end - start;

            if (_castBounds > float.Epsilon)
            {
                bool hit = Physics.SphereCast(start, _castBounds, direction, out hitInfo, direction.magnitude, _hitMask);

                if (!_highPrecision)
                    return hit;
                else if (hit)
                    return true;
            }

            return Physics.Raycast(start, direction, out hitInfo, direction.magnitude, _hitMask);
        }
    }
}
