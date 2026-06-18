#if UNITY_2020_1_OR_NEWER
#if INCLUDE_ADDRESSABLES
using UnityEngine.AddressableAssets;
#endif
using System.Runtime.CompilerServices;
using System;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace OlegHcp
{
    public enum RefType
    {
        Simple = 0,
        Lazy = 1,
#if INCLUDE_ADDRESSABLES
        Async = 2,
#endif
    }

    [Serializable]
    public struct AssetRef<T> where T : UnityObject
    {
        [SerializeField]
        private T _directRef;
        [SerializeField]
        private LazyLoadReference<T> _lazyRef;
#if INCLUDE_ADDRESSABLES
        [SerializeField]
        private AssetReferenceT<T> _asyncRef;
#endif
        [SerializeField]
        private RefType _type;

#if UNITY_EDITOR
        public static string TypeFieldName => nameof(_type);
        public static string DirectRefFieldName => nameof(_directRef);
        public static string LazyRefFieldName => nameof(_lazyRef);
#if INCLUDE_ADDRESSABLES
        public static string AsyncRefFieldName => nameof(_asyncRef);
#endif
#endif

        public RefType Type => _type;

        public bool Initialized
        {
            get
            {
                switch (_type)
                {
                    case RefType.Simple: return _directRef != null;
                    case RefType.Lazy: return _lazyRef.isSet;
#if INCLUDE_ADDRESSABLES
                    case RefType.Async: return _asyncRef.AssetGUID != null;
#endif
                    default: throw new SwitchExpressionException(_type);
                }
            }
        }

#if UNITY_6000_4_OR_NEWER
        public EntityId InstanceId
#else
        public int InstanceId
#endif
        {
            get
            {
                switch (_type)
                {
                    case RefType.Simple:
#if UNITY_6000_4_OR_NEWER
                        return _directRef.GetEntityId();
#else
                        return _directRef.GetInstanceID();
#endif

                    case RefType.Lazy:
#if UNITY_6000_4_OR_NEWER
                        return _lazyRef.entityId;
#elif UNITY_6000_3_OR_NEWER
                        return _lazyRef.entityId;
#else
                        return _lazyRef.instanceID;
#endif

#if INCLUDE_ADDRESSABLES
                    case RefType.Async: throw new InvalidOperationException("Cannot get instance ID from addressable asset reference.");
#endif
                    default: throw new SwitchExpressionException(_type);
                }
            }
        }

        public T Asset
        {
            get
            {
                switch (_type)
                {
                    case RefType.Simple: return _directRef;
                    case RefType.Lazy: return _lazyRef.asset;
#if INCLUDE_ADDRESSABLES
                    case RefType.Async: throw new InvalidOperationException("Cannot get asset directly from addressable asset reference.");
#endif
                    default: throw new SwitchExpressionException(_type);
                }
            }
        }

#if INCLUDE_ADDRESSABLES
        public AssetReferenceT<T> AsyncRef
        {
            get
            {
                if (_type != RefType.Async)
                    throw new InvalidOperationException("Reference isn't async.");

                return _asyncRef;
            }
        }
#endif
    }
}
#endif
