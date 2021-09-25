using UnityEngine;
using UnityUtility.Shooting;

namespace Project
{
    public class TestMissile : MonoBehaviour
    {
        public void OnHit(ProjectileEventType type)
        {
            Debug.Log(type);
        }
    }
}
