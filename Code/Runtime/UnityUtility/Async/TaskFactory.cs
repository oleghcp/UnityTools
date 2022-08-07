using UnityEngine;
using UnityUtility.IdGenerating;
using UnityUtility.Pool;

namespace UnityUtility.Async
{
    internal class TaskFactory : IObjectFactory<TaskRunner>
    {
        private readonly ObjectPool<TaskRunner> _taskPool;
        private readonly bool _global;
        private readonly string _dispatcherName;
        private TaskDispatcher _taskDispatcher;

        public IIdGenerator<long> IdProvider { get; }

        public TaskFactory(IIdGenerator<long> idProvider, bool global, string dispatcherName)
        {
            _global = global;
            _dispatcherName = dispatcherName;
            IdProvider = idProvider;
            _taskPool = new ObjectPool<TaskRunner>(this);
        }

        public void Release(TaskRunner runner)
        {
            _taskPool.Release(runner);
        }

        public TaskRunner Get()
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
