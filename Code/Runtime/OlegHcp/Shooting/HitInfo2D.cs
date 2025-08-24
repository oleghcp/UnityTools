#if INCLUDE_PHYSICS_2D
using UnityEngine;

namespace OlegHcp.Shooting
{
    internal struct HitInfo2D
    {
        public HitReactionType Reaction;
        public RaycastHit2D HitData;
        public Vector2 HitPosition;
        public Vector2 PreviousVelocity;
        public Vector2 NewVelocity;
    }
}
#endif
