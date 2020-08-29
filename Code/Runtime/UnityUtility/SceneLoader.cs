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

        private static T s_sceneInfo;
        private static int s_sceneLoaderIndex;
        private static ILoadDependency s_configurator;

        public static T SceneInfo => s_sceneInfo;

        protected static void SetUp(string transitionalSceneName = "SceneLoader")
        {
            s_sceneLoaderIndex = SceneUtility.GetBuildIndexByScenePath(transitionalSceneName);

            SceneManager.sceneUnloaded += scene => Unloaded_Event?.Invoke();

            SceneManager.sceneLoaded += (scene, _) =>
            {
                if (s_configurator == null && scene.buildIndex != s_sceneLoaderIndex)
                {
                    Loaded_Event?.Invoke();
                }
            };

#if UNITY_EDITOR
            ApplicationUtility.OnApplicationQuitEvent += delegate
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
            SceneManager.LoadScene(s_sceneInfo.SceneName);
        }

        /// <summary>
        /// Scene configurator should be registered on awake when target scene already loaded but you need to call Loaded_Event after some preparations.
        /// </summary>
        public static void WaitForConfigurator(ILoadDependency sceneConfigurator)
        {
            if (s_configurator != null)
            {
                throw new InvalidOperationException("Configurator already set for this scene instance.");
            }

            s_configurator = sceneConfigurator;

            IEnumerator WaitForConfigurator()
            {
                while (!s_configurator.Done)
                {
                    yield return null;
                }

                s_configurator = null;
                Loaded_Event?.Invoke();
            }

            TaskSystem.StartAsync(WaitForConfigurator());
        }

        public static void LoadScene(T sceneInfo)
        {
            BeginUnload_Event?.Invoke();

            s_sceneInfo = sceneInfo;

            SceneManager.LoadScene(s_sceneLoaderIndex);
        }
    }
}
