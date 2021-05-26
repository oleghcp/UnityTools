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

        private readonly ObjectPool<RoutineRunner> _runnersPool;
        private readonly IIdGenerator<long> _idProvider;
        private ITaskStopper _stopper;
        private GameObject _gameObject;

        private readonly bool _canBeStopped;
        private readonly bool _canBeStoppedGlobally;
        private readonly bool _dontDestroyOnLoad;

        public bool CanBeStopped => _canBeStopped;
        public bool CanBeStoppedGlobally => _canBeStoppedGlobally;
        public IIdGenerator<long> IdProvider => _idProvider;

        public TaskFactory(IAsyncSettings settings, IIdGenerator<long> idProvider, bool doNotDestroyOnLoad)
        {
            _canBeStopped = settings.CanBeStopped;
            _canBeStoppedGlobally = settings.CanBeStoppedGlobally;
            _dontDestroyOnLoad = doNotDestroyOnLoad;
            _idProvider = idProvider;

            if (_dontDestroyOnLoad)
            {
                (_gameObject = new GameObject("Tasks")).Immortalize();
                _runnersPool = new ObjectPool<RoutineRunner>(Create);
            }
            else
            {
                _runnersPool = new ObjectPool<RoutineRunner>(CreateLocal);
                SceneManager.sceneUnloaded += _ =>
                {
                    _runnersPool.Clear();
                    _gameObject = null;
                };
            }
        }

        public void RegisterStopper(ITaskStopper stopper)
        {
            if (!_canBeStoppedGlobally)
                throw new InvalidOperationException($"Tasks cannot be stopped due to the current system option. Check {TaskSystem.SYSTEM_NAME} settings.");

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

        private RoutineRunner Create()
        {
            var taskRunner = _gameObject.AddComponent<RoutineRunner>();
            taskRunner.SetUp(this);
            return taskRunner;
        }

        private RoutineRunner CreateLocal()
        {
            if (_gameObject == null)
                _gameObject = new GameObject("LocalTasks");

            return Create();
        }
    }
}
