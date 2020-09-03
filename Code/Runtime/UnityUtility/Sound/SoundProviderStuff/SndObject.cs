using UnityEngine;
using System.Collections;
using UnityUtility.Collections;
using UnityUtility.SingleScripts;
using System;
using UnityUtility.MathExt;
using System.Runtime.CompilerServices;
using UnityUtilityTools;

namespace UnityUtility.Sound.SoundProviderStuff
{
    [DisallowMultipleComponent]
    public class SndObject : SoundObjectInfo, IPoolable
    {
        internal object Sender;

        [SerializeField]
        private AudioSource _audioSource;

        private float m_volume;
        private float m_pitch;
        private SPreset m_preset;
        private SoundProvider m_provider;
        private Action m_update;

        internal override string ClipName
        {
            get { return _audioSource.clip.name; }
        }

        internal bool IsLooped
        {
            get { return _audioSource.loop; }
        }

        internal override AudioSource AudioSource
        {
            get { return _audioSource; }
        }

        ///////////////
        //Unity Funcs//
        ///////////////

        private void Awake()
        {
            gameObject.Immortalize();

            if (_audioSource == null)
            {
                _audioSource = gameObject.GetOrAddComponent<AudioSource>();
                _audioSource.playOnAwake = false;
                _audioSource.dopplerLevel = 0f;
                _audioSource.rolloffMode = AudioRolloffMode.Custom;
            }

            m_update = () =>
            {
                if (_audioSource.isPlaying || _audioSource.time != 0f) { return; }
                Stop();
            };

            f_init();
        }

        private void OnDestroy()
        {
            m_provider.RemoveSound(this);
            ApplicationUtility.OnUpdate_Event -= m_update;
        }

        ////////////////
        //Public Funcs//
        ////////////////

        internal void Play(SoundProvider provider, object sender, AudioClip clip, SPreset preset)
        {
            _audioSource.clip = clip;
            m_preset = preset;

            m_provider = provider;
            Sender = sender;

            f_play();
        }

        internal void Play3D(SoundProvider provider, AudioClip clip, SPreset preset, Vector3 pos)
        {
            _audioSource.clip = clip;
            m_preset = preset;

            _audioSource.minDistance = preset.MinDist;
            _audioSource.maxDistance = preset.MaxDist;
            _audioSource.spatialBlend = 1f;

            m_provider = provider;

            transform.position = pos;

            f_play();
        }

        internal void Play3D(SoundProvider provider, AudioClip clip, SPreset preset, Transform sender)
        {
            _audioSource.clip = clip;
            m_preset = preset;

            _audioSource.minDistance = preset.MinDist;
            _audioSource.maxDistance = preset.MaxDist;
            _audioSource.spatialBlend = 1f;

            m_provider = provider;
            Sender = sender;

            transform.SetParent(sender, Vector3.zero);

            f_play();
        }

        internal void Restart()
        {
            StopAllCoroutines();
            _audioSource.Stop();
            f_play();
        }

        internal override void Stop()
        {
            StopAllCoroutines();
            _audioSource.Stop();
            m_provider.ReleaseSound(this);
        }

        internal void StopFading(float time)
        {
            StopAllCoroutines();
            StartCoroutine(FadeAndStop(time));
        }

        internal void Mute(bool value)
        {
            _audioSource.mute = value;
        }

        internal void UpdVolume()
        {
            _audioSource.volume = m_volume * m_provider.Volume;
        }

        internal void UpdPitch()
        {
            _audioSource.pitch = m_pitch * m_provider.Pitch;
        }

        internal void Pause()
        {
            _audioSource.Pause();
        }

        internal void UnPause()
        {
            _audioSource.UnPause();
        }

        ///////////////
        //Inner Funcs//
        ///////////////

        private void f_init()
        {
            ApplicationUtility.OnUpdate_Event += m_update;
        }

        #region IPoolable
        void IPoolable.Reinit()
        {
            gameObject.SetActive(true);
            f_init();
        }

        void IPoolable.CleanUp()
        {
            if (Sender != null && Sender is Transform) { transform.Free(); }
            gameObject.SetActive(false);
            _audioSource.clip = null;
            _audioSource.spatialBlend = 0f;
            Sender = null;
            ApplicationUtility.OnUpdate_Event -= m_update;
        }
        #endregion

        //////////////
        //Inner fncs//
        //////////////

        private void f_play()
        {
            m_volume = m_preset.Volume;
            m_pitch = m_preset.Pitch;
            _audioSource.loop = Sender == null ? false : m_preset.Looped;
            _audioSource.mute = m_provider.Muted;
            UpdVolume();
            UpdPitch();
            _audioSource.Play();
        }

        ////////////
        //Routines//
        ////////////

        IEnumerator FadeAndStop(float time)
        {
            if (time < 0f)
                throw Errors.NegativeTime(nameof(time));

            float startVal = m_volume;
            float curTime = 0f;
            float ratio = 0f;

            do
            {
                yield return null;

                if (m_provider.Paused) { continue; }

                curTime += Time.unscaledDeltaTime * m_provider.Pitch.Abs();
                ratio = curTime / time;
                m_volume = Mathf.Lerp(startVal, 0f, ratio);
                UpdVolume();

            } while (ratio < 1f);

            _audioSource.Stop();
            m_provider.ReleaseSound(this);
        }
    }
}
