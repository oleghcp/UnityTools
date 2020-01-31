using System;
using UnityEngine;
using System.Collections.Generic;

#pragma warning disable CS0649
namespace UU.Sound
{
    [Serializable]
    internal class SPreset
    {
        [SerializeField]
        internal bool Looped;
        [SerializeField]
        internal float Volume;
        [SerializeField]
        internal float Pitch;
        [SerializeField]
        internal float MinDist;
        [SerializeField]
        internal float MaxDist;
    }

    [CreateAssetMenu(menuName = "Sound/Sounds Preset", fileName = "SoundsPreset")]
    public sealed class SoundsPreset : ScriptableObject
    {
        [Serializable]
        private struct Node
        {
            [SerializeField]
            internal string Name;
            [SerializeField]
            internal SPreset Stats;
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

        internal Dictionary<string, SPreset> CreateDict()
        {
            var dict = new Dictionary<string, SPreset>(m_nodes.Length);

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
