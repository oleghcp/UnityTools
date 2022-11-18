using UnityObject = UnityEngine.Object;

namespace UnityEngine
{
    public static class InstantiateExtensions
    {
        /// <summary>
        /// Instantiates unity object of defined type.
        /// </summary>
        public static T Install<T>(this T self) where T : UnityObject
        {
            return UnityObject.Instantiate(self);
        }

        /// <summary>
        /// Instantiates gameobject as a child of the specified parent.
        /// </summary>
        public static GameObject Install(this GameObject self, Transform parent, bool worldPositionStays)
        {
            return UnityObject.Instantiate(self, parent, worldPositionStays);
        }

        /// <summary>
        /// Instantiates gameobject as a child with default local position and rotation.
        /// </summary>
        public static GameObject Install(this GameObject self, Transform parent)
        {
            return UnityObject.Instantiate(self, parent, false);
        }

        /// <summary>
        /// Instantiates gameobject as a child of the specified parent.
        /// </summary>
        public static T Install<T>(this T self, Transform parent, bool worldPositionStays) where T : Component
        {
            return UnityObject.Instantiate(self, parent, worldPositionStays);
        }

        /// <summary>
        /// Instantiates defined component as a child with default local position and rotation.
        /// </summary>
        public static T Install<T>(this T self, Transform parent) where T : Component
        {
            return UnityObject.Instantiate(self, parent, false);
        }

        /// <summary>
        /// Instantiates gameobject to the specified position.
        /// </summary>
        public static GameObject Install(this GameObject self, in Vector3 position)
        {
            return UnityObject.Instantiate(self, position, self.transform.rotation);
        }

        /// <summary>
        /// Instantiates gameobject to the specified position with the specified rotation.
        /// </summary>
        public static GameObject Install(this GameObject self, in Vector3 position, in Quaternion rotation)
        {
            return UnityObject.Instantiate(self, position, rotation);
        }

        /// <summary>
        /// Instantiates gameobject as a child with the specified position and rotation.
        /// </summary>
        /// <param name="local">If true targetPos and targetRot are considered as local, otherwise as world.</param>
        public static GameObject Install(this GameObject self, Transform parent, Vector3 targetPos, Quaternion targetRot, bool local = true)
        {
            if (local)
            {
                targetPos = parent.TransformPoint(targetPos);
                targetRot = parent.rotation * targetRot;
            }

            return UnityObject.Instantiate(self, targetPos, targetRot, parent);
        }

        /// <summary>
        /// Instantiates defined component to specified position.
        /// </summary>
        public static T Install<T>(this T self, in Vector3 position) where T : Component
        {
            return UnityObject.Instantiate(self, position, self.transform.rotation);
        }

        /// <summary>
        /// Instantiates defined component to specified position.
        /// </summary>
        public static Transform Install(this Transform self, in Vector3 position)
        {
            return UnityObject.Instantiate(self, position, self.rotation);
        }

        /// <summary>
        /// Instantiates defined component to specified position with specified rotation.
        /// </summary>
        public static T Install<T>(this T self, in Vector3 position, in Quaternion rotation) where T : Component
        {
            return UnityObject.Instantiate(self, position, rotation);
        }

        /// <summary>
        /// Instantiates gameobject as a child with the specified position and rotation.
        /// </summary>
        /// <param name="local">If true targetPos and targetRot are considered as local, otherwise as world.</param>
        public static T Install<T>(this T self, Transform parent, Vector3 targetPos, Quaternion targetRot, bool local = true) where T : Component
        {
            if (local)
            {
                targetPos = parent.TransformPoint(targetPos);
                targetRot = parent.rotation * targetRot;
            }

            return UnityObject.Instantiate(self, targetPos, targetRot, parent);
        }
    }
}
