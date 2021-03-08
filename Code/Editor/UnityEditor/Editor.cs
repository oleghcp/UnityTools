using System.Collections.Generic;
using UnityObject = UnityEngine.Object;

namespace UnityEditor
{
    public abstract class Editor<T> : Editor where T : UnityObject
    {
        public new T target
        {
            get => base.target as T;
            set => base.target = value;
        }

        public IEnumerable<T> EnumerateTargets()
        {
            UnityObject[] targetsArray = targets;

            for (int i = 0; i < targetsArray.Length; i++)
            {
                yield return targetsArray[i] as T;
            }
        }
    }
}
