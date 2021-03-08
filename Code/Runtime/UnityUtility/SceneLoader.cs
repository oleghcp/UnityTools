using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityUtility.Async;
using UnityUtility.SceneLoading;

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
        private static ILoadDependency _configurator;

        public static T SceneInfo => _sceneInfo;

        protected static void SetUp(string transitionalSceneName = "SceneLoader")
        {
            if (_sceneLoaderIndex >= 0)
                return;

            _sceneLoaderIndex = SceneUtility.GetBuildIndexByScenePath(transitionalSceneName);

            SceneManager.sceneUnloaded += scene =>
            {
                if (scene.buildIndex != _sceneLoaderIndex)
                    Unloaded_Event?.Invoke();
            };

            SceneManager.sceneLoaded += (scene, _) =>
            {
                if (_configurator == null && scene.buildIndex != _sceneLoaderIndex)
                {
                    Loaded_Event?.Invoke();
                }
            };

#if UNITY_EDITOR
            ApplicationUtility.OnApplicationQuit_Event += delegate
            {
                BeginUnload_Event = null;
                Unloaded_Event = null;
                Interim_Event = null;
                Loaded_Event = null;
            };
#endif
        }

        private void Start()
        {
            Interim_Event?.Invoke();
            SceneManager.LoadScene(_sceneInfo.SceneName);
        }

        /// <summary>
        /// Scene configurator should be registered on awake when target scene already loaded but you need to call Loaded_Event after some preparations.
        /// </summary>
        public static void WaitForConfigurator(ILoadDependency sceneConfigurator)
        {
            if (_configurator != null)
            {
                throw new InvalidOperationException("Configurator already set for this scene instance.");
            }

            _configurator = sceneConfigurator;

            IEnumerator WaitForConfigurator()
            {
                while (!_configurator.Done)
                {
                    yield return null;
                }

                _configurator = null;
                Loaded_Event?.Invoke();
            }

            TaskSystem.StartAsync(WaitForConfigurator());
        }

        public static void LoadScene(T sceneInfo)
        {
            BeginUnload_Event?.Invoke();

            _sceneInfo = sceneInfo;

            SceneManager.LoadScene(_sceneLoaderIndex);
        }
    }
}
