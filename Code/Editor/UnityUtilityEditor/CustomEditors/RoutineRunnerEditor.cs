using UnityEditor;
using UnityUtility.Async;

namespace UnityUtilityEditor.CustomEditors
{
    [CustomEditor(typeof(RoutineRunner))]
    internal class RoutineRunnerEditor : Editor<RoutineRunner>
    {
        private void Awake()
        {
            EditorApplication.update += Repaint;
        }

        private void OnDestroy()
        {
            EditorApplication.update -= Repaint;
        }

        public override void OnInspectorGUI()
        {
            if (target.Id == 0L)
            {
                EditorGUILayout.HelpBox("Inactive", MessageType.Info);
                return;
            }

            EditorGUILayout.LabelField("Task ID: " + target.Id.ToString());
            EditorGUILayout.LabelField("Status: " + (target.IsPaused ? "Paused" : "Running"));
        }
    }
}
