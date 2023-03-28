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
            EditorGUILayout.LabelField("Current:", GetStateName(target.CurrentState));
            GUI.color = Colours.White;
            EditorGUILayout.LabelField(Helper.SPACE, "↑");
            EditorGUILayout.LabelField("Previous:", GetStateName(target.PrevState));
            EditorGUILayout.EndVertical();
        }

        private string GetStateName(BehaviorState state)
        {
            return state == null ? NONE_WORD : state.GetType().Name;
        }
    }
}
