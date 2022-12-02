#if UNITY_2020_1_OR_NEWER
using System;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace UnityUtility
{
    [Serializable]
    public struct AssetRef<T> where T : UnityObject
    {
        [SerializeField]
        private T _directRef;
        [SerializeField]
        private LazyLoadReference<T> _lazyRef;
        [SerializeField]
        private bool _lazy;

#if UNITY_EDITOR
        public static string DirectRefFieldName => nameof(_directRef);
        public static string LazyRefFieldName => nameof(_lazyRef);
        public static string FlagFieldName => nameof(_lazy);
#endif

        public int InstanceId => _lazy ? _lazyRef.instanceID : _directRef.GetInstanceID();
        public bool Initialized => _lazy ? _lazyRef.isSet : _directRef != null;
        public bool Lazy => _lazy;
        public T Asset => _lazy ? _lazyRef.asset : _directRef;
    }
}
#endif
