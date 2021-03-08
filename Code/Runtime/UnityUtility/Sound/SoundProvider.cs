using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityUtility.Collections;
using UnityUtility.MathExt;
using UnityUtility.SaveLoad;
using UnityUtility.Sound.SoundProviderStuff;
using UnityUtilityTools;

namespace UnityUtility.Sound
{
    public class SoundProvider
    {
        private readonly SPreset _defaultPreset = new SPreset { Volume = 1f, Pitch = 1f, MinDist = 1f, MaxDist = 500f };

        private static IObjectCreator<SndObject> _creator;
        private static ObjectPool<SndObject> _pool;

        private bool _locked;

        [SaveLoadField]
        private bool _isMuted;
        [SaveLoadField(1f)]
        private float _volume = 1f;

        private float _pitch = 1f;
        private bool _paused;

        private Dictionary<SoundKey, SndObject> _keyedSounds;
        private HashSet<SndObject> _freeSounds;
        private IClipLoader _loader;
        private Dictionary<string, SPreset> _presetList;

        public bool Muted
        {
            get { return _isMuted; }
        }

        public float Volume
        {
            get { return _volume; }
        }

        public float Pitch
        {
            get { return _pitch; }
        }

        public bool Paused
        {
            get { return _paused; }
        }

        static SoundProvider()
        {
            _creator = new DynamicSndSourceCreator();
            _pool = new ObjectPool<SndObject>(_creator.Create);
        }

        public SoundProvider(SoundsPreset presetList = null) : this(new DefaultClipLoader("Sounds/"), presetList)
        {

        }

        public SoundProvider(IClipLoader loader, SoundsPreset presetList = null)
        {
            _keyedSounds = new Dictionary<SoundKey, SndObject>();
            _freeSounds = new HashSet<SndObject>();

            _loader = loader;
            _presetList = presetList == null ? new Dictionary<string, SPreset>() : presetList.CreateDict();
        }

        public static void OverrideAudioSourceCreator(IObjectCreator<SndObject> newCreator)
        {
            _creator = newCreator;
            _pool.ChangeCreator(_creator.Create);
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

            SndObject snd = _pool.Get();
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

            if (!_keyedSounds.TryGetValue(key, out SndObject snd))
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

            SndObject snd = _pool.Get();
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

            if (!_keyedSounds.TryGetValue(key, out SndObject snd))
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
            if (_keyedSounds.TryGetValue(new SoundKey(sender, soundName), out SndObject snd))
            {
                snd.Stop();
            }
        }

        public void StopFading(object sender, string soundName, float time = 1f)
        {
            if (_keyedSounds.TryGetValue(new SoundKey(sender, soundName), out SndObject snd))
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

        internal void ReleaseSound(SndObject snd)
        {
            RemoveSound(snd);

            _pool.Release(snd);
        }

        internal void RemoveSound(SndObject snd)
        {
            if (!_locked)
            {
                if (snd.Sender == null) { _freeSounds.Remove(snd); }
                else { _keyedSounds.Remove(new SoundKey(snd.Sender, snd.ClipName)); }
            }
        }

        //--//

        private SPreset GetPreset(string soundName)
        {
            if (!_presetList.TryGetValue(soundName, out SPreset set))
                set = _defaultPreset;

            return set;
        }

        private struct SoundKey : IEquatable<SoundKey>
        {
            private readonly int HASH;

            public SoundKey(object sender, string name)
            {
                HASH = Helper.GetHashCode(sender.GetHashCode(), name.GetHashCode());
            }

            // -- //

            public override bool Equals(object obj) => obj is SoundKey && Equals((SoundKey)obj);
            public bool Equals(SoundKey other) => HASH == other.HASH;
            public override int GetHashCode() => HASH;
            public static bool operator ==(SoundKey a, SoundKey b) => a.HASH == b.HASH;
            public static bool operator !=(SoundKey a, SoundKey b) => a.HASH != b.HASH;
        }
    }
}
