using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityUtility.MathExt;

#if UNITY_2019_3_OR_NEWER
namespace UnityUtility.Shooting
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ProjectileCaster
    {
        [field: SerializeField]
        public LayerMask HitMask { get; set; }
        [SerializeField]
        private CastOptions _castBounds;
        [SerializeField, Range(0f, 1f)]
        private float _reflectedCastNear;

        public float CastBounds
        {
            get => _castBounds.CastBounds;
            set => _castBounds.CastBounds = value.CutBefore(0f);
        }

        public bool HighPrecision
        {
            get => _castBounds.HighPrecision;
            set => _castBounds.HighPrecision = value;
        }

        public float ReflectedCastNear
        {
            get => _reflectedCastNear;
            set => _reflectedCastNear = value.Clamp01();
        }

        internal bool Cast(in Vector3 source, in Vector3 direction, float distance, out RaycastHit hitInfo)
        {
            if (CastBounds > float.Epsilon)
            {
                bool hit = Physics.SphereCast(source, CastBounds, direction, out hitInfo, distance, HitMask);

                if (!_castBounds.HighPrecision)
                    return hit;
                else if (hit)
                    return true;
            }

            return Physics.Raycast(source, direction, out hitInfo, distance, HitMask);
        }

        internal bool Cast(in Vector2 source, in Vector2 direction, float distance, out RaycastHit2D hitInfo)
        {
            if (_castBounds.CastBounds > float.Epsilon)
            {
                hitInfo = Physics2D.CircleCast(source, _castBounds.CastBounds, direction, distance, HitMask);

                if (!_castBounds.HighPrecision)
                    return hitInfo.Hit();
                else if (hitInfo.Hit())
                    return true;
            }

            hitInfo = Physics2D.Raycast(source, direction, distance, HitMask);
            return hitInfo.Hit();
        }
    }
}
#endif
