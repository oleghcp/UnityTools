#if INCLUDE_PHYSICS || INCLUDE_PHYSICS_2D
using UnityEngine;

namespace OlegHcp.Shooting
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
        void OnHitFinal(in RaycastHit hitInfo, in Vector3 velocity, float speed);
        void OnHitModified(in RaycastHit hitInfo, float previousSpeed, in Vector3 previousDirection, HitReactionType reaction);
        void OnTimeOut();
        void PreUpdate(bool isPlaying);
        void PostUpdate(bool isPlaying);
    }
#endif

#if INCLUDE_PHYSICS_2D
    public interface IProjectile2DEventListener
    {
        void OnHitFinal(in RaycastHit2D hitInfo, in Vector2 velocity, float speed);
        void OnHitModified(in RaycastHit2D hitInfo, float previousSpeed, in Vector2 previousDirection, HitReactionType reaction);
        void OnTimeOut();
        void PreUpdate(bool isPlaying);
        void PostUpdate(bool isPlaying);
    }
#endif
}
#endif
