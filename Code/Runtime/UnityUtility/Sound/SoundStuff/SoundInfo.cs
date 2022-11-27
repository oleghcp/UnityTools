using System;
using System.Collections;
using UnityEngine;
using UnityUtility.Engine;
using UnityUtility.Mathematics;
using UnityUtility.Pool;
using UnityUtility.Tools;

#if !UNITY_2019_1_OR_NEWER || INCLUDE_AUDIO
namespace UnityUtility.Sound.SoundStuff
{
    [AddComponentMenu(nameof(UnityUtility) + "/Sound Info")]
    public sealed class SoundInfo : AudioInfo, IPoolable
    {
        internal object Sender;

        [SerializeField]
        private AudioSource _audioSource;

        private float _volume;
        private float _pitch;
        private SPreset _preset;
        private SoundsProvider _provider;
        private Action _update;

        internal override string ClipName => _audioSource.clip.name;
        internal bool IsLooped => _audioSource.loop;
        internal override AudioSource AudioSource => _audioSource;

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

            _update = () =>
            {
                if (_audioSource.isPlaying || _audioSource.time != 0f) { return; }
                Stop();
            };

            ApplicationUtility.OnUpdate_Event += _update;
        }

        private void OnDestroy()
        {
            _provider.RemoveSound(this);
            ApplicationUtility.OnUpdate_Event -= _update;
        }

        ////////////////
        //Public Funcs//
        ////////////////

        internal void Play(SoundsProvider provider, object sender, AudioClip clip, SPreset preset)
        {
            _audioSource.clip = clip;
            _preset = preset;

            _provider = provider;
            Sender = sender;

            PlayInternal();
        }

        internal void Play3D(SoundsProvider provider, AudioClip clip, SPreset preset, Vector3 pos)
        {
            _audioSource.clip = clip;
            _preset = preset;

            _audioSource.minDistance = preset.MinDist;
            _audioSource.maxDistance = preset.MaxDist;
            _audioSource.spatialBlend = 1f;

            _provider = provider;

            transform.position = pos;

            PlayInternal();
        }

        internal void Play3D(SoundsProvider provider, AudioClip clip, SPreset preset, Transform sender)
        {
            _audioSource.clip = clip;
            _preset = preset;

            _audioSource.minDistance = preset.MinDist;
            _audioSource.maxDistance = preset.MaxDist;
            _audioSource.spatialBlend = 1f;

            _provider = provider;
            Sender = sender;

            transform.SetParent(sender, Vector3.zero);

            PlayInternal();
        }

        internal void Restart()
        {
            StopAllCoroutines();
            _audioSource.Stop();
            PlayInternal();
        }

        internal override void Stop()
        {
            StopAllCoroutines();
            _audioSource.Stop();
            _provider.ReleaseSound(this);
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

        #region IPoolable
        void IPoolable.Reinit()
        {
            gameObject.SetActive(true);
            ApplicationUtility.OnUpdate_Event += _update;
        }

        void IPoolable.CleanUp()
        {
            if (Sender != null && Sender is Transform) { transform.Free(); }
            gameObject.SetActive(false);
            _audioSource.clip = null;
            _audioSource.spatialBlend = 0f;
            Sender = null;
            ApplicationUtility.OnUpdate_Event -= _update;
        }
        #endregion

        //////////////
        //Inner fncs//
        //////////////

        private void PlayInternal()
        {
            _volume = _preset.Volume;
            _pitch = _preset.Pitch;
            _audioSource.loop = Sender != null && _preset.Looped;
            _audioSource.mute = _provider.Muted;
            UpdVolume();
            UpdPitch();
            _audioSource.Play();
        }

        ////////////
        //Routines//
        ////////////

        private IEnumerator FadeAndStop(float time)
        {
            if (time < 0f)
                throw Errors.NegativeTime(nameof(time));

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
            _provider.ReleaseSound(this);
        }
    }
}
#endif
