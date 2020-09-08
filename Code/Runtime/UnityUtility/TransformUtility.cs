using System.Collections.Generic;
using UnityEngine;

namespace UnityUtility
{
    public static class TransformUtility
    {
        public static void OrderChildrenDueToListIndecies<T>(IList<T> components) where T : Component
        {
            for (int i = 0; i < components.Count; i++)
            {
                components[i].transform.SetSiblingIndex(i);
            }
        }
    }
}
