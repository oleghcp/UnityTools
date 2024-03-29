﻿#if INCLUDE_AUDIO
using System;
using System.Collections;
using OlegHcp.Engine;
using OlegHcp.Mathematics;
using OlegHcp.Pool;
using OlegHcp.Tools;
using UnityEngine;

namespace OlegHcp.Sound.SoundStuff
{
    [AddComponentMenu(nameof(OlegHcp) + "/Music Info")]
    public sealed class MusicInfo : AudioInfo, IPoolable, IDisposable
    {
        [SerializeField]
        private AudioSource _audioSource;

        private float _volume;
        private float _pitch;
        private MusicProvider _provider;
        private Action _update;
        private MPreset _preset;

        private bool _fading;

        internal override string ClipName => _audioSource.clip.name;
        internal override AudioSource AudioSource => _audioSource;
        internal bool Fading => _fading;

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

            _update = () =>
            {
                if (_audioSource.isPlaying || _audioSource.time != 0f) { return; }
                Stop();
            };

            ApplicationUtility.OnUpdate_Event += _update;
        }

        ////////////////
        //Public Funcs//
        ////////////////        

        internal void Play(MusicProvider provider, AudioClip clip, MPreset preset)
        {
            _provider = provider;
            _audioSource.clip = clip;
            _preset = preset;

            PlayInternal();
        }

        internal void Restart()
        {
            StopAllCoroutines();
            _audioSource.Stop();
            _fading = false;

            PlayInternal();
        }

        internal override void Stop()
        {
            StopAllCoroutines();
            _audioSource.Stop();
            _provider.ReleaseMusic(this);
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
            _audioSource.volume = _volume * _provider.Volume;
        }

        internal void UpdPitch()
        {
            _audioSource.pitch = _pitch * _provider.Pitch;
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

        private void PlayInternal()
        {
            _audioSource.loop = _preset.Looped;
            _audioSource.time = _preset.StartTime;
            _pitch = _preset.Pitch;

            if (_preset.Rising)
            {
                _volume = 0f;
                StartCoroutine(Rise());
            }
            else
            {
                _volume = _preset.Volume;
                UpdVolume();
            }

            UpdPitch();
            _audioSource.PlayDelayed(_preset.StartDelay);
        }

        #region IPoolable
        void IPoolable.Reinit()
        {
            gameObject.SetActive(true);
            ApplicationUtility.OnUpdate_Event += _update;
        }

        void IPoolable.CleanUp()
        {
            gameObject.SetActive(false);
            _audioSource.clip = null;
            _fading = false;
            ApplicationUtility.OnUpdate_Event -= _update;
        }

        void IDisposable.Dispose()
        {
            if (gameObject != null)
                gameObject.Destroy();
        }
        #endregion

        ////////////
        //Routines//
        ////////////

        private IEnumerator Rise()
        {
            UpdVolume();
            float curTime = 0f;

            while (curTime < _preset.StartDelay)
            {
                yield return null;

                if (_provider.Paused) { continue; }

                curTime += Time.unscaledDeltaTime * _audioSource.pitch.Abs();
            }

            curTime = 0f;
            float ratio = 0f;

            do
            {
                yield return null;

                if (_provider.Paused) { continue; }

                curTime += Time.unscaledDeltaTime * _provider.Pitch.Abs();
                ratio = curTime / _preset.RisingDur;
                _volume = Mathf.Lerp(0f, _preset.Volume, curTime);
                UpdVolume();

            } while (ratio < 1f);
        }

        private IEnumerator FadeAndStop(float time)
        {
            if (time < 0f)
                throw ThrowErrors.NegativeTime(nameof(time));

            _fading = true;
            float startVal = _volume;
            float curTime = 0f;
            float ratio = 0f;

            do
            {
                yield return null;

                if (_provider.Paused) { continue; }

                curTime += Time.unscaledDeltaTime * _provider.Pitch.Abs();
                ratio = curTime / time;
                _volume = Mathf.Lerp(startVal, 0f, ratio);
                UpdVolume();

            } while (ratio < 1f);

            _audioSource.Stop();
            _provider.ReleaseMusic(this);
        }
    }
}
#endif
