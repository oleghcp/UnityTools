using System;
using UnityEngine;
using UnityEngine.Events;

#if UNITY_2019_3_OR_NEWER
namespace UnityUtility.Shooting
{
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
