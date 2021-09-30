using System;
using UnityEditor;

namespace UnityUtilityEditor.Configs
{
    [Serializable]
    internal class LayerSetConfig
    {
        public string ClassName = "LayerSet";
        public string RootFolder = $"{AssetDatabaseExt.ASSET_FOLDER}Code/";
        public string Namespace = "Project";
        public bool TagFields;
        public bool SortingLayerFields;
        public bool LayerFields = true;
        public LayerMaskFieldType MaskFieldType;
        public MaskField[] LayerMasks = new MaskField[0];

        [Serializable]
        public struct MaskField
        {
            public string Name;
            public int Mask;
        }

        public enum LayerMaskFieldType
        {
            LayerMask,
            Int,
        }
    }
}
