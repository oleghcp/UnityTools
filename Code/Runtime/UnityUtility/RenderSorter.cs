﻿using UnityEngine;

namespace UnityUtility
{
    [RequireComponent(typeof(Renderer))]
    [DisallowMultipleComponent]
    [AddComponentMenu(nameof(UnityUtility) + "/Render Sorter")]
    public sealed class RenderSorter : MonoBehaviour
    {
        [SerializeField]
        private Renderer _renderer;

#if UNITY_EDITOR
        internal Renderer Renderer => _renderer;
        internal static string RendererFieldName => nameof(_renderer);
#endif
    }
}
