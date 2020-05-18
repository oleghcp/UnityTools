using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityUtility.Async;

namespace UnityUtility.Scripts
{
    public interface ISceneConfigurator
    {
        bool ConfigurationDone { get; }
    }

    public interface ISceneInfo
    {
        string SceneName { get; }
    }

    public abstract class SceneLoader<T> : MonoBehaviour where T : ISceneInfo
    {
        public static event Action Unloaded_Event;
        public static event Action Interim_Event;
        public static event Action Loaded_Event;

        private static T _sceneInfo;
        private static int _sceneLoaderIndex;
        private static bool _sceneConfigured;
        private static ISceneConfigurator _sceneConfigurator;

        public static T SceneInfo => _sceneInfo;

        protected static void SetUp()
        {
            _sceneLoaderIndex = SceneUtility.GetBuildIndexByScenePath("SceneLoader");
            _sceneConfigured = true;

            SceneManager.sceneLoaded += (scene, _) =>
            {
                if (_sceneConfigured && scene.buildIndex != _sceneLoaderIndex)
                    Loaded_Event?.Invoke();
            };
        }

        private void Start()
        {
            IEnumerator ClearAndLoad()
            {
                Unloaded_Event?.Invoke();

                yield return null;

                Interim_Event?.Invoke();

                SceneManager.LoadScene(_sceneInfo.SceneName);
            }

            StartCoroutine(ClearAndLoad());
        }

        public static void RegisterConfigurator(ISceneConfigurator configurator)
        {
            _sceneConfigurator = configurator;
            _sceneConfigured = false;

            IEnumerator WaitForConfigurator()
            {
                while (!_sceneConfigurator.ConfigurationDone)
                {
                    yield return null;
                }

                _sceneConfigurator = null;
                Loaded_Event?.Invoke();
            }

            TaskSystem.StartAsync(WaitForConfigurator());
        }

        public static void LoadScene(T sceneInfo)
        {
            _sceneInfo = sceneInfo;
            _sceneConfigured = true;

            SceneManager.LoadScene(_sceneLoaderIndex);
        }
    }
}
