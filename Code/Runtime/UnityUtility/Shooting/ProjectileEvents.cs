using System;
using UnityEngine;
using UnityEngine.Events;

#if UNITY_2019_3_OR_NEWER && (INCLUDE_PHYSICS || INCLUDE_PHYSICS_2D)
namespace UnityUtility.Shooting
{
#if INCLUDE_PHYSICS
    [Serializable]
    public sealed class ProjectileEvents
    {
        [field: SerializeField]
        public UnityEvent<RaycastHit> OnHit { get; private set; }
        [field: SerializeField]
        public UnityEvent OnTimeOut { get; private set; }
        [field: SerializeField]
        public UnityEvent<RaycastHit> OnReflect { get; private set; }
    }
#endif

#if INCLUDE_PHYSICS_2D
    [Serializable]
    public sealed class ProjectileEvents2D
    {
        [field: SerializeField]
        public UnityEvent<RaycastHit2D> OnHit { get; private set; }
        [field: SerializeField]
        public UnityEvent OnTimeOut { get; private set; }
        [field: SerializeField]
        public UnityEvent<RaycastHit2D> OnReflect { get; private set; }
    }
#endif
}
#endif
