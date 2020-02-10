using UU.Async;
using UnityEditor;
using UnityEngine;

namespace UUEditor
{
    [CustomEditor(typeof(RoutineExecutor))]
    internal class RoutineExecutorEditor : Editor
    {
        private RoutineExecutor m_target;

        private void Awake()
        {
            m_target = target as RoutineExecutor;
        }

        public override void OnInspectorGUI()
        {
            if (m_target.ID != 0)
            {
                GUILayout.Space(10f);
                EditorGUILayout.LabelField("Task ID: " + m_target.ID.ToString());
                EditorGUILayout.LabelField("Status: " + (m_target.IsPaused ? "Paused" : "Running"));
                Repaint();
            }
        }
    }
}
