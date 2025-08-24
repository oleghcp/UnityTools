#if INCLUDE_PHYSICS || INCLUDE_PHYSICS_2D
using System.Collections.Generic;
using OlegHcp.CSharp.Collections;
using UnityEngine;

namespace OlegHcp.Shooting
{
    internal static class ShootingExtensions
    {
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

        public static bool Has(this HashSet<Component> self, Component item)
        {
            if (self.IsNullOrEmpty())
                return false;

            return self.Contains(item);
        }
    }
}
#endif
