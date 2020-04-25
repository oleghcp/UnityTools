using UnityEngine;

#pragma warning disable CS0649
namespace UnityUtility.Scripts
{
    [RequireComponent(typeof(Renderer)), DisallowMultipleComponent]
    public class RenderSorter : MonoBehaviour
    {
        [SerializeField, HideInInspector]
        private Renderer _renderer;
        [SerializeField, SortingLayerID]
        private int _sortingLayer;
        [SerializeField]
        private int _sortingOrder;
    }
}
