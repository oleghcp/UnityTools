using UnityEditor;
using UnityEngine;
using UnityUtility.MathExt;
using UnityUtility.Sound;
using UnityObject = UnityEngine.Object;

namespace UnityUtilityEditor.CustomEditors.Sound
{
    [CustomEditor(typeof(SoundsPreset))]
    internal class SoundsPresetEditor : AudioPresetEditor
    {
        protected override void DrawTableHeader()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Name", EditorStyles.boldLabel, GUILayout.Width(100f));
                EditorGUILayout.LabelField("Vol", EditorStyles.boldLabel, GUILayout.Width(40f));
                EditorGUILayout.LabelField("Loop", EditorStyles.boldLabel, GUILayout.Width(35f));
                EditorGUILayout.LabelField("Pitch", EditorStyles.boldLabel, GUILayout.Width(40f));
                GUILayout.Space(5f);
                EditorGUILayout.LabelField("Min/Max Dist", EditorStyles.boldLabel, GUILayout.Width(90f));
            }
        }

        protected override bool DrawTableRow(SerializedProperty nodes, int index)
        {
            SerializedProperty node = nodes.GetArrayElementAtIndex(index);

            SerializedProperty name = node.FindPropertyRelative(SoundsPreset.NamePropName);
            SerializedProperty stats = node.FindPropertyRelative(SoundsPreset.StatsPropName);

            SerializedProperty volume = stats.FindPropertyRelative(nameof(SPreset.Volume));
            SerializedProperty loop = stats.FindPropertyRelative(nameof(SPreset.Looped));
            SerializedProperty pitch = stats.FindPropertyRelative(nameof(SPreset.Pitch));
            SerializedProperty minDist = stats.FindPropertyRelative(nameof(SPreset.MinDist));
            SerializedProperty maxDist = stats.FindPropertyRelative(nameof(SPreset.MaxDist));

            bool needBreak = false;

            using (new EditorGUILayout.HorizontalScope())
            {
                name.stringValue = EditorGUILayout.TextField(name.stringValue, GUILayout.Width(100f));
                volume.floatValue = EditorGUILayout.FloatField(volume.floatValue, GUILayout.Width(40f)).Clamp01();
                GUILayout.Space(10f);
                loop.boolValue = EditorGUILayout.Toggle(loop.boolValue, GUILayout.Width(25f));
                pitch.floatValue = EditorGUILayout.FloatField(pitch.floatValue, GUILayout.Width(40f)).Clamp(0f, 3f);
                GUILayout.Space(5f);
                minDist.floatValue = EditorGUILayout.FloatField(minDist.floatValue, GUILayout.Width(40f)).Clamp(0f, maxDist.floatValue - 1f);
                maxDist.floatValue = EditorGUILayout.FloatField(maxDist.floatValue, GUILayout.Width(40f)).CutBefore(minDist.floatValue + 1f);
                GUILayout.Space(5f);
                if (GUILayout.Button("X", GUILayout.Height(17f), GUILayout.Width(EditorGuiUtility.SmallButtonWidth)))
                {
                    nodes.DeleteArrayElementAtIndex(index);
                    needBreak = true;
                }
            }

            return needBreak;
        }

        protected override void AddObject(SerializedProperty nodes, UnityObject obj)
        {
            SerializedProperty node = nodes.PlaceArrayElement();

            node.FindPropertyRelative(SoundsPreset.NamePropName).stringValue = obj != null ? obj.name : string.Empty;

            SerializedProperty stats = node.FindPropertyRelative(SoundsPreset.StatsPropName);

            stats.FindPropertyRelative(nameof(SPreset.Looped)).boolValue = false;
            stats.FindPropertyRelative(nameof(SPreset.Volume)).floatValue = 1f;
            stats.FindPropertyRelative(nameof(SPreset.Pitch)).floatValue = 1f;
            stats.FindPropertyRelative(nameof(SPreset.MinDist)).floatValue = 1f;
            stats.FindPropertyRelative(nameof(SPreset.MaxDist)).floatValue = 500f;
        }
    }
}
