using UnityEngine;
using System.Collections;
using UnityUtility.Collections;
using System;
using UnityUtility.Scripts;
using UnityUtility.MathExt;
using System.Runtime.CompilerServices;

namespace UnityUtility.Sound.SoundProviderStuff
{
    [DisallowMultipleComponent]
    public class MusObject : SoundObjectInfo, IPoolable
    {
        [SerializeField]
        private AudioSource _audioSource;

        private float m_volume;
        private float m_pitch;
        private MusicProvider m_provider;
        private Action m_update;
        private MPreset m_preset;

        private bool m_fading;

        internal override string ClipName
        {
            get { return _audioSource.clip.name; }
        }

        internal override AudioSource AudioSource
        {
            get { return _audioSource; }
        }

        internal bool Fading
        {
            get { return m_fading; }
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
            }

            m_update = () =>
            {
                if (_audioSource.isPlaying || _audioSource.time != 0f) { return; }
                Stop();
            };

            f_init();
        }

        ////////////////
        //Public Funcs//
        ////////////////        

        internal void Play(MusicProvider provider, AudioClip clip, MPreset preset)
        {
            m_provider = provider;
            _audioSource.clip = clip;
            m_preset = preset;

            f_play();
        }

        internal void Restart()
        {
            StopAllCoroutines();
            _audioSource.Stop();
            m_fading = false;

            f_play();
        }

        internal override void Stop()
        {
            StopAllCoroutines();
            _audioSource.Stop();
            m_provider.ReleaseMusic(this);
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

        private void f_play()
        {
            _audioSource.loop = m_preset.Looped;
            _audioSource.time = m_preset.StartTime;
            m_pitch = m_preset.Pitch;

            if (m_preset.Rising)
            {
                m_volume = 0f;
                StartCoroutine(Rise());
            }
            else
            {
                m_volume = m_preset.Volume;
                UpdVolume();
            }

            UpdPitch();
            _audioSource.PlayDelayed(m_preset.StartDelay);
        }

        #region IPoolable
        void IPoolable.Reinit()
        {
            gameObject.SetActive(true);
            f_init();
        }

        void IPoolable.CleanUp()
        {
            gameObject.SetActive(false);
            _audioSource.clip = null;
            m_fading = false;
            ApplicationUtility.OnUpdate_Event -= m_update;
        }
        #endregion

        ////////////
        //Routines//
        ////////////

        IEnumerator Rise()
        {
            UpdVolume();
            float curTime = 0f;

            while (curTime < m_preset.StartDelay)
            {
                yield return null;

                if (m_provider.Paused) { continue; }

                curTime += Time.unscaledDeltaTime * _audioSource.pitch.Abs();
            }

            curTime = 0f;
            float ratio = 0f;

            do
            {
                yield return null;

                if (m_provider.Paused) { continue; }

                curTime += Time.unscaledDeltaTime * m_provider.Pitch.Abs();
                ratio = curTime / m_preset.RisingDur;
                m_volume = Mathf.Lerp(0f, m_preset.Volume, curTime);
                UpdVolume();

            } while (ratio < 1f);
        }

        IEnumerator FadeAndStop(float time)
        {
            if (time < 0f)
                throw new ArgumentOutOfRangeException(nameof(time), "Time cannot be negative.");

            m_fading = true;
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
            m_provider.ReleaseMusic(this);
        }
    }
}
