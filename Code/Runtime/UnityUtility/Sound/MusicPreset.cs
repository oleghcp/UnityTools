using System;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable CS0649
namespace UnityUtility.Sound
{
    [Serializable]
    internal class MPreset
    {
        public bool Looped;
        public bool Rising;
        public float Volume;
        public float Pitch;
        public float StartDelay;
        public float StartTime;
        public float RisingDur;
    }

    [CreateAssetMenu(menuName = "Sound (ext.)/Music Preset", fileName = "MusicPreset")]
    public sealed class MusicPreset : ScriptableObject
    {
        [Serializable]
        private struct Node
        {
            public string Name;
            public MPreset Stats;
        }

        [SerializeField]
        private Node[] _nodes;

#if UNITY_EDITOR
        internal static string NameProp => nameof(Node.Name);
        internal static string StatsProp => nameof(Node.Stats);
#endif

        internal Dictionary<string, MPreset> CreateDict()
        {
            var dict = new Dictionary<string, MPreset>(_nodes.Length);

            for (int i = 0; i < _nodes.Length; i++)
            {
                if (_nodes[i].Name.HasAnyData())
                {
                    dict.Add(_nodes[i].Name, _nodes[i].Stats);
                }
            }

            return dict;
        }
    }
}
