using System;
using UnityEditor;
using UnityEngine;
using UnityUtility.Sound.SoundStuff;

namespace UnityUtilityEditor.CustomEditors.Sound
{
    [CustomEditor(typeof(AudioInfo), true)]
    internal class AudioInfoEditor : Editor<AudioInfo>
    {
        private string _lengthInfo;

        public override void OnInspectorGUI()
        {
            AudioSource audioSource = target.AudioSource;

            if (audioSource != null && (audioSource.isPlaying || audioSource.time != 0f))
            {
                InitLengthInfo();

                GUILayout.Space(10f);

                EditorGUILayout.LabelField(target.ClipName);
                EditorGUILayout.LabelField(_lengthInfo);

                GUI.enabled = false;
                EditorGUILayout.Slider(audioSource.time, 0f, audioSource.clip.length);
                GUI.enabled = true;

                if (GUIExt.DrawCenterButton("Stop", 30f, 150f))
                    target.Stop();

                GUILayout.Space(10f);
            }
            else
            {
                _lengthInfo = null;
            }
        }

        private void InitLengthInfo()
        {
            if (_lengthInfo == null)
            {
                AudioClip clip = target.AudioSource.clip;
                if (clip != null)
                    _lengthInfo = "Length: " + TimeSpan.FromSeconds(clip.length).ToString(@"hh\:mm\:ss\:fff");
            }
        }
    }
}
