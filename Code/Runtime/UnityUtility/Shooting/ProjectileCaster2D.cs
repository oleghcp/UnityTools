using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityUtility.MathExt;

namespace UnityUtility.Shooting
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ProjectileCaster2D
    {
        [field: SerializeField]
        public LayerMask HitMask { get; set; }
        [SerializeField, Min(0f)]
        private float _castBounds;
        [field: SerializeField]
        public bool HighPrecision { get; set; }
        [SerializeField, Range(0f, 1f)]
        private float _reflectedCastNear;

        public float CastBounds
        {
            get => _castBounds;
            set => _castBounds = value.CutBefore(0f);
        }

        public float ReflectedCastNear
        {
            get => _reflectedCastNear;
            set => _reflectedCastNear = value.Clamp01();
        }

        internal bool Cast(in Vector2 source, in Vector2 direction, float distance, out RaycastHit2D hitInfo)
        {
            if (_castBounds > float.Epsilon)
            {
                hitInfo = Physics2D.CircleCast(source, _castBounds, direction, distance, HitMask);

                if (!HighPrecision)
                    return hitInfo.Hit();
                else if (hitInfo.Hit())
                    return true;
            }

            hitInfo = Physics2D.Raycast(source, direction, distance, HitMask);
            return hitInfo.Hit();
        }


    }
}
