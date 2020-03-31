using UnityUtility.Async;
using UnityEditor;
using UnityEngine;

namespace UUEditor
{
    [CustomEditor(typeof(RoutineRunner))]
    internal class RoutineRunnerEditor : Editor
    {
        private RoutineRunner m_target;

        private void Awake()
        {
            m_target = target as RoutineRunner;
        }

        public override void OnInspectorGUI()
        {
            if (m_target.Id != 0)
            {
                GUILayout.Space(10f);
                EditorGUILayout.LabelField("Task ID: " + m_target.Id.ToString());
                EditorGUILayout.LabelField("Status: " + (m_target.IsPaused ? "Paused" : "Running"));
                Repaint();
            }
        }
    }
}
