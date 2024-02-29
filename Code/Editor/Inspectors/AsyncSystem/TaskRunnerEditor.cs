using OlegHcp;
using OlegHcp.Async;
using OlegHcp.IO;
using OlegHcpEditor.Engine;
using OlegHcpEditor.Window;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace OlegHcpEditor.Inspectors.AsyncSystem
{
    [CustomEditor(typeof(TaskRunner))]
    internal class TaskRunnerEditor : Editor<TaskRunner>
    {
        private static GUIStyle _hyperLinkStyle;
        private static string _inactiveLabel = "Inactive";
        private static string _stackTraceButtonLabel = "Stack Trace";
        private static GUILayoutOption[] _buttonOptions = new GUILayoutOption[] { GUILayout.Height(EditorGUIUtility.singleLineHeight) };

        private long _id;
        private string _startPoint;

        private void Awake()
        {
            if (_hyperLinkStyle == null)
            {
                _hyperLinkStyle = new GUIStyle(EditorStyles.label);
                _hyperLinkStyle.normal.textColor = Colours.Sky;
                _hyperLinkStyle.hover.textColor = Colours.Sky;
                _hyperLinkStyle.active.textColor = Colours.Lime;
            }
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
                EditorGUILayout.HelpBox(_inactiveLabel, MessageType.Info);
                return;
            }

            EditorGUILayout.BeginHorizontal();

            GUILayout.Label($"Task {target.Id}", EditorStyles.boldLabel);

            if (GUILayout.Button(_stackTraceButtonLabel, _buttonOptions))
                StackTraceWindow.Create(target.StackTrace);

            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button(_startPoint, _hyperLinkStyle))
                OpenCode();
        }

        private void Init()
        {
            _id = target.Id;
            _startPoint = _id == 0L ? string.Empty
                                    : GetStartLine(target.StackTrace, Application.dataPath.Length);
        }

        private void Update()
        {
            if (_id != target.Id)
            {
                Init();
                Repaint();
            }
        }

        private static string GetStartLine(string stackTrace, int offset = 0)
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
                    return targetLine.Substring(index + 4 + offset);
                }
            }

            return string.Empty;
        }

        private void OpenCode()
        {
            int offset = PathUtility.GetParentPath(Application.dataPath).Length + 1;
            string startPoint = GetStartLine(target.StackTrace, offset);

            int index = startPoint.LastIndexOf(':');
            string filePath = startPoint.Substring(0, index);
            string lineNumber = startPoint.Substring(index + 1);

            UnityObject scriptAsst = AssetDatabase.LoadAssetAtPath(filePath, typeof(UnityObject));

            EditorUtilityExt.OpenCsProject();
            AssetDatabase.OpenAsset(scriptAsst, int.Parse(lineNumber));
        }
    }
}
