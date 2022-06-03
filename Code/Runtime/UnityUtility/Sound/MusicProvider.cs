#if !UNITY_2019_1_OR_NEWER || INCLUDE_AUDIO
using System.Collections.Generic;
using UnityUtility.MathExt;
using UnityUtility.Pool;
using UnityUtility.SaveLoad;
using UnityUtility.Sound.SoundStuff;

namespace UnityUtility.Sound
{
    public class MusicProvider
    {
        private readonly MPreset _defaultPreset = new MPreset { Volume = 1f, Pitch = 1f, Looped = true, RisingDur = 0.5f };

        private bool _locked;

        [SaveLoadField]
        private bool _isMuted;
        [SaveLoadField(1f)]
        private float _volume = 1f;

        private float _pitch = 1f;
        private bool _paused;

        private Dictionary<string, MusicInfo> _music;

        private IClipLoader _loader;
        private ObjectPool<MusicInfo> _pool;
        private Dictionary<string, MPreset> _presetList;

        public bool Muted => _isMuted;
        public float Volume => _volume;
        public float Pitch => _pitch;
        public bool Paused => _paused;

        public MusicProvider(IClipLoader loader, IObjectFactory<MusicInfo> factory, MusicPreset presetList = null)
        {
            _music = new Dictionary<string, MusicInfo>();
            _pool = new ObjectPool<MusicInfo>(factory);
            _loader = loader;
            _presetList = presetList == null ? new Dictionary<string, MPreset>() : presetList.CreateDict();
        }

        public void SetVolume(float value)
        {
            _volume = value.Clamp01();

            foreach (var kvp in _music)
            {
                kvp.Value.UpdVolume();
            }
        }

        public void SetPitch(float value)
        {
            _pitch = value;

            foreach (var kvp in _music)
            {
                kvp.Value.UpdPitch();
            }
        }

        public void Play(string musicName)
        {
            if (_paused) { return; }

            if (!_music.TryGetValue(musicName, out MusicInfo mus))
            {
                if (!_presetList.TryGetValue(musicName, out MPreset set))
                    set = _defaultPreset;

                mus = _music.Place(musicName, _pool.Get());
                mus.Play(this, _loader.LoadClip(musicName), set);
            }
            else if (mus.Fading)
            {
                mus.Restart();
            }
        }

        public void Stop(string musicName)
        {
            if (_music.TryGetValue(musicName, out MusicInfo mus))
            {
                mus.Stop();
            }
        }

        public void StopFading(string musicName, float time = 1f)
        {
            if (_music.TryGetValue(musicName, out MusicInfo mus))
            {
                mus.StopFading(time);
            }
        }

        public void StopAll()
        {
            _locked = true;

            foreach (var kvp in _music)
            {
                kvp.Value.Stop();
            }

            _music.Clear();

            _locked = false;
        }

        public void StopAllFading(float time = 1f)
        {
            foreach (var kvp in _music)
            {
                kvp.Value.StopFading(time);
            }
        }

        public void Pause(bool on)
        {
            if (_paused == on) { return; }

            if (_paused = on)
                foreach (var kvp in _music) { kvp.Value.Pause(); }
            else
                foreach (var kvp in _music) { kvp.Value.UnPause(); }
        }

        public void MuteAll(bool value)
        {
            if (_isMuted != value)
            {
                _isMuted = value;

                foreach (var kvp in _music) { kvp.Value.Mute(value); }
            }
        }

        internal void ReleaseMusic(MusicInfo mus)
        {
            if (!_locked)
            {
                _music.Remove(mus.ClipName);
            }

            _pool.Release(mus);
        }
    }
}
#endif
