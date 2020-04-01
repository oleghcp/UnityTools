using UnityUtility.Sound.SoundProviderStuff;
using System;
using UnityEditor;
using UnityEngine;

namespace UnityUtilityEditor.SoundEditors
{
    internal abstract class SoundObjectEditor : Editor
    {
        private SoundObjectInfo m_target;
        private string m_length;

        private void Awake()
        {
            m_target = target as SoundObjectInfo;
        }

        public override void OnInspectorGUI()
        {
            AudioSource audioSource = m_target.AudioSource;

            if (audioSource != null && (audioSource.isPlaying || audioSource.time != 0f))
            {
                f_checkLen();

                GUILayout.Space(10f);
                EditorGUILayout.LabelField(m_target.ClipName);
                EditorGUILayout.LabelField(m_length);

                GUI.enabled = false;
                EditorGUILayout.Slider(audioSource.time, 0f, audioSource.clip.length);
                GUI.enabled = true;

                if (EditorScriptUtility.DrawCenterButton("Stop", 30f, 150f))
                    m_target.Stop();

                GUILayout.Space(10f);
            }
            else
            {
                m_length = null;
            }
        }

        private void f_checkLen()
        {
            if (m_length == null)
            {
                AudioClip clip = m_target.AudioSource.clip;
                if (clip != null)
                    m_length = "Length: " + TimeSpan.FromSeconds(clip.length).ToString(@"hh\:mm\:ss\:fff");
            }
        }
    }
}
