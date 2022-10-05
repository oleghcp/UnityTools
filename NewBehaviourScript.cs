using UnityEngine;
using UnityUtility;
using UnityUtility.Rng;

public class NewBehaviourScript : MonoBehaviour
{
    private IRng _rng;

    private void Start()
    {
        _rng = new XorshiftRng(1);

        for (int i = 0; i < 100; i++)
        {
            Debug.Log(_rng.Next(0, 100));
        }
    }
}
