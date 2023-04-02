using System;
using UnityEngine;
using UnityUtility.Inspector;
using UnityUtility.NodeBased.Service;

namespace UnityUtility.NodeBased
{
    [Serializable]
    public abstract class Condition
    {
        public abstract bool Satisfied(RawNode from, object data);
    }

    [Serializable]
    internal class Any : Condition
    {
        [SerializeReference, ReferenceSelection]
        private Condition[] _conditions;

        public override bool Satisfied(RawNode from, object data)
        {
            for (int i = 0; i < _conditions.Length; i++)
            {
                if (_conditions[i].Satisfied(from, data))
                    return true;
            }

            return false;
        }
    }

    [Serializable]
    internal class All : Condition
    {
        [SerializeReference, ReferenceSelection]
        private Condition[] _conditions;

        public override bool Satisfied(RawNode from, object data)
        {
            for (int i = 0; i < _conditions.Length; i++)
            {
                if (!_conditions[i].Satisfied(from, data))
                    return false;
            }

            return true;
        }
    }
}
