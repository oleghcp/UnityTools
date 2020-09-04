using UnityEditor;
using UnityUtility.Async;

namespace UnityUtilityEditor.CustomEditors
{
    [CustomEditor(typeof(RoutineRunner))]
    internal class RoutineRunnerEditor : Editor
    {
        private RoutineRunner m_target;

        private void Awake()
        {
            m_target = target as RoutineRunner;
            EditorApplication.update += Repaint;
        }

        private void OnDestroy()
        {
            EditorApplication.update -= Repaint;
        }

        public override void OnInspectorGUI()
        {
            if (m_target.Id == 0L)
            {
                EditorGUILayout.HelpBox("Inactive", MessageType.Info);
                return;
            }

            EditorGUILayout.LabelField("Task ID: " + m_target.Id.ToString());
            EditorGUILayout.LabelField("Status: " + (m_target.IsPaused ? "Paused" : "Running"));
        }
    }
}
