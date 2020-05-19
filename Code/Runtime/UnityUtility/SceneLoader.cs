using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityUtility.Async;

namespace UnityUtility
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

        private static T s_sceneInfo;
        private static int s_sceneLoaderIndex;
        private static bool s_sceneConfigured;
        private static ISceneConfigurator s_sceneConfigurator;

        public static T SceneInfo => s_sceneInfo;

        protected static void SetUp(string transitionalSceneName = "SceneLoader")
        {
            s_sceneLoaderIndex = SceneUtility.GetBuildIndexByScenePath(transitionalSceneName);
            s_sceneConfigured = true;

            SceneManager.sceneLoaded += (scene, _) =>
            {
                if (s_sceneConfigured && scene.buildIndex != s_sceneLoaderIndex)
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

                SceneManager.LoadScene(s_sceneInfo.SceneName);
            }

            StartCoroutine(ClearAndLoad());
        }

        public static void WaitForConfigurator(ISceneConfigurator sceneConfigurator)
        {
            if (s_sceneConfigurator != null)
            {
                throw new InvalidOperationException("Configurator already set for this scene instance.");
            }

            s_sceneConfigurator = sceneConfigurator;
            s_sceneConfigured = false;

            IEnumerator WaitForConfigurator()
            {
                while (!s_sceneConfigurator.ConfigurationDone)
                {
                    yield return null;
                }

                s_sceneConfigurator = null;
                Loaded_Event?.Invoke();
            }

            TaskSystem.StartAsync(WaitForConfigurator());
        }

        public static void LoadScene(T sceneInfo)
        {
            s_sceneInfo = sceneInfo;
            s_sceneConfigured = true;

            SceneManager.LoadScene(s_sceneLoaderIndex);
        }
    }
}
