using UnityEngine;
using UnityUtility.Shooting;

namespace Project
{
    public class TestMissile : MonoBehaviour, IGravityProvider2D
    {
        private void Awake()
        {
            GetComponent<Projectile2D>().OverrideGravityProvider(this);
        }

        public void OnHit(ProjectileEventType type)
        {
            Debug.Log(type);
        }

        public Vector2 GetGravity()
        {
            Vector2 vector = Vector2.zero - transform.position.XY();
            return vector.normalized * (9.8f / vector.magnitude);
        }
    }
}
