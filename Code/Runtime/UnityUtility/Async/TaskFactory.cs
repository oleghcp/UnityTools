using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityUtility.Collections;
using UnityUtility.IdGenerating;

namespace UnityUtility.Async
{
    public interface ITaskStopper
    {
        event Action StopAllTasks_Event;
    }

    internal class TaskFactory
    {
        public event Action StopTasks_Event;

        private readonly ObjectPool<RoutineRunner> m_runnersPool;
        private readonly IdGenerator<long> m_idProvider;

        private readonly bool m_canBeStopped;
        private readonly bool m_canBeStoppedGlobally;
        private readonly bool m_dontDestroyOnLoad;
        private GameObject m_gameObject;

        private ITaskStopper m_stopper;

        public bool CanBeStopped
        {
            get { return m_canBeStopped; }
        }

        public bool CanBeStoppedGlobally
        {
            get { return m_canBeStoppedGlobally; }
        }

        public TaskFactory(IAsyncSettings settings, IdGenerator<long> idProvider, bool doNotDestroyOnLoad)
        {
            m_canBeStopped = settings.CanBeStopped;
            m_canBeStoppedGlobally = settings.CanBeStoppedGlobally;
            m_dontDestroyOnLoad = doNotDestroyOnLoad;
            m_idProvider = idProvider;

            if (m_dontDestroyOnLoad)
            {
                (m_gameObject = new GameObject("Tasks")).Immortalize();
                m_runnersPool = new ObjectPool<RoutineRunner>(f_create);
            }
            else
            {
                m_runnersPool = new ObjectPool<RoutineRunner>(f_createLocal);
                SceneManager.sceneUnloaded += _ =>
                {
                    m_runnersPool.Clear();
                    m_gameObject = null;
                };
            }
        }

        public void RegisterStopper(ITaskStopper stopper)
        {
            if (!m_canBeStoppedGlobally)
            {
                throw new InvalidOperationException($"Tasks cannot be stopped due to the current system option. Check {TaskSystem.SYSTEM_NAME} settings.");
            }

            if (m_stopper != null)
            {
                throw new InvalidOperationException("Stop object is already set.");
            }

            (m_stopper = stopper).StopAllTasks_Event += () => StopTasks_Event?.Invoke();
        }

        public long GetNewId()
        {
            return m_idProvider.GetNewId();
        }

        public void Release(RoutineRunner runner)
        {
            m_runnersPool.Release(runner);
        }

        public RoutineRunner GetRunner()
        {
            return m_runnersPool.Get();
        }

        // -- //

        private RoutineRunner f_create()
        {
            var taskRunner = m_gameObject.AddComponent<RoutineRunner>();
            taskRunner.SetUp(this);
            return taskRunner;
        }

        private RoutineRunner f_createLocal()
        {
            if (m_gameObject == null)
                m_gameObject = new GameObject("LocalTasks");

            return f_create();
        }
    }
}
