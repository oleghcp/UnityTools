using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityUtility.Collections;
using UnityUtility.IdGenerating;
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
            _runnersPool = new ObjectPool<RoutineRunner>(global ? CreateGlobal : CreateLocal);
        }

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
            {
                _gameObject = new GameObject("LocalTasks");
                _runnersPool.Clear();
            }

            return Create();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private RoutineRunner Create()
        {
            return _gameObject.AddComponent<RoutineRunner>().SetUp(this);
        }
    }
}
