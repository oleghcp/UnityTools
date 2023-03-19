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

#if UNITY_EDITOR
        internal static string FieldName => nameof(_mask);
#endif

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

        public void SetAll(bool flagValue, int length = SIZE)
        {
            BitMask.SetFor(ref _mask, flagValue, length);
        }

        public void SwitchFlag(int index)
        {
            BitMask.SwitchFlag(ref _mask, index);
        }

        public void Or(IntMask other, int length = SIZE)
        {
            BitMask.OrFor(ref _mask, other._mask, length);
        }

        public void And(IntMask other, int length = SIZE)
        {
            BitMask.AndFor(ref _mask, other._mask, length);
        }

        public void Not(int length = SIZE)
        {
            BitMask.NotFor(ref _mask, length);
        }

        public void Xor(IntMask other, int length = SIZE)
        {
            BitMask.XorFor(ref _mask, other._mask, length);
        }

        public void Except(IntMask other, int length = SIZE)
        {
            BitMask.ExceptFor(ref _mask, other._mask, length);
        }

        public bool Equals(IntMask other, int length = SIZE)
        {
            return BitMask.Equals(_mask, other._mask, length);
        }

        public bool Intersects(IntMask other, int length = SIZE)
        {
            return BitMask.Intersects(_mask, other._mask, length);
        }

        public bool All(int length = SIZE)
        {
            return BitMask.AllFor(_mask, length);
        }

        public bool Any(int length = SIZE)
        {
            return BitMask.AnyFor(_mask, length);
        }

        public bool Empty(int length = SIZE)
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
            return _mask == other._mask;
        }

        public override bool Equals(object other)
        {
            return other is IntMask mask && Equals(mask);
        }
        #endregion

        #region Operators
        public static bool operator ==(IntMask a, IntMask b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(IntMask a, IntMask b)
        {
            return !a.Equals(b);
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
