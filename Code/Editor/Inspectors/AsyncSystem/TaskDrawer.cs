using OlegHcp.Async;
using OlegHcp.IO;
using OlegHcpEditor.Window;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace OlegHcpEditor.Inspectors.AsyncSystem
{
    internal class TaskDrawer
    {
        private RoutineIterator _iterator;
        private string _startPoint;

        private static GUIStyle _hyperLinkStyle;
        private static string _stackTraceButtonLabel;
        private static GUILayoutOption[] _buttonOptions;

        public TaskDrawer(RoutineIterator iterator, string stackTraceButtonLabel, GUIStyle hyperLinkStyle, GUILayoutOption[] buttonOptions)
        {
            _iterator = iterator;
            _startPoint = GetStartLine(iterator.StackTrace, Application.dataPath.Length);

            _stackTraceButtonLabel = stackTraceButtonLabel;
            _hyperLinkStyle = hyperLinkStyle;
            _buttonOptions = buttonOptions;
        }

        public void Draw()
        {
            EditorGUILayout.BeginHorizontal();

            GUILayout.Label($"Task #{_iterator.Id}", EditorStyles.boldLabel);

            if (GUILayout.Button(_stackTraceButtonLabel, _buttonOptions))
                StackTraceWindow.Create(_iterator.StackTrace);

            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button(_startPoint, _hyperLinkStyle))
                OpenCode(_iterator.StackTrace);
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

        private static void OpenCode(string stackTrace)
        {
            int offset = PathUtility.GetParentPath(Application.dataPath).Length + 1;
            string startPoint = GetStartLine(stackTrace, offset);

            int index = startPoint.LastIndexOf(':');
            string filePath = startPoint.Substring(0, index);
            string lineNumber = startPoint.Substring(index + 1);

            UnityObject scriptAsset = AssetDatabase.LoadAssetAtPath(filePath, typeof(UnityObject));

            EditorUtilityExt.OpenCsProject();
            AssetDatabase.OpenAsset(scriptAsset, int.Parse(lineNumber));
        }
    }
}
