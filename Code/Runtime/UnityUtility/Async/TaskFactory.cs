using UnityEngine;
using UnityEngine.SceneManagement;
using UnityUtility.Collections;
using UnityUtility.IdGenerating;

namespace UnityUtility.Async
{
    internal interface IAsyncSettings
    {
        bool CanBeStopped { get; }
        bool CanBeStoppedGlobaly { get; }
        bool DoNotDestroyOnLoad { get; }
    }

    internal class TaskFactory
    {
        private class DefaultSettings : IAsyncSettings
        {
            bool IAsyncSettings.CanBeStopped => true;
            bool IAsyncSettings.CanBeStoppedGlobaly => false;
            bool IAsyncSettings.DoNotDestroyOnLoad => true;
        }

        private readonly string GAME_OBJECT_NAME;
        private readonly ObjectPool<RoutineRunner> m_runnersPool;
        private readonly IdGenerator<long> m_idProvider;

        private bool m_canBeStopped;
        private bool m_canBeStoppedGlobaly;
        private bool m_dontDestroyOnLoad;

        public bool CanBeStopped => m_canBeStopped;

        public TaskFactory(string gameObjectName = "Task")
        {
            GAME_OBJECT_NAME = gameObjectName;

            IAsyncSettings settings = GetSettings();

            m_canBeStopped = settings.CanBeStopped;
            m_canBeStoppedGlobaly = settings.CanBeStoppedGlobaly;
            m_dontDestroyOnLoad = settings.DoNotDestroyOnLoad;

            m_idProvider = new LongIdGenerator();
            m_runnersPool = new ObjectPool<RoutineRunner>(f_create);

            if (!m_dontDestroyOnLoad)
                SceneManager.sceneUnloaded += _ => m_runnersPool.Clear();
        }

        public long GetNewId()
        {
            return m_idProvider.GetNewId();
        }

        public void Release(ITask runner)
        {
            m_runnersPool.Release(runner as RoutineRunner);
        }

        public ITask GetRunner()
        {
            return m_runnersPool.Get();
        }

        // -- //

        private static IAsyncSettings GetSettings()
        {
            IAsyncSettings settings = Resources.Load<AsyncSystemSettings>(nameof(AsyncSystemSettings));

            return (settings == null ? new DefaultSettings() : settings) as IAsyncSettings;
        }

        private RoutineRunner f_create()
        {
            var taskRunner = Script.CreateInstance<RoutineRunner>(GAME_OBJECT_NAME);
            taskRunner.SetUp(this, m_dontDestroyOnLoad);
            return taskRunner;
        }
    }
}
