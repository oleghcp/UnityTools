using System.Collections.Generic;
using OlegHcp.Engine;
using UnityEngine;

namespace OlegHcp.Tools
{
    public static class SceneTool
    {
        private static GameObject _globalGameObject;
        private static GameObject _localGameObject;

        public static IReadOnlyList<GameObject> GetDontDestroyOnLoadObjects()
        {
            var scene = GetGlobal().scene;
            List<GameObject> buffer = new List<GameObject>(scene.rootCount);
            scene.GetRootGameObjects(buffer);
            return buffer;
        }

        public static void GetDontDestroyOnLoadObjects(List<GameObject> buffer)
        {
            GetGlobal().scene.GetRootGameObjects(buffer);
        }

        internal static GameObject GetGlobal()
        {
            if (_globalGameObject == null)
            {
                _globalGameObject = new GameObject(nameof(OlegHcp));
                _globalGameObject.Immortalize();
            }

            return _globalGameObject;
        }

        internal static GameObject GetLocal()
        {
            if (_localGameObject == null)
                _localGameObject = new GameObject(nameof(OlegHcp));

            return _localGameObject;
        }
    }
}
