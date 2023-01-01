using UnityEngine;

namespace UnityUtility
{
    public static class VectorExtensions
    {
        public static (float min, float max) ToMinMaxTuple(this in Vector2 self)
        {
            return (self.x, self.y);
        }

        public static (int from, int before) ToMinMaxTuple(this in Vector2Int self)
        {
            return (self.x, self.y);
        }
    }
}
