using OlegHcpEditor.Engine;
using UnityEditor;
using UnityEngine;

namespace OlegHcpEditor.Inspectors
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(ScriptableObject), true)]
    internal class ScriptableObjectEditor : BaseEditor<ScriptableObject>
    {

    }
}
