using System.Runtime.CompilerServices;

namespace UnityEngine
{
    public static class PhysicsObjectsExtensions
    {
        /// <summary>
        /// Returns true if any object was hit. Otherwise returns false.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Hit(this in RaycastHit hit)
        {
            return hit.collider != null;
        }

        /// <summary>
        /// Calls GetComponent() on the game object that was hit.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GetComponent<T>(this in RaycastHit hit)
        {
            return hit.collider.GetComponent<T>();
        }

        /// <summary>
        /// Returns the layer in which the game object that was hit is.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetLayer(this in RaycastHit hit)
        {
            return hit.collider.gameObject.layer;
        }

        /// <summary>
        /// Returns true if the layer of the hit game object equal to <paramref name="comparedLayer"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CompareLayer(this in RaycastHit hit, int comparedLayer)
        {
            return hit.collider.gameObject.layer == comparedLayer;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CompareLayer(this in RaycastHit hit, LayerMask mask)
        {
            return mask.HasLayer(hit.collider.gameObject.layer);
        }

        /// <summary>
        /// Returns true if any object was hit. Otherwise returns false.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Hit(this in RaycastHit2D hit)
        {
            return hit.collider != null;
        }

        /// <summary>
        /// Calls GetComponent() on the game object that was hit.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GetComponent<T>(this in RaycastHit2D hit)
        {
            return hit.collider.GetComponent<T>();
        }

        /// <summary>
        /// Returns the layer in which the game object that was hit is.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetLayer(this in RaycastHit2D hit)
        {
            return hit.collider.gameObject.layer;
        }

        /// <summary>
        /// Returns true if the layer of the hit game object equal to <paramref name="comparedLayer"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CompareLayer(this in RaycastHit2D hit, int comparedLayer)
        {
            return hit.collider.gameObject.layer == comparedLayer;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CompareLayer(this in RaycastHit2D hit, LayerMask mask)
        {
            return mask.HasLayer(hit.collider.gameObject.layer);
        }

        /// <summary>
        /// Returns the layer in which the game object that was hit is.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetLayer(this Collision collision)
        {
            return collision.gameObject.layer;
        }

        /// <summary>
        /// Returns true if the layer of the hit game object equal to <paramref name="comparedLayer"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CompareLayer(this Collision collision, int comparedLayer)
        {
            return collision.gameObject.layer == comparedLayer;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CompareLayer(this Collision collision, LayerMask mask)
        {
            return mask.HasLayer(collision.gameObject.layer);
        }

        /// <summary>
        /// Returns the layer in which the game object that was hit is.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetLayer(this Collision2D collision)
        {
            return collision.gameObject.layer;
        }

        /// <summary>
        /// Returns true if the layer of the hit game object equal to <paramref name="comparedLayer"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CompareLayer(this Collision2D collision, int comparedLayer)
        {
            return collision.gameObject.layer == comparedLayer;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CompareLayer(this Collision2D collision, LayerMask mask)
        {
            return mask.HasLayer(collision.gameObject.layer);
        }
    }
}
