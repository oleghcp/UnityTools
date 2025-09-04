#if INCLUDE_PHYSICS || INCLUDE_PHYSICS_2D
using System.Collections.Generic;
using OlegHcp.CSharp.Collections;
using UnityEngine;

namespace OlegHcp.Shooting
{
    internal static class ShootingExtensions
    {
        public static void Restore(this HashSet<Component> self, bool clearCollection)
        {
            if (self.IsNullOrEmpty())
                return;

            foreach (Component item in self)
            {
                item.gameObject.SetActive(true);
            }

            if (clearCollection)
                self.Clear();
        }

#if INCLUDE_PHYSICS
        public static void Invoke(this List<HitInfo> self, IProjectileEventListener listener)
        {
            for (int i = 0; i < self.Count; i++)
            {
                if (UnityObjectUtility.IsNullOrDead(listener))
                    break;

                HitInfo hit = self[i];
                listener.OnHitModified(hit.HitData, hit.HitPosition, hit.PreviousVelocity, hit.NewVelocity, hit.Reaction);
            }

            self.Clear();
        }
#endif

#if INCLUDE_PHYSICS_2D
        public static void Invoke(this List<HitInfo2D> self, IProjectile2DEventListener listener)
        {
            for (int i = 0; i < self.Count; i++)
            {
                if (UnityObjectUtility.IsNullOrDead(listener))
                    break;

                HitInfo2D hit = self[i];
                listener.OnHitModified(hit.HitData, hit.HitPosition, hit.PreviousVelocity, hit.NewVelocity, hit.Reaction);
            }

            self.Clear();
        }
#endif
    }
}
#endif
