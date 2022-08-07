using System;
using UnityEngine;
using UnityUtility.IdGenerating;
using UnityUtility.Pool;

namespace UnityUtility.Async
{
    public interface ITaskStopper
    {
        event Action StopAllTasks_Event;
    }

    internal class TaskFactory : IObjectFactory<TaskRunner>
    {
        private readonly ObjectPool<TaskRunner> _taskPool;
        private readonly bool _global;
        private readonly string _dispatcherName;
        private readonly bool _canBeStopped;
        private TaskDispatcher _taskDispatcher;

        public bool CanBeStopped => _canBeStopped;
        public IIdGenerator<long> IdProvider { get; }

        public TaskFactory(IIdGenerator<long> idProvider, bool canBeStopped, bool global, string dispatcherName)
        {
            _global = global;
            _dispatcherName = dispatcherName;
            _canBeStopped = canBeStopped;
            IdProvider = idProvider;
            _taskPool = new ObjectPool<TaskRunner>(this);
        }

        public void RegisterStopper(ITaskStopper stopper)
        {
            stopper.StopAllTasks_Event += () =>
            {
                if (_taskDispatcher != null)
                    _taskDispatcher.StopAllTasks();
            };
        }

        public void Release(TaskRunner runner)
        {
            _taskPool.Release(runner);
        }

        public TaskRunner GetRunner()
        {
            return _taskPool.Get();
        }

        TaskRunner IObjectFactory<TaskRunner>.Create()
        {
            if (_taskDispatcher == null)
            {
                _taskDispatcher = ComponentUtility.CreateInstance<TaskDispatcher>(_dispatcherName);

                if (_global)
                    _taskDispatcher.Immortalize();
                else
                    _taskDispatcher.SetUp(_taskPool);
            }


            TaskRunner taskRunner = _taskDispatcher.gameObject.AddComponent<TaskRunner>();
            _taskDispatcher.AddTaskRunner(taskRunner);
            return taskRunner.SetUp(this);
        }
    }
}
