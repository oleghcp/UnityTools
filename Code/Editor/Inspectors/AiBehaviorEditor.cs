#if UNITY_2019_3_OR_NEWER
using UnityEditor;
using UnityEngine;
using UnityUtility;
using UnityUtility.AiSimulation;
using UnityUtilityEditor.Engine;

namespace UnityUtilityEditor.Inspectors
{
    [CustomEditor(typeof(AiBehavior))]
    public class AiBehaviorEditor : Editor<AiBehavior>
    {
        private const string NONE_WORD = "None";
        private const string SPACE = " ";

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (!EditorApplication.isPlaying || target.BehaviorSetInstance == null)
                return;

            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUI.color = Colours.Lime;
            EditorGUILayout.LabelField("Current:", GetStateName(target.BehaviorSetInstance.CurrentState));
            GUI.color = Colours.White;
            EditorGUILayout.LabelField(SPACE, "↑");
            EditorGUILayout.LabelField("Previous:", GetStateName(target.BehaviorSetInstance.PrevState));
            EditorGUILayout.EndVertical();
        }

        private string GetStateName(BehaviorState state)
        {
            return state == null ? NONE_WORD : state.GetType().Name;
        }
    }
}
#endif
