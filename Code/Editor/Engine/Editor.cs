using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityObject = UnityEngine.Object;

namespace UnityUtilityEditor.Engine
{
    public abstract class Editor<T> : Editor, IReadOnlyList<T> where T : UnityObject
    {
#pragma warning disable IDE1006
        public new T target
        {
            get => base.target as T;
            set => base.target = value;
        }

        public new IReadOnlyList<T> targets => this;
#pragma warning restore IDE1006

        T IReadOnlyList<T>.this[int index] => (T)base.targets[index];
        int IReadOnlyCollection<T>.Count => base.targets.Length;

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            UnityObject[] array = base.targets;

            for (int i = 0; i < array.Length; i++)
            {
                yield return (T)array[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return base.targets.GetEnumerator();
        }
    }
}
