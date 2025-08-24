#if INCLUDE_PHYSICS || INCLUDE_PHYSICS_2D
using System.Collections.Generic;
using OlegHcp.CSharp.Collections;
using UnityEngine;

namespace OlegHcp.Shooting
{
    internal static class ShootingExtensions
    {
#if INCLUDE_PHYSICS
        public static void CleanUp(this HashSet<Component> self, bool clearCollection)
        {
            if (self.IsNullOrEmpty())
                return;

            foreach (Collider item in self)
            {
                item.enabled = true;
            }

            if (clearCollection)
                self.Clear();
        }

        public static void Invoke(this List<HitInfo> self, IProjectileEventListener listener)
        {
            if (listener != null)
            {
                for (int i = 0; i < self.Count; i++)
                {
                    HitInfo hit = self[i];
                    listener.OnHitModified(hit.HitData, hit.HitPosition, hit.PreviousVelocity, hit.NewVelocity, hit.Reaction);
                }
            }

            self.Clear();
        }
#endif

#if INCLUDE_PHYSICS_2D
        public static void CleanUp2D(this HashSet<Component> self, bool clearCollection)
        {
            if (self.IsNullOrEmpty())
                return;

            foreach (Collider2D item in self)
            {
                item.enabled = true;
            }

            if (clearCollection)
                self.Clear();
        }

        public static void Invoke(this List<HitInfo2D> self, IProjectile2DEventListener listener)
        {
            if (listener != null)
            {
                for (int i = 0; i < self.Count; i++)
                {
                    HitInfo2D hit = self[i];
                    listener.OnHitModified(hit.HitData, hit.HitPosition, hit.PreviousVelocity, hit.NewVelocity, hit.Reaction);
                }
            }

            self.Clear();
        }
#endif

        public static bool Has(this HashSet<Component> self, Component item)
        {
            if (self.IsNullOrEmpty())
                return false;

            return self.Contains(item);
        }
    }
}
#endif
