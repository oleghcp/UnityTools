using OlegHcp.Async;
using UnityEditor;
using UnityEngine;

namespace OlegHcpEditor.Inspectors.AsyncSystem
{
    [CustomEditor(typeof(TaskDispatcher))]
    internal class TaskDispatcherEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGuiLayout.BeginHorizontalCentering(EditorStyles.helpBox, GUILayout.Height(30f));
            EditorGuiLayout.BeginVerticalCentering();
            GUILayout.Label(ObjectNames.NicifyVariableName(target.name), EditorStyles.boldLabel);
            EditorGuiLayout.EndVerticalCentering();
            EditorGuiLayout.EndHorizontalCentering();
        }
    }
}
