using System.Collections;
using System.Collections.Generic;

namespace UnityEngine
{
    internal class ParrentIterator : IEnumerable<Transform>
    {
        internal Transform Owner;

        public IEnumerator<Transform> GetEnumerator()
        {
            for (Transform p = Owner.parent; p != null; p = p.parent)
            {
                yield return p;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    internal class ChildrenIteratorSimple : IEnumerable<Transform>
    {
        internal Transform Owner;

        public IEnumerator<Transform> GetEnumerator()
        {
            int length = Owner.childCount;

            for (int i = 0; i < length; i++)
            {
                yield return Owner.GetChild(i);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Owner.GetEnumerator();
        }
    }

    internal class ChildrenIteratorFull : IEnumerable<Transform>
    {
        internal Transform Owner;

        public IEnumerator<Transform> GetEnumerator()
        {
            return f_iterate(Owner);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return f_iterate(Owner);
        }

        private IEnumerator<Transform> f_iterate(Transform root)
        {
            int length = root.childCount;

            for (int i = 0; i < length; i++)
            {
                Transform child = root.GetChild(i);

                yield return child;

                var chIterator = f_iterate(child);

                while (chIterator.MoveNext())
                {
                    yield return chIterator.Current;
                }
            }
        }
    }

    public static class TransformLINQExtensions
    {
        /// <summary>
        /// Returns IEnumerable collection of parents;
        /// </summary>
        public static IEnumerable<Transform> AsParents(this Transform t)
        {
            return new ParrentIterator { Owner = t };
        }

        /// <summary>
        /// Returns IEnumerable collection of children;
        /// </summary>        
        /// <param name="allChildren">If true returns collection of all children. Otherwise returns topmost children only.</param>
        public static IEnumerable<Transform> AsChildren(this Transform t, bool allChildren = false)
        {
            if (allChildren)
            {
                return new ChildrenIteratorFull { Owner = t };
            }

            return new ChildrenIteratorSimple { Owner = t };
        }
    }
}
