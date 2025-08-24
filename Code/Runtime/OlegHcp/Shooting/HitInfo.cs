#if INCLUDE_PHYSICS
using UnityEngine;

namespace OlegHcp.Shooting
{
    internal struct HitInfo
    {
        public HitReactionType Reaction;
        public RaycastHit HitData;
        public Vector3 HitPosition;
        public Vector3 PreviousVelocity;
        public Vector3 NewVelocity;
    }
}
#endif
