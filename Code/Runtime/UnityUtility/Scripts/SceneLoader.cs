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

    public abstract class SceneLoader<T> : MonoBehaviour where T : Enum
    {
        private const string SCENE_LOADER_SCENE_NAME = "SceneLoader";

        public static event Action Unloaded_Event;
        public static event Action Interim_Event;
        public static event Action Loaded_Event;

        private static T _scene;
        private static int _sceneLoaderIndex;
        private static object _transData;
        private static bool _sceneConfigured;
        private static ISceneConfigurator _sceneConfigurator;

        public static T Scene => _scene;
        public static object TransData => _transData;

        protected static void SetUp()
        {
            _sceneLoaderIndex = SceneUtility.GetBuildIndexByScenePath(SCENE_LOADER_SCENE_NAME);
            _sceneConfigured = true;

            SceneManager.sceneLoaded += (scene, _) =>
            {
                if (_sceneConfigured && scene.buildIndex != _sceneLoaderIndex)
                    Loaded_Event?.Invoke();
            };
        }

        private void Start()
        {
            StartCoroutine(ClearAndLoad());
        }

        private IEnumerator ClearAndLoad()
        {
            Unloaded_Event?.Invoke();

            yield return null;

            Interim_Event?.Invoke();

            SceneManager.LoadScene(_scene.GetName());
        }

        public static void RegSceneConfigurator(ISceneConfigurator configurator)
        {
            _sceneConfigurator = configurator;
            _sceneConfigured = false;

            TaskSystem.StartAsync(WaitForConfigurator());
        }

        public static void LoadScene(T scene, object transData = null)
        {
            _scene = scene;
            _transData = transData;
            _sceneConfigured = true;

            SceneManager.LoadScene(_sceneLoaderIndex);
        }

        // -- //

        private static IEnumerator WaitForConfigurator()
        {
            while (!_sceneConfigurator.ConfigurationDone)
            {
                yield return null;
            }

            _sceneConfigurator = null;
            Loaded_Event?.Invoke();
        }
    }
}
