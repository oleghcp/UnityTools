#if INCLUDE_AUDIO
using System;
using OlegHcp.Sound.SoundStuff;
using UnityEditor;
using UnityEngine;

namespace OlegHcpEditor.Inspectors.Sound
{
    [CustomEditor(typeof(AudioInfo), true)]
    internal class AudioInfoEditor : Editor<AudioInfo>
    {
        private string _lengthInfo;

        private void OnEnable()
        {
            EditorApplication.update += Repaint;
        }

        private void OnDisable()
        {
            EditorApplication.update -= Repaint;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

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

                using (new EditorGuiLayout.HorizontalCenteringScope())
                {
                    if (GUILayout.Button("Stop", GUILayout.Height(30f), GUILayout.MinWidth(150f)))
                        target.Stop();
                }

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
                    _lengthInfo = "Length: " + TimeSpan.FromSeconds(clip.length).ToString(@"hh\:mm\:ss\.fff");
            }
        }
    }
}
#endif
