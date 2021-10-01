using UnityEngine;

#if UNITY_2019_3_OR_NEWER
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

    public interface IEventListener
    {
        void OnHit(in RaycastHit hitInfo);
        void OnTimeOut();
        void OnReflect(in RaycastHit hitInfo);
    }

    public interface IEventListener2D
    {
        void OnHit(in RaycastHit2D hitInfo);
        void OnTimeOut();
        void OnReflect(in RaycastHit2D hitInfo);
    }
} 
#endif
