using System;
using UnityEngine;

namespace UnityUtility
{
    [Serializable]
    public struct IntMask : IEquatable<IntMask>
    {
        private const int SIZE = BitMask.SIZE;

        [SerializeField, HideInInspector]
        private int _mask;

        public bool this[int index]
        {
            get => BitMask.HasFlag(_mask, index);
            set => BitMask.SetFlag(ref _mask, index, value);
        }

        public IntMask(bool defaultValue)
        {
            _mask = defaultValue ? -1 : 0;
        }

        public IntMask(int mask)
        {
            _mask = mask;
        }

        public void SwitchFlag(int index)
        {
            BitMask.SwitchFlag(ref _mask, index);
        }

        public void InvertFor(int length = SIZE)
        {
            BitMask.InvertFor(ref _mask, length);
        }

        public bool Equals(IntMask other, int length = SIZE)
        {
            return BitMask.Equals(_mask, other._mask, length);
        }

        public bool Overlaps(IntMask other, int length = SIZE)
        {
            return BitMask.Overlaps(_mask, other._mask, length);
        }

        public IntMask GetIntersection(IntMask other, int length = SIZE)
        {
            return BitMask.GetIntersection(_mask, other._mask, length);
        }

        public bool AllFor(int length = SIZE)
        {
            return BitMask.AllFor(_mask, length);
        }

        public bool AnyFor(int length = SIZE)
        {
            return BitMask.AnyFor(_mask, length);
        }

        public bool EmptyFor(int length = SIZE)
        {
            return BitMask.EmptyFor(_mask, length);
        }

        public int GetCount(int length = SIZE)
        {
            return BitMask.GetCount(_mask, length);
        }

        #region Regular Stuff
        public override int GetHashCode()
        {
            return _mask.GetHashCode();
        }

        public bool Equals(IntMask other)
        {
            return this == other;
        }

        public override bool Equals(object other)
        {
            return other is IntMask mask && this == mask;
        }
        #endregion

        #region Operators
        public static bool operator ==(IntMask a, IntMask b)
        {
            return a._mask == b._mask;
        }

        public static bool operator !=(IntMask a, IntMask b)
        {
            return !(a == b);
        }

        public static implicit operator IntMask(int value)
        {
            return new IntMask(value);
        }

        public static explicit operator int(IntMask mask)
        {
            return mask._mask;
        }

        public static implicit operator LayerMask(IntMask intMask)
        {
            return intMask._mask;
        }

        public static implicit operator IntMask(LayerMask layerMask)
        {
            return new IntMask(layerMask);
        }
        #endregion
    }
}
