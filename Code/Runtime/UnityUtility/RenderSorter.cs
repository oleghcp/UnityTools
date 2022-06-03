#pragma warning disable CS0649
using UnityEngine;

namespace UnityUtility
{
    [RequireComponent(typeof(Renderer))]
    [DisallowMultipleComponent]
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
