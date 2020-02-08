using System;
using UnityEngine;
using System.Collections.Generic;

#pragma warning disable CS0649
namespace UU.Sound
{
    [Serializable]
    internal class MPreset
    {
        [SerializeField]
        internal bool Looped;
        [SerializeField]
        internal bool Rising;
        [SerializeField]
        internal float Volume;
        [SerializeField]
        internal float Pitch;
        [SerializeField]
        internal float StartDelay;
        [SerializeField]
        internal float StartTime;
        [SerializeField]
        internal float RisingDur;
    }

    [CreateAssetMenu(menuName = "Sound (ext.)/Music Preset", fileName = "MusicPreset")]
    public sealed class MusicPreset : ScriptableObject
    {
        [Serializable]
        private struct Node
        {
            [SerializeField]
            internal string Name;
            [SerializeField]
            internal MPreset Stats;
        }

        [SerializeField]
        private Node[] m_nodes;

        internal static string NameProp
        {
            get { return nameof(Node.Name); }
        }

        internal static string StatsProp
        {
            get { return nameof(Node.Stats); }
        }

        internal Dictionary<string, MPreset> CreateDict()
        {
            var dict = new Dictionary<string, MPreset>(m_nodes.Length);

            for (int i = 0; i < m_nodes.Length; i++)
            {
                if (m_nodes[i].Name.HasAnyData())
                {
                    dict.Add(m_nodes[i].Name, m_nodes[i].Stats);
                }
            }

            return dict;
        }
    }
}
