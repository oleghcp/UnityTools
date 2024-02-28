#if INCLUDE_AUDIO
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using OlegHcp.CSharp.Collections;
using OlegHcp.Mathematics;
using OlegHcp.Pool;
using OlegHcp.SaveLoad;
using OlegHcp.Sound.SoundStuff;
using OlegHcp.Tools;
using UnityEngine;

namespace OlegHcp.Sound
{
    public class SoundsProvider
    {
        private readonly SPreset _defaultPreset = new SPreset { Volume = 1f, Pitch = 1f, MinDist = 1f, MaxDist = 500f };

        private bool _locked;

        [SaveLoadField]
        private bool _isMuted;
        [SaveLoadField(1f)]
        private float _volume = 1f;

        private float _pitch = 1f;
        private bool _paused;

        private Dictionary<SoundKey, SoundInfo> _keyedSounds;
        private HashSet<SoundInfo> _freeSounds;
        private ObjectPool<SoundInfo> _pool;
        private IClipLoader _loader;
        private Dictionary<string, SPreset> _presetList;

        public bool Muted => _isMuted;
        public float Volume => _volume;
        public float Pitch => _pitch;
        public bool Paused => _paused;

        public SoundsProvider(IClipLoader loader, IObjectFactory<SoundInfo> factory, SoundsPreset presetList = null)
        {
            _keyedSounds = new Dictionary<SoundKey, SoundInfo>();
            _freeSounds = new HashSet<SoundInfo>();
            _pool = new ObjectPool<SoundInfo>(factory);
            _loader = loader;
            _presetList = presetList == null ? new Dictionary<string, SPreset>() : presetList.CreateDict();
        }

        public void SetVolume(float value)
        {
            _volume = value.Clamp01();

            foreach (var kvp in _keyedSounds) { kvp.Value.UpdVolume(); }
            foreach (var snd in _freeSounds) { snd.UpdVolume(); }
        }

        public void SetPitch(float value)
        {
            _pitch = value;

            foreach (var kvp in _keyedSounds) { kvp.Value.UpdPitch(); }
            foreach (var snd in _freeSounds) { snd.UpdPitch(); }
        }

        public void Play(string soundName)
        {
            Play(_loader.LoadClip(soundName));
        }

        public void Play(AudioClip clip)
        {
            if (_paused) { return; }

            SPreset set = GetPreset(clip.name);

            SoundInfo snd = _pool.Get();
            snd.Play(this, null, clip, set);
            _freeSounds.Add(snd);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Play(object sender, string soundName, bool breakCurrent = false)
        {
            Play(sender, _loader.LoadClip(soundName), breakCurrent);
        }

        public void Play(object sender, AudioClip clip, bool breakCurrent = false)
        {
            if (_paused) { return; }

            SoundKey key = new SoundKey(sender, clip.name);

            if (!_keyedSounds.TryGetValue(key, out SoundInfo snd))
            {
                SPreset set = GetPreset(clip.name);
                snd = _keyedSounds.Place(key, _pool.Get());
                snd.Play(this, sender, clip, set);
            }
            else if (breakCurrent && !snd.IsLooped)
            {
                snd.Restart();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Play3D(string soundName, Vector3 pos)
        {
            Play3D(_loader.LoadClip(soundName), pos);
        }

        public void Play3D(AudioClip clip, Vector3 pos)
        {
            if (_paused) { return; }

            SPreset set = GetPreset(clip.name);

            SoundInfo snd = _pool.Get();
            snd.Play3D(this, clip, set, pos);
            _freeSounds.Add(snd);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Play3D(Transform sender, string soundName, bool breakCurrent = false)
        {
            Play3D(sender, _loader.LoadClip(soundName), breakCurrent);
        }

        public void Play3D(Transform sender, AudioClip clip, bool breakCurrent = false)
        {
            if (_paused) { return; }
            SoundKey key = new SoundKey(sender, clip.name);

            if (!_keyedSounds.TryGetValue(key, out SoundInfo snd))
            {
                SPreset set = GetPreset(clip.name);
                snd = _keyedSounds.Place(key, _pool.Get());
                snd.Play3D(this, clip, set, sender);
            }
            else if (breakCurrent && !snd.IsLooped)
            {
                snd.Restart();
            }
        }

        public void Stop(object sender, string soundName)
        {
            if (_keyedSounds.TryGetValue(new SoundKey(sender, soundName), out SoundInfo snd))
            {
                snd.Stop();
            }
        }

        public void StopFading(object sender, string soundName, float time = 1f)
        {
            if (_keyedSounds.TryGetValue(new SoundKey(sender, soundName), out SoundInfo snd))
            {
                snd.StopFading(time);
            }
        }

        public void StopAll()
        {
            _locked = true;

            foreach (var kvp in _keyedSounds) { kvp.Value.Stop(); }
            _keyedSounds.Clear();
            foreach (var snd in _freeSounds) { snd.Stop(); }
            _freeSounds.Clear();

            _locked = false;
        }

        public void Pause(bool on)
        {
            if (_paused == on) { return; }

            if (_paused = on)
            {
                foreach (var kvp in _keyedSounds) { kvp.Value.Pause(); }
                foreach (var snd in _freeSounds) { snd.Pause(); }
            }
            else
            {
                foreach (var kvp in _keyedSounds) { kvp.Value.UnPause(); }
                foreach (var snd in _freeSounds) { snd.UnPause(); }
            }
        }

        public void MuteAll(bool value)
        {
            if (_isMuted != value)
            {
                _isMuted = value;

                foreach (var kvp in _keyedSounds) { kvp.Value.Mute(value); }
                foreach (var snd in _freeSounds) { snd.Mute(value); }
            }
        }

        internal void ReleaseSound(SoundInfo snd)
        {
            RemoveSound(snd);

            _pool.Release(snd);
        }

        internal void RemoveSound(SoundInfo snd)
        {
            if (!_locked)
            {
                if (snd.Sender == null) { _freeSounds.Remove(snd); }
                else { _keyedSounds.Remove(new SoundKey(snd.Sender, snd.ClipName)); }
            }
        }

        private SPreset GetPreset(string soundName)
        {
            if (!_presetList.TryGetValue(soundName, out SPreset set))
                set = _defaultPreset;

            return set;
        }

        private readonly struct SoundKey : IEquatable<SoundKey>
        {
            private readonly int _hash;

            public SoundKey(object sender, string name)
            {
                _hash = Helper.GetHashCode(sender.GetHashCode(), name.GetHashCode());
            }

            public override bool Equals(object obj) => obj is SoundKey soundKey && Equals(soundKey);
            public bool Equals(SoundKey other) => _hash == other._hash;
            public override int GetHashCode() => _hash;
            public static bool operator ==(SoundKey a, SoundKey b) => a.Equals(b);
            public static bool operator !=(SoundKey a, SoundKey b) => !a.Equals(b);
        }
    }
}
#endif
