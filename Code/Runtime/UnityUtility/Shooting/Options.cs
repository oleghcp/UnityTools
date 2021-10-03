using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace UnityUtility.Shooting
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct RicochetOptions
    {
        public int Count;
        public LayerMask RicochetMask;
        [Range(0f, 1f)]
        public float SpeedRemainder;
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct CastOptions
    {
        [Min(0f)]
        public float CastBounds;
        public bool HighPrecision;
    }
}
