using System.Collections.Generic;
using OlegHcp.Async;
using OlegHcp.CSharp.Collections;
using UnityEditor;
using UnityEngine;

namespace OlegHcpEditor.Inspectors.AsyncSystem
{
    [CustomEditor(typeof(TaskRunner))]
    internal class TaskRunnerEditor : Editor<TaskRunner>
    {
        private static GUIStyle _hyperLinkStyle;
        private static string _stackTraceButtonLabel = "Stack Trace";
        private static GUILayoutOption[] _buttonOptions = new GUILayoutOption[] { GUILayout.Height(EditorGUIUtility.singleLineHeight) };

        private IReadOnlyList<RoutineIterator> _activeTasks;
        private List<TaskDrawer> _taskDrawers = new List<TaskDrawer>();
        private int _version;

        private void OnEnable()
        {
            if (_hyperLinkStyle == null)
                _hyperLinkStyle = EditorStylesExt.HyperLink;

            _activeTasks = target.ActiveTasks;

            EditorApplication.update += Update;
        }

        private void OnDisable()
        {
            EditorApplication.update -= Update;
        }

        public override void OnInspectorGUI()
        {
            UpdateList();

            GUILayout.Label($"Pool: {target.PoolCount}", EditorStyles.boldLabel);

            for (int i = 0; i < _activeTasks.Count; i++)
            {
                if (_activeTasks[i] == null)
                {
                    _taskDrawers[i] = default;
                    continue;
                }

                EditorGUILayout.Space();

                if (_taskDrawers[i].IsDead)
                    _taskDrawers[i] = new TaskDrawer(_activeTasks[i]);

                _taskDrawers[i].Draw(_hyperLinkStyle, _stackTraceButtonLabel, _buttonOptions);
            }
        }

        private void Update()
        {
            if (UpdateList())
                Repaint();
        }

        private bool UpdateList()
        {
            if (_version == target.Version)
                return false;

            _taskDrawers.SetCount(_activeTasks.Count);
            _version = target.Version;

            return true;
        }
    }
}
