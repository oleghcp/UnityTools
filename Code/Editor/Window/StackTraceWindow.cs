using OlegHcp.Async;
using UnityEditor;
using UnityEngine;

namespace OlegHcpEditor.Window
{
    internal class StackTraceWindow : EditorWindow
    {
        public const string ExtensionClassName = "RoutineExtensions";

        private string _stackTrace;
        private GUILayoutOption[] _options;
        private Vector2 _scrollPosition;

        public static void Create(string stackTrace)
        {
            GetWindow<StackTraceWindow>(true, "Stack Trace").SetUp(stackTrace);
        }

        private void OnGUI()
        {
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, EditorStyles.helpBox);
            EditorGUILayout.SelectableLabel(_stackTrace, _options);
            EditorGUILayout.EndScrollView();
        }

        public void SetUp(string stackTrace)
        {
            int index = stackTrace.IndexOf(ExtensionClassName);
            if (index < 0)
                index = stackTrace.IndexOf(nameof(TaskSystem));

            _stackTrace = stackTrace.Substring(index);
            _stackTrace = _stackTrace.Substring(_stackTrace.IndexOf('\n') + 1);

            Vector2 textSize = EditorStyles.textArea.CalcSize(EditorGuiUtility.TempContent(_stackTrace));
            _options = new GUILayoutOption[]
            {
                GUILayout.Width(textSize.x),
                GUILayout.Height(textSize.y),
            };
        }
    }
}
