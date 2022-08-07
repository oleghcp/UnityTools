using UnityEditor;
using UnityEngine;
using UnityUtility;
using UnityUtility.Async;

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
            _paused = target.IsPaused;

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

            EditorGUILayout.LabelField($"Task ID: {target.Id}");
            GUI.color = target.IsPaused ? Colours.Blue : Colours.Lime;
            EditorGUILayout.LabelField(target.IsPaused ? "Paused" : "Running");
            GUI.color = Colours.White;
        }

        private void Update()
        {
            if (_id != target.Id || _paused != target.IsPaused)
            {
                _id = target.Id;
                _paused = target.IsPaused;
                Repaint();
            }
        }
    }
}
