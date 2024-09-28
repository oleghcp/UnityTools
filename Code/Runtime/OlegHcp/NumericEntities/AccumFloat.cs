using System;
using OlegHcp.Tools;

namespace OlegHcp.NumericEntities
{
#if UNITY
    [Serializable]
#endif
    public struct AccumFloat : IAccumEntity<float>, IEquatable<AccumFloat>
    {
#if UNITY
        [UnityEngine.SerializeField, UnityEngine.HideInInspector]
#endif
        private float _got;
#if UNITY
        [UnityEngine.SerializeField, UnityEngine.HideInInspector]
#endif
        private float _spent;

        public float Value => _got - _spent;
        public bool IsEmpty => _spent >= _got;
        public float Got => _got;
        public float Spent => _spent;

#if UNITY_EDITOR
        internal static string GotFieldName => nameof(_got);
#endif

        public AccumFloat(float got, float spent)
        {
            if (spent > got)
                throw ThrowErrors.MinMax(nameof(spent), nameof(got));

            _got = got;
            _spent = spent;
        }

        public void Add(float addValue)
        {
            if (addValue < 0f)
                throw ThrowErrors.NegativeParameter(nameof(addValue));

            _got += addValue;
        }

        public bool Spend(float spendValue)
        {
            if (spendValue < 0f)
                throw ThrowErrors.NegativeParameter(nameof(spendValue));

            if (spendValue <= Value)
            {
                _spent += spendValue;
                return true;
            }

            return false;
        }

        public void Merge(AccumFloat other)
        {
            _got = Math.Max(_got, other._got);
            _spent = Math.Max(_spent, other._spent);
        }

        // -- //

        public override bool Equals(object obj)
        {
            return obj is AccumFloat accumFloat && Equals(accumFloat);
        }

        public bool Equals(AccumFloat other)
        {
            return other._got == _got && other._spent == _spent;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_got, _spent);
        }

        public static implicit operator float(AccumFloat entity)
        {
            return entity.Value;
        }

        // -- //

        public static bool operator ==(AccumFloat a, AccumFloat b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(AccumFloat a, AccumFloat b)
        {
            return !a.Equals(b);
        }

        public static AccumFloat operator +(AccumFloat value1, float value2)
        {
            if (value2 < 0f)
                throw ThrowErrors.NegativeParameter(nameof(value2));

            value1._got += value2;
            return value1;
        }

        public static AccumFloat operator -(AccumFloat value1, float value2)
        {
            if (value2 < 0f)
                throw ThrowErrors.NegativeParameter(nameof(value2));

            value1._spent += value2;
            return value1;
        }
    }
}
