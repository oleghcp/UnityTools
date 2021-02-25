using UnityEditor;
using UnityEngine;
using UnityUtility.MathExt;
using UnityUtility.Sound;
using UnityObject = UnityEngine.Object;

namespace UnityUtilityEditor.CustomEditors
{
    [CustomEditor(typeof(MusicPreset))]
    internal class MusPresetEditor : SoundsPresetEditor
    {
        protected override void DrawTableHeader()
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Name", EditorStyles.boldLabel, GUILayout.Width(100f));
                EditorGUILayout.LabelField("Vol", EditorStyles.boldLabel, GUILayout.Width(40f));
                EditorGUILayout.LabelField("Loop", EditorStyles.boldLabel, GUILayout.Width(35f));
                EditorGUILayout.LabelField("Pitch", EditorStyles.boldLabel, GUILayout.Width(40f));
                EditorGUILayout.LabelField("Time", EditorStyles.boldLabel, GUILayout.Width(40f));
                EditorGUILayout.LabelField("Delay", EditorStyles.boldLabel, GUILayout.Width(40f));
                EditorGUILayout.LabelField("Rise", EditorStyles.boldLabel, GUILayout.Width(30f));
                EditorGUILayout.LabelField("Duration", EditorStyles.boldLabel, GUILayout.Width(60f));
            }
            EditorGUILayout.EndHorizontal();
        }

        protected override bool DrawTableRow(SerializedProperty nodes, int index)
        {
            SerializedProperty node = nodes.GetArrayElementAtIndex(index);

            SerializedProperty name = node.FindPropertyRelative(MusicPreset.NameProp);
            SerializedProperty stats = node.FindPropertyRelative(MusicPreset.StatsProp);

            SerializedProperty volume = stats.FindPropertyRelative(nameof(MPreset.Volume));
            SerializedProperty loop = stats.FindPropertyRelative(nameof(MPreset.Looped));
            SerializedProperty pitch = stats.FindPropertyRelative(nameof(MPreset.Pitch));
            SerializedProperty time = stats.FindPropertyRelative(nameof(MPreset.StartTime));
            SerializedProperty delay = stats.FindPropertyRelative(nameof(MPreset.StartDelay));
            SerializedProperty rising = stats.FindPropertyRelative(nameof(MPreset.Rising));
            SerializedProperty intensity = stats.FindPropertyRelative(nameof(MPreset.RisingDur));

            bool needBreak = false;

            EditorGUILayout.BeginHorizontal();
            {
                name.stringValue = EditorGUILayout.TextField(name.stringValue, GUILayout.Width(100f));
                volume.floatValue = EditorGUILayout.FloatField(volume.floatValue, GUILayout.Width(40f)).Clamp01();
                GUILayout.Space(10f);
                loop.boolValue = EditorGUILayout.Toggle(loop.boolValue, GUILayout.Width(25f));
                pitch.floatValue = EditorGUILayout.FloatField(pitch.floatValue, GUILayout.Width(40f)).Clamp(0f, 3f);
                time.floatValue = EditorGUILayout.FloatField(time.floatValue, GUILayout.Width(40f)).CutBefore(0f);
                delay.floatValue = EditorGUILayout.FloatField(delay.floatValue, GUILayout.Width(40f)).CutBefore(0f);
                GUILayout.Space(10f);
                rising.boolValue = EditorGUILayout.Toggle(rising.boolValue, GUILayout.Width(25f));
                if (!rising.boolValue)
                    GUI.enabled = false;
                intensity.floatValue = EditorGUILayout.FloatField(intensity.floatValue, GUILayout.Width(40f)).CutBefore(0f);
                GUI.enabled = true;
                if (GUILayout.Button("X", GUILayout.Height(15f), GUILayout.Width(20f)))
                {
                    nodes.DeleteArrayElementAtIndex(index);
                    needBreak = true;
                }
            }
            EditorGUILayout.EndHorizontal();

            return needBreak;
        }

        protected override void AddObject(SerializedProperty nodes, UnityObject obj)
        {
            SerializedProperty node = nodes.PlaceArrayElement();

            node.FindPropertyRelative(MusicPreset.NameProp).stringValue = obj != null ? obj.name : string.Empty;

            SerializedProperty stats = node.FindPropertyRelative(MusicPreset.StatsProp);

            stats.FindPropertyRelative(nameof(MPreset.Volume)).floatValue = 1f;
            stats.FindPropertyRelative(nameof(MPreset.Looped)).boolValue = true;
            stats.FindPropertyRelative(nameof(MPreset.Pitch)).floatValue = 1f;
            stats.FindPropertyRelative(nameof(MPreset.StartTime)).floatValue = 0f;
            stats.FindPropertyRelative(nameof(MPreset.StartDelay)).floatValue = 0f;
            stats.FindPropertyRelative(nameof(MPreset.Rising)).boolValue = false;
            stats.FindPropertyRelative(nameof(MPreset.RisingDur)).floatValue = 1f;
        }
    }
}
