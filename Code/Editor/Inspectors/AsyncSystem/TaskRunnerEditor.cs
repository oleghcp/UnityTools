using UnityEditor;
using UnityEngine;
using UnityUtility;
using UnityUtility.Async;
using UnityUtilityEditor.Engine;

namespace UnityUtilityEditor.Inspectors.AsyncSystem
{
    [CustomEditor(typeof(TaskRunner))]
    internal class TaskRunnerEditor : Editor<TaskRunner>
    {
        private long _id;
        private bool _paused;

        private void Awake()
        {
            _id = target.Id;
            _paused = target.IsWaiting;

            EditorApplication.update += Update;
        }

        private void OnDestroy()
        {
            EditorApplication.update -= Update;
        }

        public override void OnInspectorGUI()
        {
            if (target.Id == 0L)
            {
                EditorGUILayout.HelpBox("Inactive", MessageType.Info);
                return;
            }

            EditorGUILayout.LabelField($"Task {target.Id}");
            GUI.color = target.IsWaiting ? Colours.Blue : Colours.Lime;
            EditorGUILayout.LabelField(target.IsWaiting ? "Waiting" : "Running");
            GUI.color = Colours.White;
        }

        private void Update()
        {
            if (_id != target.Id || _paused != target.IsWaiting)
            {
                _id = target.Id;
                _paused = target.IsWaiting;
                Repaint();
            }
        }
    }
}
