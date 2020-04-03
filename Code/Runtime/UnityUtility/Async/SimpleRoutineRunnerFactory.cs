using UnityEngine;
using UnityEngine.SceneManagement;
using UnityUtility.Collections;
using UnityUtility.IdGenerating;

namespace UnityUtility.Async
{
    internal class SimpleRoutineRunnerFactory : ITaskFactory
    {
        private readonly string GAME_OBJECT_NAME;

        private readonly ObjectPool<RoutineRunner> m_runnersPool;
        private readonly IdGenerator<long> m_idProvider;
        private bool _disposed;

        public SimpleRoutineRunnerFactory(string gameObjectName = "Task")
        {
            GAME_OBJECT_NAME = gameObjectName;

            m_idProvider = new LongIdGenerator();
            m_runnersPool = new ObjectPool<RoutineRunner>(f_create);

            SceneManager.sceneUnloaded += SceneUnloadedHandler;
        }

        public long GetNewId()
        {
            return m_idProvider.GetNewId();
        }

        public void Release(ITask runner)
        {
            if (_disposed)
            {
                (runner as Poolable).CleanUp();
                (runner as RoutineRunner).Destroy();
                return;
            }

            m_runnersPool.Release(runner as RoutineRunner);
        }

        public ITask GetRunner()
        {
            return m_runnersPool.Get();
        }

        // -- //

        private RoutineRunner f_create()
        {
            var taskRunner = Script.CreateInstance<RoutineRunner>(GAME_OBJECT_NAME);
            taskRunner.SetUp(this);
            return taskRunner;
        }

        private void SceneUnloadedHandler(Scene _)
        {
            m_runnersPool.Clear();
        }

        #region IDisposable
        public void Dispose()
        {
            _disposed = true;
            SceneManager.sceneUnloaded -= SceneUnloadedHandler;
            m_runnersPool.Clear(item => item.Destroy());
        }
        #endregion
    }
}
