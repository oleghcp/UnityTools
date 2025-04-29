using OlegHcp.Async;
using OlegHcp.IO;
using OlegHcpEditor.Window;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace OlegHcpEditor.Inspectors.AsyncSystem
{
    internal readonly struct TaskDrawer
    {
        private readonly RoutineIterator _iterator;
        private readonly string _startPoint;

        public bool IsDead => _iterator == null || _iterator.Id == 0L;

        public TaskDrawer(RoutineIterator iterator)
        {
            _iterator = iterator;
            _startPoint = GetStartLine(iterator.StackTrace, Application.dataPath.Length);
        }

        public void Draw(GUIStyle hyperLinkStyle, string stackTraceButtonLabel, GUILayoutOption[] buttonOptions)
        {


            EditorGUILayout.BeginHorizontal();

            GUILayout.Label($"Task #{_iterator.Id}", EditorStyles.boldLabel);

            if (GUILayout.Button(stackTraceButtonLabel, buttonOptions))
                StackTraceWindow.Create(_iterator.StackTrace);

            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button(_startPoint, hyperLinkStyle))
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
