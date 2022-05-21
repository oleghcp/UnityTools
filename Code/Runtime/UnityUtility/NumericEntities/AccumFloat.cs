using System;
using UnityEngine;
using UnityUtility.MathExt;
using UnityUtilityTools;

namespace UnityUtility.NumericEntities
{
    [Serializable]
    public struct AccumFloat : IAccumEntity<float>, IEquatable<AccumFloat>, IMergeable<AccumFloat>
    {
        [SerializeField, HideInInspector]
        private float _got;
        [SerializeField, HideInInspector]
        private float _spent;

        public float Value => _got - _spent;
        public bool IsEmpty => _got.Approx(_spent);
        public float Got => _got;
        public float Spent => _spent;

        public AccumFloat(float got, float spent)
        {
            if (spent > got)
                throw Errors.MinMax(nameof(spent), nameof(got));

            _got = got;
            _spent = spent;
        }

        public void Add(float addValue)
        {
            if (addValue < 0f)
                throw Errors.NegativeParameter(nameof(addValue));

            _got += addValue;
        }

        public bool Spend(float spendValue)
        {
            if (spendValue < 0f)
                throw Errors.NegativeParameter(nameof(spendValue));

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
            return obj is AccumFloat accumFloat && this == accumFloat;
        }

        public bool Equals(AccumFloat other)
        {
            return this == other;
        }

        public override int GetHashCode()
        {
            return Helper.GetHashCode(_got.GetHashCode(), _spent.GetHashCode());
        }

        public static implicit operator float(AccumFloat entity)
        {
            return entity.Value;
        }

        // -- //

        public static bool operator ==(AccumFloat a, AccumFloat b)
        {
            return a._got == b._got && a._spent == b._spent;
        }

        public static bool operator !=(AccumFloat a, AccumFloat b)
        {
            return !(a == b);
        }

        public static AccumFloat operator +(AccumFloat value1, float value2)
        {
            if (value2 < 0f)
                throw Errors.NegativeParameter(nameof(value2));

            value1._got += value2;
            return value1;
        }

        public static AccumFloat operator -(AccumFloat value1, float value2)
        {
            if (value2 < 0f)
                throw Errors.NegativeParameter(nameof(value2));

            value1._spent += value2;
            return value1;
        }
    }
}
