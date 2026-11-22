using System;
using System.Linq;
using OlegHcp.Engine;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace OlegHcpEditor.Engine
{
    public static class UnityObjectExtensions
    {
        public static void DestroyImmediate(this UnityObject self)
        {
            UnityObject.DestroyImmediate(self);
        }

        public static void DestroyChildrenImmediate(this Transform self)
        {
            Transform[] children = self.GetTopChildren();

            for (int i = children.Length - 1; i >= 0; i--)
            {
                UnityObject.DestroyImmediate(children[i].gameObject);
            }
        }

        public static void DestroyChildrenImmediate(this Transform self, Predicate<Transform> predicate)
        {
            Transform[] children = self.EnumerateChildren(false)
                                       .Where(item => predicate(item))
                                       .ToArray();

            for (int i = children.Length - 1; i >= 0; i--)
            {
                UnityObject.DestroyImmediate(children[i].gameObject);
            }
        }
    }
}
