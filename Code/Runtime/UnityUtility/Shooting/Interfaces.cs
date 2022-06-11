using UnityEngine;

#if UNITY_2019_3_OR_NEWER && (INCLUDE_PHYSICS || INCLUDE_PHYSICS_2D)
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

#if INCLUDE_PHYSICS
    public interface IProjectileEventListener
    {
        void OnHit(in RaycastHit hitInfo);
        void OnTimeOut();
        void OnReflect(in RaycastHit hitInfo);
    }
#endif

#if INCLUDE_PHYSICS_2D
    public interface IProjectile2DEventListener
    {
        void OnHit(in RaycastHit2D hitInfo);
        void OnTimeOut();
        void OnReflect(in RaycastHit2D hitInfo);
    }
#endif
}
#endif
