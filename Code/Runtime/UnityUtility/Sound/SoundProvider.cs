using System;
using UnityEngine;
using System.Collections.Generic;
using UnityUtility.Sound.SoundProviderStuff;
using UnityUtility.Collections;
using UnityUtility.MathExt;
using UnityUtility.SaveLoad;
using System.Runtime.CompilerServices;
using Tools;

namespace UnityUtility.Sound
{
    internal struct SoundKey : IEquatable<SoundKey>
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

    public class SoundProvider
    {
        private readonly SPreset DEF_SET = new SPreset { Volume = 1f, Pitch = 1f, MinDist = 1f, MaxDist = 500f };

        private static IObjectCreator<SndObject> s_creator;
        private static ObjectPool<SndObject> s_pool;

        private bool m_locked;

        [SaveLoadField]
        private bool m_isMuted;
        [SaveLoadField(1f)]
        private float m_volume = 1f;

        private float m_pitch = 1f;
        private bool m_paused;

        private Dictionary<SoundKey, SndObject> m_keyedSounds;
        private HashSet<SndObject> m_freeSounds;

        private IClipLoader m_loader;
        private Dictionary<string, SPreset> m_presetList;

        public bool Muted
        {
            get { return m_isMuted; }
        }

        public float Volume
        {
            get { return m_volume; }
        }

        public float Pitch
        {
            get { return m_pitch; }
        }

        public bool Paused
        {
            get { return m_paused; }
        }

        static SoundProvider()
        {
            s_creator = new DynamicSndSourceCreator();
            s_pool = new ObjectPool<SndObject>(s_creator.Create);
        }

        public SoundProvider(SoundsPreset presetList = null) : this(new DefaultClipLoader("Sounds/"), presetList)
        {

        }

        public SoundProvider(IClipLoader loader, SoundsPreset presetList = null)
        {
            m_keyedSounds = new Dictionary<SoundKey, SndObject>();
            m_freeSounds = new HashSet<SndObject>();

            m_loader = loader;
            m_presetList = presetList == null ? new Dictionary<string, SPreset>() : presetList.CreateDict();
        }

        public static void OverrideAudioSourceCreator(IObjectCreator<SndObject> newCreator)
        {
            s_creator = newCreator;
            s_pool.ChangeCreator(s_creator.Create);
        }

        public void SetVolume(float value)
        {
            m_volume = value.Saturate();

            foreach (var kvp in m_keyedSounds) { kvp.Value.UpdVolume(); }
            foreach (var snd in m_freeSounds) { snd.UpdVolume(); }
        }

        public void SetPitch(float value)
        {
            m_pitch = value;

            foreach (var kvp in m_keyedSounds) { kvp.Value.UpdPitch(); }
            foreach (var snd in m_freeSounds) { snd.UpdPitch(); }
        }

        public void Play(string soundName)
        {
            Play(m_loader.LoadClip(soundName));
        }

        public void Play(AudioClip clip)
        {
            if (m_paused) { return; }

            SPreset set = f_getPreset(clip.name);

            SndObject snd = s_pool.Get();
            snd.Play(this, null, clip, set);
            m_freeSounds.Add(snd);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Play(object sender, string soundName, bool breakCurrent = false)
        {
            Play(sender, m_loader.LoadClip(soundName), breakCurrent);
        }

        public void Play(object sender, AudioClip clip, bool breakCurrent = false)
        {
            if (m_paused) { return; }

            SoundKey key = new SoundKey(sender, clip.name);

            if (!m_keyedSounds.TryGetValue(key, out SndObject snd))
            {
                SPreset set = f_getPreset(clip.name);
                snd = m_keyedSounds.Place(key, s_pool.Get());
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
            Play3D(m_loader.LoadClip(soundName), pos);
        }

        public void Play3D(AudioClip clip, Vector3 pos)
        {
            if (m_paused) { return; }

            SPreset set = f_getPreset(clip.name);

            SndObject snd = s_pool.Get();
            snd.Play3D(this, clip, set, pos);
            m_freeSounds.Add(snd);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Play3D(Transform sender, string soundName, bool breakCurrent = false)
        {
            Play3D(sender, m_loader.LoadClip(soundName), breakCurrent);
        }

        public void Play3D(Transform sender, AudioClip clip, bool breakCurrent = false)
        {
            if (m_paused) { return; }
            SoundKey key = new SoundKey(sender, clip.name);

            if (!m_keyedSounds.TryGetValue(key, out SndObject snd))
            {
                SPreset set = f_getPreset(clip.name);
                snd = m_keyedSounds.Place(key, s_pool.Get());
                snd.Play3D(this, clip, set, sender);
            }
            else if (breakCurrent && !snd.IsLooped)
            {
                snd.Restart();
            }
        }

        public void Stop(object sender, string soundName)
        {
            if (m_keyedSounds.TryGetValue(new SoundKey(sender, soundName), out SndObject snd))
            {
                snd.Stop();
            }
        }

        public void StopFading(object sender, string soundName, float time = 1f)
        {
            if (m_keyedSounds.TryGetValue(new SoundKey(sender, soundName), out SndObject snd))
            {
                snd.StopFading(time);
            }
        }

        public void StopAll()
        {
            m_locked = true;

            foreach (var kvp in m_keyedSounds) { kvp.Value.Stop(); }
            m_keyedSounds.Clear();
            foreach (var snd in m_freeSounds) { snd.Stop(); }
            m_freeSounds.Clear();

            m_locked = false;
        }

        public void Pause(bool on)
        {
            if (m_paused == on) { return; }

            if (m_paused = on)
            {
                foreach (var kvp in m_keyedSounds) { kvp.Value.Pause(); }
                foreach (var snd in m_freeSounds) { snd.Pause(); }
            }
            else
            {
                foreach (var kvp in m_keyedSounds) { kvp.Value.UnPause(); }
                foreach (var snd in m_freeSounds) { snd.UnPause(); }
            }
        }

        public void MuteAll(bool value)
        {
            if (m_isMuted != value)
            {
                m_isMuted = value;

                foreach (var kvp in m_keyedSounds) { kvp.Value.Mute(value); }
                foreach (var snd in m_freeSounds) { snd.Mute(value); }
            }
        }

        internal void ReleaseSound(SndObject snd)
        {
            RemoveSound(snd);

            s_pool.Release(snd);
        }

        internal void RemoveSound(SndObject snd)
        {
            if (!m_locked)
            {
                if (snd.Sender == null) { m_freeSounds.Remove(snd); }
                else { m_keyedSounds.Remove(new SoundKey(snd.Sender, snd.ClipName)); }
            }
        }

        //--//

        private SPreset f_getPreset(string soundName)
        {
            if (!m_presetList.TryGetValue(soundName, out SPreset set))
                set = DEF_SET;

            return set;
        }
    }
}
