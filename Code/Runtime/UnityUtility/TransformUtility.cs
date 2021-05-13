using System.Collections.Generic;
using UnityEngine;

namespace UnityUtility
{
    public static class TransformUtility
    {
        public static void OrderSiblingsByList<T>(IList<Transform> siblings) 
        {
            for (int i = 0; i < siblings.Count; i++)
            {
                siblings[i].SetSiblingIndex(i);
            }
        }

        public static void OrderSiblingsByList<T>(IList<T> siblings) where T : Component
        {
            for (int i = 0; i < siblings.Count; i++)
            {
                siblings[i].transform.SetSiblingIndex(i);
            }
        }

        public static void OrderSiblingsByList<T>(IList<GameObject> siblings) 
        {
            for (int i = 0; i < siblings.Count; i++)
            {
                siblings[i].transform.SetSiblingIndex(i);
            }
        }
    }
}
