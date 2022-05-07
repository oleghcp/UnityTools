using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnityUtility
{
    public interface ISceneInfo
    {
        string SceneName { get; }
    }

    public abstract class SceneLoader<T> : MonoBehaviour where T : ISceneInfo
    {
        public static event Action BeginUnload_Event;
        public static event Action Unloaded_Event;
        public static event Action Interim_Event;
        public static event Action Loaded_Event;

        private static T _sceneInfo;
        private static int _sceneLoaderIndex = -1;

        public static T SceneInfo => _sceneInfo;

        protected static void SetUp(string transitionalSceneName = "SceneLoader")
        {
            if (_sceneLoaderIndex >= 0)
                return;

            _sceneLoaderIndex = SceneUtility.GetBuildIndexByScenePath(transitionalSceneName);

            SceneManager.sceneUnloaded += SceneUnloaded;
            SceneManager.activeSceneChanged += ActiveSceneChanged;

#if UNITY_EDITOR
            ApplicationUtility.OnApplicationQuit_Event += delegate
            {
                BeginUnload_Event = null;
                Unloaded_Event = null;
                Interim_Event = null;
                Loaded_Event = null;

                SceneManager.sceneUnloaded -= SceneUnloaded;
                SceneManager.activeSceneChanged -= ActiveSceneChanged;
            };
#endif
        }

        private void Start()
        {
            Interim_Event?.Invoke();
            SceneManager.LoadScene(_sceneInfo.SceneName);
        }

        public static void LoadScene(T sceneInfo)
        {
            BeginUnload_Event?.Invoke();

            _sceneInfo = sceneInfo;

            SceneManager.LoadScene(_sceneLoaderIndex);
        }

        private static void SceneUnloaded(Scene scene)
        {
            if (scene.buildIndex == _sceneLoaderIndex && SceneManager.GetActiveScene() == scene)
                return;

            Unloaded_Event?.Invoke();
        }

        private static void ActiveSceneChanged(Scene prevScene, Scene newScene)
        {
            if (newScene.buildIndex != _sceneLoaderIndex)
                Loaded_Event?.Invoke();
        }
    }
}
