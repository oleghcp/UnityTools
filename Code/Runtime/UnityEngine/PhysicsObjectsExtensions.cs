using System.Runtime.CompilerServices;

namespace UnityEngine
{
    public static class PhysicsObjectsExtensions
    {
        /// <summary>
        /// Returns true if any object was hit. Otherwise returns false.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Hit(this in RaycastHit self)
        {
            return self.collider != null;
        }

        /// <summary>
        /// Calls GetComponent() on the game object that was hit.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GetComponent<T>(this in RaycastHit self)
        {
            return self.collider.GetComponent<T>();
        }

        /// <summary>
        /// Returns the layer in which the game object that was hit is.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetLayer(this in RaycastHit self)
        {
            return self.collider.gameObject.layer;
        }

        /// <summary>
        /// Returns true if any object was hit. Otherwise returns false.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Hit(this in RaycastHit2D self)
        {
            return self.collider != null;
        }

        /// <summary>
        /// Calls GetComponent() on the game object that was hit.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GetComponent<T>(this in RaycastHit2D self)
        {
            return self.collider.GetComponent<T>();
        }

        /// <summary>
        /// Returns the layer in which the game object that was hit is.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetLayer(this in RaycastHit2D self)
        {
            return self.collider.gameObject.layer;
        }

        /// <summary>
        /// Returns the layer in which the game object that was hit is.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetLayer(this Collision self)
        {
            return self.collider.gameObject.layer;
        }

        /// <summary>
        /// Returns the layer in which the game object that was hit is.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetLayer(this Collision2D self)
        {
            return self.collider.gameObject.layer;
        }

        /// <summary>
        /// Returns the layer in which the game object that was hit is.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetLayer(this ControllerColliderHit self)
        {
            return self.collider.gameObject.layer;
        }
    }
}
