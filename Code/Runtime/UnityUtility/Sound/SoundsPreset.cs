using System;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable CS0649
namespace UnityUtility.Sound
{
    [Serializable]
    internal class SPreset
    {
        public bool Looped;
        public float Volume;
        public float Pitch;
        public float MinDist;
        public float MaxDist;
    }

    [CreateAssetMenu(menuName = "Sound (ext.)/Sounds Preset", fileName = "SoundsPreset")]
    public sealed class SoundsPreset : ScriptableObject
    {
        [Serializable]
        private struct Node
        {
            public string Name;
            public SPreset Stats;
        }

        [SerializeField]
        private Node[] _nodes;

#if UNITY_EDITOR
        internal static string NamePropName => nameof(Node.Name);
        internal static string StatsPropName => nameof(Node.Stats);
#endif

        internal Dictionary<string, SPreset> CreateDict()
        {
            var dict = new Dictionary<string, SPreset>(_nodes.Length);

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
