using System;
using System.Collections.Generic;
using UnityEngine;
using UnityUtility.Mathematics;

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

        public static void AlignPositions(Span<Vector2> positions, in Vector2 startPosition, in Vector2 direction, float step, float startPosOffsetRatio = 0f)
        {
            int count = positions.Length;
            float offset = (count - 1) * step * startPosOffsetRatio.Clamp01();

            for (int i = 0; i < count; i++)
            {
                positions[i] = startPosition - direction * offset;
                offset -= step;
            }
        }

        public static void AlignPositions(Span<Vector3> positions, in Vector3 startPosition, in Vector3 direction, float step, float startPosOffsetRatio = 0f)
        {
            int count = positions.Length;
            float offset = (count - 1) * step * startPosOffsetRatio.Clamp01();

            for (int i = 0; i < count; i++)
            {
                positions[i] = startPosition - direction * offset;
                offset -= step;
            }
        }

        public static void AlignPositions(Span<Vector2> positions, in Vector2 startPosition, in Vector2 direction, Span<float> stepsBetweenItems, float startPosOffsetRatio = 0f)
        {
            int count = positions.Length;
            float offset = stepsBetweenItems.Sum() * startPosOffsetRatio;

            for (int i = 0; i < count; i++)
            {
                positions[i] = startPosition - direction * offset;

                if (i + 1 < count)
                    offset -= stepsBetweenItems[i];
            }
        }

        public static void AlignPositions(Span<Vector3> positions, in Vector3 startPosition, in Vector3 direction, Span<float> stepsBetweenItems, float startPosOffsetRatio = 0f)
        {
            int count = positions.Length;
            float offset = stepsBetweenItems.Sum() * startPosOffsetRatio;

            for (int i = 0; i < count; i++)
            {
                positions[i] = startPosition - direction * offset;

                if (i + 1 < count)
                    offset -= stepsBetweenItems[i];
            }
        }
    }
}
