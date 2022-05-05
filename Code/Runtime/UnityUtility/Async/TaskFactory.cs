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

        private readonly ObjectPool<RoutineRunner> _runnersPool;
        private readonly IIdGenerator<long> _idProvider;
        private readonly bool _global;
        private ITaskStopper _stopper;
        private GameObject _gameObject;

        private readonly bool _canBeStopped;
        private readonly bool _canBeStoppedGlobally;

        public bool CanBeStopped => _canBeStopped;
        public bool CanBeStoppedGlobally => _canBeStoppedGlobally;
        public IIdGenerator<long> IdProvider => _idProvider;

        public TaskFactory(IAsyncSettings settings, IIdGenerator<long> idProvider, bool global)
        {
            _canBeStopped = settings.CanBeStopped;
            _canBeStoppedGlobally = settings.CanBeStoppedGlobally;
            _idProvider = idProvider;
            _global = global;

            if (global)
            {
                _runnersPool = new ObjectPool<RoutineRunner>(CreateGlobal);
            }
            else
            {
                _runnersPool = new ObjectPool<RoutineRunner>(CreateLocal);
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

        public void Release(RoutineRunner runner)
        {
            _runnersPool.Release(runner);
        }

        public RoutineRunner GetRunner()
        {
            return _runnersPool.Get();
        }

        // -- //

        private RoutineRunner CreateGlobal()
        {
            if (_gameObject == null)
                (_gameObject = new GameObject("Tasks")).Immortalize();

            return Create();
        }

        private RoutineRunner CreateLocal()
        {
            if (_gameObject == null)
                _gameObject = new GameObject("LocalTasks");

            return Create();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private RoutineRunner Create()
        {
            return _gameObject.AddComponent<RoutineRunner>().SetUp(this);
        }

        private void ActiveSceneChanged(Scene arg0, Scene arg1)
        {
            _gameObject = null;
            _runnersPool.Clear();
        }
    }
}
