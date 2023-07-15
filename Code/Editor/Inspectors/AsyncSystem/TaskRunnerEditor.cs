using UnityEditor;
using UnityEngine;
using UnityUtility;
using UnityUtility.Async;
using UnityUtilityEditor.Engine;
using UnityUtilityEditor.Window;

namespace UnityUtilityEditor.Inspectors.AsyncSystem
{
    [CustomEditor(typeof(TaskRunner))]
    internal class TaskRunnerEditor : Editor<TaskRunner>
    {
        private long _id;
        private string _startPoint;
        private GUIStyle _hyperLinkStyle;

        private void Awake()
        {
            _hyperLinkStyle = new GUIStyle(EditorStyles.label);
            _hyperLinkStyle.normal.textColor = Colours.Sky;
            _hyperLinkStyle.hover.textColor = Colours.Sky;
            _hyperLinkStyle.active.textColor = Colours.Lime;
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

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField($"Task {target.Id}", EditorStyles.boldLabel);

            if (GUILayout.Button("Full Stack Trace", _hyperLinkStyle))
                StackTraceWindow.Create(target.StackTrace);

            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button(_startPoint, _hyperLinkStyle))
            {
                //string args = $"-g \"{filePath}\":{lineIndex}";
                //System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo
                //{
                //    FileName = CodeEditor.CurrentEditorPath,
                //    Arguments = args,
                //    UseShellExecute = false,
                //    CreateNoWindow = true
                //};
                //System.Diagnostics.Process.Start(startInfo);
            }
        }

        private void Init()
        {
            _id = target.Id;
            _startPoint = _id == 0L ? string.Empty
                                    : GetStartLine(target.StackTrace);
        }

        private void Update()
        {
            if (_id != target.Id)
            {
                Init();
                Repaint();
            }
        }

        private static string GetStartLine(string stackTrace)
        {
            string[] lines = stackTrace.Split('\n');

            string searchLine1 = $"{nameof(RoutineExtensions)}.cs";
            string searchLine2 = $"{nameof(TaskSystem)}.cs";

            for (int i = lines.Length - 1; i >= 0; i--)
            {
                if (lines[i].Contains(searchLine1) || lines[i].Contains(searchLine2))
                {
                    string targetLine = lines[i + 1];
                    int index = targetLine.LastIndexOf(" in ");
                    return targetLine.Substring(index + 4 + Application.dataPath.Length);
                }
            }

            return string.Empty;
        }
    }
}
