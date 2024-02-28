using System;
using OlegHcp.NodeBased.Service;
using UnityEngine;

namespace OlegHcp.NodeBased
{
    [Serializable]
    public abstract class Condition
    {
        [SerializeField, HideInInspector]
        private bool _not;

#if UNITY_EDITOR
        internal static string NotFieldName => nameof(_not);
#endif

        internal bool Check(RawNode from, object data)
        {
            return Satisfied(from, data) != _not;
        }

        protected abstract bool Satisfied(RawNode from, object data);
    }

    [Serializable]
    internal class Any : Condition
    {
        [SerializeReference]
        private Condition[] _conditions;

        protected override bool Satisfied(RawNode from, object data)
        {
            for (int i = 0; i < _conditions.Length; i++)
            {
                if (_conditions[i].Check(from, data))
                    return true;
            }

            return false;
        }
    }

    [Serializable]
    internal class All : Condition
    {
        [SerializeReference]
        private Condition[] _conditions;

        protected override bool Satisfied(RawNode from, object data)
        {
            for (int i = 0; i < _conditions.Length; i++)
            {
                if (!_conditions[i].Check(from, data))
                    return false;
            }

            return true;
        }
    }
}
