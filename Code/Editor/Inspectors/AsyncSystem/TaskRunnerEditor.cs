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
        private string _startPoint;

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

            EditorGUILayout.LabelField($"Task {target.Id}", EditorStyles.boldLabel);
            EditorGUILayout.LabelField(_startPoint);
        }

        private void Init()
        {
            _id = target.Id;
            _startPoint = GetStartLine(target.StackTrace);
        }

        private void Update()
        {
            if (_id != target.Id)
            {
                Init();
                Repaint();
            }
        }

        private string GetStartLine(string stackTrace)
        {
            string[] lines = stackTrace.Split('\n');
            int index = -1;

            for (int i = lines.Length - 1; i >= 0; i--)
            {
                if (lines[i].Contains(nameof(TaskSystem.StartAsync)))
                {
                    index = i + 1;
                    break;
                }
            }

            if (index < 0)
                return string.Empty;

            string targetLine = lines[index];
            index = targetLine.LastIndexOf(" in ");
            return targetLine.Substring(index + 4 + Application.dataPath.Length);
        }
    }
}
