using OlegHcp.Engine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace MenuItems
{
    public static class SceneMenuItems
    {
        [MenuItem(nameof(GameObject) + "/Order Children (ext.)/A-Z", false, 20)]
        private static void OrderChildrenA()
        {
            GameObject gameObject = Selection.activeGameObject;
            gameObject.transform.OrderChildren(item => item.name);
            EditorSceneManager.MarkSceneDirty(gameObject.scene);
        }

        [MenuItem(nameof(GameObject) + "/Order Children (ext.)/Z-A", false, 21)]
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
            return Selection.activeGameObject != null && Selection.activeGameObject.transform.childCount > 0;
        }
    }
}
