using System.Collections.Generic;
using UnityEditor;
using UnityObject = UnityEngine.Object;

namespace UnityUtilityEditor.Engine
{
    public abstract class Editor<T> : Editor where T : UnityObject
    {
#pragma warning disable IDE1006
        public new T target
        {
            get => base.target as T;
            set => base.target = value;
        }
#pragma warning restore IDE1006

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
