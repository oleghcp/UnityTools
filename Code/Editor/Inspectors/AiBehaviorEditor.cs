using UnityEditor;
using UnityEngine;
using UnityUtility;
using UnityUtility.AiSimulation;
using UnityUtility.Tools;
using UnityUtilityEditor.Engine;

namespace UnityUtilityEditor.Inspectors
{
    [CustomEditor(typeof(AiBehavior))]
    public class AiBehaviorEditor : Editor<AiBehavior>
    {
        private const string NONE_WORD = "None";

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (!EditorApplication.isPlaying || !target.Initialized)
                return;

            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUI.color = Colours.Lime;
            EditorGUILayout.LabelField("Current:", GetCurrentStateInfo());
            GUI.color = Colours.White;
            EditorGUILayout.LabelField(Helper.Space, "↑");
            EditorGUILayout.LabelField("Previous:", GetPrevStateInfo());
            EditorGUILayout.EndVertical();
        }

        private string GetCurrentStateInfo()
        {
            if (target.CurrentState == null)
                return NONE_WORD;

            return $"{target.CurrentState.GetType().Name} ({target.Status})" ;
        }

        private string GetPrevStateInfo()
        {
            return target.PrevState == null ? NONE_WORD : target.PrevState.GetType().Name;
        }
    }
}
