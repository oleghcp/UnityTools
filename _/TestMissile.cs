using UnityEngine;

namespace Project
{
    public class TestMissile : MonoBehaviour
    {
        private void OnHit(string msg)
        {
            Debug.Log(msg);
        }
    }
}
