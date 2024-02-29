using UnityEngine;

namespace OlegHcp.Engine
{
    public static class LayerMaskExtensions
    {
        public static bool HasLayer(this LayerMask mask, int layer)
        {
            return BitMask.HasFlag(mask, layer);
        }

        public static bool HasLayer(this LayerMask mask, string layer)
        {
            return BitMask.HasFlag(mask, LayerMask.NameToLayer(layer));
        }
    }
}
