#if INCLUDE_PHYSICS || INCLUDE_PHYSICS_2D
using UnityEngine;

namespace UnityUtility.Shooting
{
    public interface ITimeProvider
    {
        float GetDeltaTime();
    }

    public interface IGravityProvider
    {
        Vector3 GetGravity();
    }

    public interface IGravityProvider2D
    {
        Vector2 GetGravity();
    }

    public interface IRotationProvider
    {
        Quaternion GetRotation();
    }

#if INCLUDE_PHYSICS
    public interface IProjectileEventListener
    {
        void OnHit(in RaycastHit hitInfo);
        void OnTimeOut();
        void OnReflect(in RaycastHit hitInfo);
        void PreUpdate();
        void PostUpdate();
    }
#endif

#if INCLUDE_PHYSICS_2D
    public interface IProjectile2DEventListener
    {
        void OnHit(in RaycastHit2D hitInfo);
        void OnTimeOut();
        void OnReflect(in RaycastHit2D hitInfo);
        void PreUpdate();
        void PostUpdate();
    }
#endif
}
#endif
