using System;
using System.Collections.Generic;
using UnityEditor;

namespace UnityUtilityEditor.Configs
{
    [Serializable]
    internal class LayerSetConfig
    {
        public bool GenerateStaticClass;
        public string RootFolder = $"{AssetDatabaseExt.ASSET_FOLDER}Code/";
        public string NameSpace = nameof(UnityEngine);
        public bool TagFields;
        public bool SortingLayerFields;
        public bool LayersFields = true;
        public List<LayerMaskField> LayerMasks;
        public bool AutoGenerate;

        public LayerSetConfig()
        {
            LayerMasks = new List<LayerMaskField>();
        }

        [Serializable]
        public struct LayerMaskField
        {
            public string Name;
            public int Mask;
        }
    }
}
