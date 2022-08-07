using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityUtility.IdGenerating;
using UnityUtility.Pool;
using UnityUtilityTools;

namespace UnityUtility.Async
{
    public interface ITaskStopper
    {
        event Action StopAllTasks_Event;
    }

    internal class TaskFactory
    {
        public event Action StopTasks_Event;

        private readonly ObjectPool<TaskRunner> _runnersPool;
        private readonly IIdGenerator<long> _idProvider;
        private readonly bool _global;
        private ITaskStopper _stopper;
        private TaskDispatcher _taskDispatcher;

        private readonly bool _canBeStopped;
        private readonly bool _canBeStoppedGlobally;

        public bool CanBeStopped => _canBeStopped;
        public bool CanBeStoppedGlobally => _canBeStoppedGlobally;
        public IIdGenerator<long> IdProvider => _idProvider;
        public TaskDispatcher TaskDispatcher => _taskDispatcher;

        public TaskFactory(IAsyncSettings settings, IIdGenerator<long> idProvider, bool global)
        {
            _canBeStopped = settings.CanBeStopped;
            _canBeStoppedGlobally = settings.CanBeStoppedGlobally;
            _idProvider = idProvider;
            _global = global;

            if (global)
            {
                _runnersPool = new ObjectPool<TaskRunner>(CreateGlobal);
            }
            else
            {
                _runnersPool = new ObjectPool<TaskRunner>(CreateLocal);
                SceneManager.activeSceneChanged += ActiveSceneChanged;
            }
        }

#if UNITY_EDITOR
        public void CleanUp()
        {
            if (!_global)
                SceneManager.activeSceneChanged -= ActiveSceneChanged;
        }
#endif

        public void RegisterStopper(ITaskStopper stopper)
        {
            if (!_canBeStoppedGlobally)
                throw Errors.CannotStopTask();

            if (_stopper != null)
                throw new InvalidOperationException("Stoping object is already set.");

            (_stopper = stopper).StopAllTasks_Event += () => StopTasks_Event?.Invoke();
        }

        public void Release(TaskRunner runner)
        {
            _runnersPool.Release(runner);
        }

        public TaskRunner GetRunner()
        {
            return _runnersPool.Get();
        }

        // -- //

        private TaskRunner CreateGlobal()
        {
            if (_taskDispatcher == null)
                (_taskDispatcher = ComponentUtility.CreateInstance<TaskDispatcher>("Tasks")).Immortalize();

            return Create();
        }

        private TaskRunner CreateLocal()
        {
            if (_taskDispatcher == null)
                _taskDispatcher = ComponentUtility.CreateInstance<TaskDispatcher>("LocalTasks");

            return Create();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private TaskRunner Create()
        {
            return _taskDispatcher.gameObject
                                  .AddComponent<TaskRunner>()
                                  .SetUp(this);
        }

        private void ActiveSceneChanged(Scene arg0, Scene arg1)
        {
            _taskDispatcher = null;
            _runnersPool.Clear();
        }
    }
}
