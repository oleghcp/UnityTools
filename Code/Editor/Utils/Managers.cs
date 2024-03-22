using UnityEditor;
using UnityObject = UnityEngine.Object;

namespace OlegHcpEditor.Utils
{
    public static class Managers
    {
        public static UnityObject GetTagManager()
        {
            return GetManagerAsset("TagManager");
        }

        public static UnityObject GetInputManager()
        {
            return GetManagerAsset("InputManager");
        }

        public static UnityObject GetManagerAsset(string assetName)
        {
            string assetPath = $"{AssetDatabaseExt.PROJECT_SETTINGS_FOLDER}{assetName}{AssetDatabaseExt.ASSET_EXTENSION}";
            return AssetDatabase.LoadAssetAtPath(assetPath, typeof(UnityObject));
        }
    }
}
