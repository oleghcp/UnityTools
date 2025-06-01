using System;
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

        public bool Check(object data)
        {
            return Satisfied(data) != _not;
        }

        protected abstract bool Satisfied(object data);
    }

    [Serializable]
    internal class Any : Condition
    {
        [SerializeReference]
        private Condition[] _conditions;

        protected override bool Satisfied(object data)
        {
            for (int i = 0; i < _conditions.Length; i++)
            {
                if (_conditions[i].Check(data))
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

        protected override bool Satisfied(object data)
        {
            for (int i = 0; i < _conditions.Length; i++)
            {
                if (!_conditions[i].Check(data))
                    return false;
            }

            return true;
        }
    }
}
