using UnityEditor;
using UnityUtility.Async;

namespace UnityUtilityEditor.CustomEditors.AsyncSystem
{
    [CustomEditor(typeof(RoutineRunner))]
    internal class RoutineRunnerEditor : Editor<RoutineRunner>
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
            EditorGUILayout.LabelField("Status: " + (target.IsPaused ? "Paused" : "Running"));
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
