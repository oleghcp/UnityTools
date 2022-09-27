using System;
using System.Collections;
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
        public static event Action Loaded_Event;

        private static T _sceneInfo;
        private static Scene _sceneLoader;

        public static T SceneInfo => _sceneInfo;

        protected static void SetUp(string transitionalSceneName = "SceneLoader")
        {
            if (_sceneLoader == default)
                return;

            _sceneLoader = SceneManager.GetSceneByName(transitionalSceneName);

            SceneManager.sceneUnloaded += SceneUnloaded;
            SceneManager.activeSceneChanged += ActiveSceneChanged;

#if UNITY_EDITOR
            ApplicationUtility.OnApplicationQuit_Event += delegate
            {
                BeginUnload_Event = null;
                Unloaded_Event = null;
                Loaded_Event = null;

                SceneManager.sceneUnloaded -= SceneUnloaded;
                SceneManager.activeSceneChanged -= ActiveSceneChanged;
            };
#endif
        }

        private IEnumerator Start()
        {
            yield return StartCoroutine(GetRransitionalRoutine());
            SceneManager.LoadScene(_sceneInfo.SceneName);
        }

        public static void LoadScene(T sceneInfo)
        {
            BeginUnload_Event?.Invoke();

            _sceneInfo = sceneInfo;

            SceneManager.LoadScene(_sceneLoader.buildIndex);
        }

        protected virtual IEnumerator GetRransitionalRoutine()
        {
            yield break;
        }

        private static void SceneUnloaded(Scene scene)
        {
            if (SceneManager.GetActiveScene() == scene && scene != _sceneLoader)
                Unloaded_Event?.Invoke();
        }

        private static void ActiveSceneChanged(Scene prevScene, Scene newScene)
        {
            if (newScene != _sceneLoader)
                Loaded_Event?.Invoke();
        }
    }
}
