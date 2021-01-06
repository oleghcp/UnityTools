using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityUtility
{
    public static class VectorExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static (float min, float max) ToMinMaxTuple(this in Vector2 self)
        {
            return (self.x, self.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static (int min, int max) ToMinMaxTuple(this in Vector2Int self)
        {
            return (self.x, self.y);
        }
    }
}
