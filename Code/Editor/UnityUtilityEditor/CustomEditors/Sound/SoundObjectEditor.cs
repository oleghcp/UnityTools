using System;
using UnityEditor;
using UnityEngine;
using UnityUtility.Sound.SoundProviderStuff;

namespace UnityUtilityEditor.CustomEditors.Sound
{
    internal abstract class SoundObjectEditor : Editor
    {
        private SoundObjectInfo _target;
        private string _lengthInfo;

        private void Awake()
        {
            _target = target as SoundObjectInfo;
        }

        public override void OnInspectorGUI()
        {
            AudioSource audioSource = _target.AudioSource;

            if (audioSource != null && (audioSource.isPlaying || audioSource.time != 0f))
            {
                InitLengthInfo();

                GUILayout.Space(10f);
                EditorGUILayout.LabelField(_target.ClipName);
                EditorGUILayout.LabelField(_lengthInfo);

                GUI.enabled = false;
                EditorGUILayout.Slider(audioSource.time, 0f, audioSource.clip.length);
                GUI.enabled = true;

                if (GUIExt.DrawCenterButton("Stop", 30f, 150f))
                    _target.Stop();

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
                AudioClip clip = _target.AudioSource.clip;
                if (clip != null)
                    _lengthInfo = "Length: " + TimeSpan.FromSeconds(clip.length).ToString(@"hh\:mm\:ss\:fff");
            }
        }
    }
}
