using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace UnityUtility.Engine
{
    public static class InstantiationExtensions
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
            return UnityObject.Instantiate(self, position, self.transform.localRotation);
        }

        /// <summary>
        /// Instantiates gameobject to the specified position with the specified rotation.
        /// </summary>
        public static GameObject Install(this GameObject self, in Vector3 position, in Quaternion rotation)
        {
            return UnityObject.Instantiate(self, position, rotation);
        }

        /// <summary>
        /// Instantiates defined component to specified position.
        /// </summary>
        public static Transform Install(this Transform self, in Vector3 position)
        {
            return UnityObject.Instantiate(self, position, self.localRotation);
        }

        /// <summary>
        /// Instantiates defined component to specified position.
        /// </summary>
        public static T Install<T>(this T self, in Vector3 position) where T : Component
        {
            return UnityObject.Instantiate(self, position, self.transform.localRotation);
        }

        /// <summary>
        /// Instantiates defined component to specified position with specified rotation.
        /// </summary>
        public static T Install<T>(this T self, in Vector3 position, in Quaternion rotation) where T : Component
        {
            return UnityObject.Instantiate(self, position, rotation);
        }

        /// <summary>
        /// Instantiates game object as a child with the specified position and rotation.
        /// </summary>
        /// <param name="local">If true targetPos and targetRot are considered as local, otherwise as world.</param>
        public static GameObject Install(this GameObject self, Transform parent, Vector3 targetPos, Quaternion targetRot, bool local)
        {
            if (local)
            {
                targetPos = parent.TransformPoint(targetPos);
                targetRot = parent.rotation * targetRot;
            }

            return UnityObject.Instantiate(self, targetPos, targetRot, parent);
        }

        /// <summary>
        /// Instantiates object as a child with the specified position and rotation.
        /// </summary>
        /// <param name="local">If true targetPos and targetRot are considered as local, otherwise as world.</param>
        public static T Install<T>(this T self, Transform parent, Vector3 targetPos, Quaternion targetRot, bool local) where T : Component
        {
            if (local)
            {
                targetPos = parent.TransformPoint(targetPos);
                targetRot = parent.rotation * targetRot;
            }

            return UnityObject.Instantiate(self, targetPos, targetRot, parent);
        }

#if UNITY_2019_3_OR_NEWER
        /// <summary>
        /// Instantiates asset.
        /// </summary>
        public static T Install<T>(this LazyLoadReference<T> self) where T : UnityObject
        {
            return UnityObject.Instantiate(self.asset);
        }

        /// <summary>
        /// Instantiates gameobject asset as a child of the specified parent.
        /// </summary>
        public static GameObject Install(this LazyLoadReference<GameObject> self, Transform parent, bool worldPositionStays)
        {
            return UnityObject.Instantiate(self.asset, parent, worldPositionStays);
        }

        /// <summary>
        /// Instantiates gameobject asset as a child with default local position and rotation.
        /// </summary>
        public static GameObject Install(this LazyLoadReference<GameObject> self, Transform parent)
        {
            return UnityObject.Instantiate(self.asset, parent, false);
        }

        /// <summary>
        /// Instantiates gameobject asset as a child of the specified parent.
        /// </summary>
        public static T Install<T>(this LazyLoadReference<T> self, Transform parent, bool worldPositionStays) where T : Component
        {
            return UnityObject.Instantiate(self.asset, parent, worldPositionStays);
        }

        /// <summary>
        /// Instantiates asset as a child with default local position and rotation.
        /// </summary>
        public static T Install<T>(this LazyLoadReference<T> self, Transform parent) where T : Component
        {
            return UnityObject.Instantiate(self.asset, parent, false);
        }

        /// <summary>
        /// Instantiates gameobject asset to the specified position.
        /// </summary>
        public static GameObject Install(this LazyLoadReference<GameObject> self, in Vector3 position)
        {
            GameObject asset = self.asset;
            return UnityObject.Instantiate(asset, position, asset.transform.localRotation);
        }

        /// <summary>
        /// Instantiates gameobject asset to the specified position with the specified rotation.
        /// </summary>
        public static GameObject Install(this LazyLoadReference<GameObject> self, in Vector3 position, in Quaternion rotation)
        {
            return UnityObject.Instantiate(self.asset, position, rotation);
        }

        /// <summary>
        /// Instantiates asset to specified position.
        /// </summary>
        public static Transform Install(this LazyLoadReference<Transform> self, in Vector3 position)
        {
            Transform asset = self.asset;
            return UnityObject.Instantiate(asset, position, asset.localRotation);
        }

        /// <summary>
        /// Instantiates asset to specified position.
        /// </summary>
        public static T Install<T>(this LazyLoadReference<T> self, in Vector3 position) where T : Component
        {
            T asset = self.asset;
            return UnityObject.Instantiate(asset, position, asset.transform.localRotation);
        }

        /// <summary>
        /// Instantiates asset to specified position with specified rotation.
        /// </summary>
        public static T Install<T>(this LazyLoadReference<T> self, in Vector3 position, in Quaternion rotation) where T : Component
        {
            return UnityObject.Instantiate(self.asset, position, rotation);
        }

        /// <summary>
        /// Instantiates gameobject asset as a child with the specified position and rotation.
        /// </summary>
        /// <param name="local">If true targetPos and targetRot are considered as local, otherwise as world.</param>
        public static GameObject Install(this LazyLoadReference<GameObject> self, Transform parent, Vector3 targetPos, Quaternion targetRot, bool local)
        {
            return Install(self.asset, parent, targetPos, targetRot, local);
        }

        /// <summary>
        /// Instantiates gameobject asset as a child with the specified position and rotation.
        /// </summary>
        /// <param name="local">If true targetPos and targetRot are considered as local, otherwise as world.</param>
        public static T Install<T>(this LazyLoadReference<T> self, Transform parent, Vector3 targetPos, Quaternion targetRot, bool local) where T : Component
        {
            return Install(self.asset, parent, targetPos, targetRot, local);
        }
#endif
    }
}
