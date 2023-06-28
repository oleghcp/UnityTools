using UnityEditor;
using UnityEngine;
using UnityUtility.Async;
using UnityUtilityEditor.Engine;

namespace UnityUtilityEditor.Inspectors.AsyncSystem
{
    [CustomEditor(typeof(TaskRunner))]
    internal class TaskRunnerEditor : Editor<TaskRunner>
    {
        private long _id;
        private bool _paused;
        private string _startPoint = string.Empty;

        private void Awake()
        {
            Init();

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

            if (target.IsWaiting)
                EditorGUILayout.LabelField($"Task {target.Id} (Waiting)", EditorStyles.boldLabel);
            else
                EditorGUILayout.LabelField($"Task {target.Id}", EditorStyles.boldLabel);

            EditorGUILayout.LabelField(_startPoint);
        }

        private void Init()
        {
            _id = target.Id;
            _paused = target.IsWaiting;
            _startPoint = GetStackTraceStartPoint(target.StackTrace);
        }

        private void Update()
        {
            if (_id != target.Id || _paused != target.IsWaiting)
            {
                Init();
                Repaint();
            }
        }

        private string GetStackTraceStartPoint(string stackTrace)
        {
            int index = stackTrace.LastIndexOf(" in ");
            return stackTrace.Substring(index + 4 + Application.dataPath.Length);
        }
    }
}
