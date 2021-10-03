using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace UnityUtility.Shooting
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct RicochetInfo
    {
        public int Count;
        public LayerMask RicochetMask;
        [Range(0f, 1f)]
        public float SpeedRemainder;
    }
}
