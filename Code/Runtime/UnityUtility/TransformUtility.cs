using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityUtility
{
    public static class TransformUtility
    {
        public static void OrderSiblingsByList(IList<Transform> siblings)
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

        public static void OrderSiblingsByList(IList<GameObject> siblings)
        {
            for (int i = 0; i < siblings.Count; i++)
            {
                siblings[i].transform.SetSiblingIndex(i);
            }
        }

        public static void AlignPositions(Span<Vector2> positions, float step, in Vector2 center, in Vector2 direction)
        {
            int count = positions.Length;
            float offset = (count - 1) * step * 0.5f;

            for (int i = 0; i < count; i++)
            {
                positions[i] = center - direction * offset;
                offset -= step;
            }
        }

        public static void AlignPositions(Span<Vector3> positions, float step, in Vector3 center, in Vector3 direction)
        {
            int count = positions.Length;
            float offset = (count - 1) * step * 0.5f;

            for (int i = 0; i < count; i++)
            {
                positions[i] = center - direction * offset;
                offset -= step;
            }
        }

        public static void AlignPositions(Span<Vector2> positions, Span<float> stepsBetweenItems, in Vector2 center, in Vector2 direction)
        {
            int count = positions.Length;
            float offset = stepsBetweenItems.Sum() * 0.5f;

            for (int i = 0; i < count; i++)
            {
                positions[i] = center - direction * offset;

                if (i + 1 < count)
                    offset -= stepsBetweenItems[i];
            }
        }

        public static void AlignPositions(Span<Vector3> positions, Span<float> stepsBetweenItems, in Vector3 center, in Vector3 direction)
        {
            int count = positions.Length;
            float offset = stepsBetweenItems.Sum() * 0.5f;

            for (int i = 0; i < count; i++)
            {
                positions[i] = center - direction * offset;

                if (i + 1 < count)
                    offset -= stepsBetweenItems[i];
            }
        }
    }
}
