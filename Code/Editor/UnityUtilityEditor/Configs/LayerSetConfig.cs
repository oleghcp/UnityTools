using System;
using System.Collections.Generic;
using UnityEditor;

namespace UnityUtilityEditor.Configs
{
    [Serializable]
    internal class LayerSetConfig
    {
        public bool GenerateStaticClass;
        public string ClassName = "LayerSet";
        public string RootFolder = $"{AssetDatabaseExt.ASSET_FOLDER}Code/";
        public string Namespace = nameof(UnityEngine);
        public bool TagFields;
        public bool SortingLayerFields;
        public bool LayersFields = true;
        public List<MaskField> LayerMasks;
        public bool AutoGenerate;

        public LayerSetConfig()
        {
            LayerMasks = new List<MaskField>();
        }

        [Serializable]
        public struct MaskField
        {
            public string Name;
            public int Mask;
        }
    }
}
