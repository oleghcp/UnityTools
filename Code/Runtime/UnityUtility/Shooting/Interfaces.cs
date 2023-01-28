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
        void OnHit(in RaycastHit hitInfo, in Vector3 velocity, float speed);
        void OnTimeOut();
        void OnReflect(in RaycastHit hitInfo, in Vector3 previousVelocity, float previousSpeed);
        void PreUpdate();
        void PostUpdate();
    }
#endif

#if INCLUDE_PHYSICS_2D
    public interface IProjectile2DEventListener
    {
        void OnHit(in RaycastHit2D hitInfo, in Vector2 velocity, float speed);
        void OnTimeOut();
        void OnReflect(in RaycastHit2D hitInfo, in Vector2 previousVelocity, float previousSpeed);
        void PreUpdate();
        void PostUpdate();
    }
#endif
}
#endif
