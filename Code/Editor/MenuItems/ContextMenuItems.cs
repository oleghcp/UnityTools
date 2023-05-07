using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityUtility.Engine;

namespace MenuItems
{
    public static class ContextMenuItems
    {
        public const string CONTEXT_MENU_NAME = "CONTEXT/";
        public const string RESET_ITEM_NAME = "Reset";

        [MenuItem(nameof(GameObject) + "/Order Children (ext.)/A-Z", false, -1000)]
        private static void OrderChildrenA()
        {
            GameObject gameObject = Selection.activeGameObject;
            gameObject.transform.OrderChildren(item => item.name);
            EditorSceneManager.MarkSceneDirty(gameObject.scene);
        }

        [MenuItem(nameof(GameObject) + "/Order Children (ext.)/Z-A", false, -999)]
        private static void OrderChildrenZ()
        {
            GameObject gameObject = Selection.activeGameObject;
            gameObject.transform.OrderChildren(compare);
            EditorSceneManager.MarkSceneDirty(gameObject.scene);

            int compare(Transform a, Transform b) => -string.Compare(a.name, b.name);
        }

        [MenuItem(nameof(GameObject) + "/Order Children (ext.)/A-Z", true)]
        [MenuItem(nameof(GameObject) + "/Order Children (ext.)/Z-A", true)]
        private static bool OrderChildrenAvailable()
        {
            return Selection.activeGameObject != null;
        }
    }
}
