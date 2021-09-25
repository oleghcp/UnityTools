using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace UnityUtility.Shooting
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct ProjectileCaster2D
    {
        [SerializeField]
        private LayerMask _hitMask;
        [SerializeField, Min(0f)]
        private float _castBounds;
        [SerializeField]
        private bool _highPrecision;

        public bool Cast(in Vector2 source, in Vector2 direction, float distance, out RaycastHit2D hitInfo)
        {
            if (_castBounds > float.Epsilon)
            {
                hitInfo = Physics2D.CircleCast(source, _castBounds, direction, distance, _hitMask);

                if (!_highPrecision)
                    return hitInfo.Hit();
                else if (hitInfo.Hit())
                    return true;
            }

            hitInfo = Physics2D.Raycast(source, direction, distance, _hitMask);
            return hitInfo.Hit();
        }


    }
}
