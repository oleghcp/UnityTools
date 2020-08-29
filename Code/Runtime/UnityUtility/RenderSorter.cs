#pragma warning disable CS0649
using UnityEngine;

namespace UnityUtility
{
    [RequireComponent(typeof(Renderer)), DisallowMultipleComponent]
    public sealed class RenderSorter : MonoBehaviour
    {
        [SerializeField, HideInInspector]
        private Renderer _renderer;
        [SerializeField, SortingLayerID]
        private int _sortingLayer;
        [SerializeField]
        private int _sortingOrder;
    }
}
