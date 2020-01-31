using System;
using System.Collections.Generic;
using UU.Sound.SoundProviderStuff;
using UU.Collections;
using UU.MathExt;
using UU.SaveLoad;

namespace UU.Sound
{
    public class MusicProvider
    {
        private readonly MPreset DEF_SET = new MPreset { Volume = 1f, Pitch = 1f, Looped = true, RisingDur = 0.5f };

        private static ObjectCreator<MusObject> s_creator;
        private static ObjectPool<MusObject> s_pool;

        private bool m_locked;

        [SaveLoadField]
        private bool m_isMuted;
        [SaveLoadField(1f)]
        private float m_volume = 1f;

        private float m_pitch = 1f;
        private bool m_paused;

        private Dictionary<string, MusObject> m_music;

        private ClipLoader m_loader;
        private Dictionary<string, MPreset> m_presetList;

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

        static MusicProvider()
        {
            s_creator = new DynamicMusSourceCreator();
            s_pool = new ObjectPool<MusObject>(s_creator.Create);
        }

        public MusicProvider(MusicPreset presetList = null) : this(new DefaultClipLoader("Music/"), presetList)
        {

        }

        public MusicProvider(ClipLoader loader, MusicPreset presetList = null)
        {
            m_music = new Dictionary<string, MusObject>();

            m_loader = loader;
            m_presetList = presetList == null ? new Dictionary<string, MPreset>() : presetList.CreateDict();
        }

        public static void OverrideAudioSourceCreator(ObjectCreator<MusObject> newCreator)
        {
            s_creator = newCreator;
            s_pool.ChangeCreator(s_creator.Create);
        }

        public void SetVolume(float value)
        {
            m_volume = value.Saturate();

            foreach (var kvp in m_music)
            {
                kvp.Value.UpdVolume();
            }
        }

        public void SetPitch(float value)
        {
            m_pitch = value;

            foreach (var kvp in m_music)
            {
                kvp.Value.UpdPitch();
            }
        }

        public void Play(string musicName)
        {
            if (m_paused) { return; }

            if (!m_music.TryGetValue(musicName, out MusObject mus))
            {
                if (!m_presetList.TryGetValue(musicName, out MPreset set))
                    set = DEF_SET;

                mus = m_music.AddNGet(musicName, s_pool.Get());
                mus.Play(this, m_loader.LoadClip(musicName), set);
            }
            else if (mus.Fading)
            {
                mus.Restart();
            }
        }

        public void Stop(string musicName)
        {
            if (m_music.TryGetValue(musicName, out MusObject mus))
            {
                mus.Stop();
            }
        }

        public void StopFading(string musicName, float time = 1f)
        {
            if (m_music.TryGetValue(musicName, out MusObject mus))
            {
                mus.StopFading(time);
            }
        }

        public void StopAll()
        {
            m_locked = true;

            foreach (var kvp in m_music)
            {
                kvp.Value.Stop();
            }

            m_music.Clear();

            m_locked = false;
        }

        public void StopAllFading(float time = 1f)
        {
            foreach (var kvp in m_music)
            {
                kvp.Value.StopFading(time);
            }
        }

        public void Pause(bool on)
        {
            if (m_paused == on) { return; }

            if (m_paused = on)
                foreach (var kvp in m_music) { kvp.Value.Pause(); }
            else
                foreach (var kvp in m_music) { kvp.Value.UnPause(); }
        }

        public void MuteAll(bool value)
        {
            if (m_isMuted != value)
            {
                m_isMuted = value;

                foreach (var kvp in m_music) { kvp.Value.Mute(value); }
            }
        }

        internal void ReleaseMusic(MusObject mus)
        {
            if (!m_locked)
            {
                m_music.Remove(mus.ClipName);
            }

            s_pool.Release(mus);
        }
    }
}
