using System.Collections.Generic;
using UnityEngine;
using UnityUtility.Mathematics;

namespace UnityUtility.Engine
{
    public static class CollectionExtensions
    {
        public static Vector3 AveragePosition(this IList<Vector3> self)
        {
            return MathUtility.AveragePosition(self);
        }

        public static Vector2 AveragePosition(this IList<Vector2> self)
        {
            return MathUtility.AveragePosition(self);
        }

        public static Vector3 AveragePosition(this IEnumerable<Vector3> self)
        {
            return MathUtility.AveragePosition(self);
        }

        public static Vector2 AveragePosition(this IEnumerable<Vector2> self)
        {
            return MathUtility.AveragePosition(self);
        }
    }
}
