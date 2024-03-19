using OlegHcpEditor.Engine;
using UnityEditor;
using UnityEngine;

namespace OlegHcpEditor.Inspectors
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(MonoBehaviour), true)]
    internal class MonoBehaviourEditor : Editor<MonoBehaviour>
    {

    }
}
