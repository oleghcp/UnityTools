namespace UnityEngine
{
#pragma warning disable UNT0014
    public static class PhysicsExtensions
    {
#if !UNITY_2019_1_OR_NEWER || INCLUDE_PHYSICS
        /// <summary>
        /// Returns the layer in which the game object that was hit is.
        /// </summary>
        public static int GetLayer(this ControllerColliderHit self)
        {
            return self.collider.gameObject.layer;
        }

        /// <summary>
        /// Returns the layer in which the game object that was hit is.
        /// </summary>
        public static int GetLayer(this Collision self)
        {
            return self.collider.gameObject.layer;
        }

        /// <summary>
        /// Returns true if any object was hit. Otherwise returns false.
        /// </summary>
        public static bool Hit(this in RaycastHit self)
        {
            return self.collider != null;
        }

        public static T GetComponent<T>(this in RaycastHit self)
        {
            return self.collider.GetComponent<T>();
        }

        public static T GetComponentInParent<T>(this in RaycastHit self)
        {
            return self.collider.GetComponentInParent<T>();
        }

        public static T GetComponentInChildren<T>(this in RaycastHit self)
        {
            return self.collider.GetComponentInChildren<T>();
        }

        /// <summary>
        /// Returns the layer in which the game object that was hit is.
        /// </summary>
        public static int GetLayer(this in RaycastHit self)
        {
            return self.collider.gameObject.layer;
        }
#endif

#if !UNITY_2019_1_OR_NEWER || INCLUDE_PHYSICS_2D
        /// <summary>
        /// Returns the layer in which the game object that was hit is.
        /// </summary>
        public static int GetLayer(this Collision2D self)
        {
            return self.collider.gameObject.layer;
        }

        /// <summary>
        /// Returns true if any object was hit. Otherwise returns false.
        /// </summary>
        public static bool Hit(this in RaycastHit2D self)
        {
            return self.collider != null;
        }

        public static T GetComponent<T>(this in RaycastHit2D self)
        {
            return self.collider.GetComponent<T>();
        }

        public static T GetComponentInParent<T>(this in RaycastHit2D self)
        {
            return self.collider.GetComponentInParent<T>();
        }

        public static T GetComponentInChildren<T>(this in RaycastHit2D self)
        {
            return self.collider.GetComponentInChildren<T>();
        }

        /// <summary>
        /// Returns the layer in which the game object that was hit is.
        /// </summary>
        public static int GetLayer(this in RaycastHit2D self)
        {
            return self.collider.gameObject.layer;
        }
#endif
    }
}
