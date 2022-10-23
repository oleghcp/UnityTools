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

        private static int _sceneLoaderIndex = -1;

        private static T _sceneInfo;
        private static LoadingWay _loadingWay;
        private static int _methodCode;

        public static T SceneInfo => _sceneInfo;

        protected static void SetUp(string transitionalSceneName)
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
                Loaded_Event = null;

                SceneManager.sceneUnloaded -= SceneUnloaded;
                SceneManager.activeSceneChanged -= ActiveSceneChanged;
            };
#endif
        }

        private IEnumerator Start()
        {
            yield return StartCoroutine(GetTransitionalRoutine());

            switch (_loadingWay)
            {
                case LoadingWay.Regular:
                    SceneManager.LoadScene(_sceneInfo.SceneName);
                    break;

                case LoadingWay.Async:
                    AsyncOperation loading = SceneManager.LoadSceneAsync(_sceneInfo.SceneName);
                    OnAsyncLoadingStarted(loading);
                    break;

                case LoadingWay.Custom:
                    OnCustomLoadRequested(_sceneInfo, _methodCode);
                    break;

                default:
                    throw new UnsupportedValueException(_loadingWay);
            }
        }

        public static void LoadScene(T sceneInfo)
        {
            LoadSceneLoader(sceneInfo, LoadingWay.Regular);
        }

        public static void LoadSceneAsync(T sceneInfo)
        {
            LoadSceneLoader(sceneInfo, LoadingWay.Async);
        }

        public static void LoadSceneCustom(T sceneInfo, int methodCode = 0)
        {
            _methodCode = methodCode;
            LoadSceneLoader(sceneInfo, LoadingWay.Custom);
        }

        protected virtual IEnumerator GetTransitionalRoutine()
        {
            yield break;
        }

        protected virtual void OnAsyncLoadingStarted(AsyncOperation loading)
        {

        }

        protected virtual void OnCustomLoadRequested(T sceneInfo, int methodCode)
        {
            throw new NotImplementedException();
        }

        private static void LoadSceneLoader(T sceneInfo, LoadingWay loading)
        {
            BeginUnload_Event?.Invoke();

            _sceneInfo = sceneInfo;
            _loadingWay = loading;

            SceneManager.LoadScene(_sceneLoaderIndex);
        }

        private static void SceneUnloaded(Scene scene)
        {
            if (scene.buildIndex != _sceneLoaderIndex && scene == SceneManager.GetActiveScene())
                Unloaded_Event?.Invoke();
        }

        private static void ActiveSceneChanged(Scene prevScene, Scene newScene)
        {
            if (newScene.buildIndex != _sceneLoaderIndex)
                Loaded_Event?.Invoke();
        }

        private enum LoadingWay
        {
            Regular,
            Async,
            Custom,
        }
    }
}
