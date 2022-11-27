using UnityEditor;
using UnityObject = UnityEngine.Object;

namespace UnityUtilityEditor.Engine
{
    internal static class Managers
    {
        public static UnityObject GetTagManager()
        {
            return GetAsset("TagManager");
        }

        public static UnityObject GetInputManager()
        {
            return GetAsset("InputManager");
        }

        private static UnityObject GetAsset(string assetName)
        {
            string assetPath = $"{AssetDatabaseExt.PROJECT_SETTINGS_FOLDER}{assetName}{AssetDatabaseExt.ASSET_EXTENSION}";
            return AssetDatabase.LoadAssetAtPath(assetPath, typeof(UnityObject));
        }
    }
}
