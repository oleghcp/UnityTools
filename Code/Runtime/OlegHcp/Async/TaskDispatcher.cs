using System.Collections.Generic;
using OlegHcp.CSharp.Collections;
using OlegHcp.Pool;
using UnityEngine;

namespace OlegHcp.Async
{
    internal class TaskDispatcher : MonoBehaviour, IObjectFactory<TaskRunner>
    {
        private ObjectPool<TaskRunner> _taskPool;
        private List<TaskRunner> _tasks = new List<TaskRunner>();

        private void Awake()
        {
            _taskPool = new ObjectPool<TaskRunner>(this);
        }

        private void Update()
        {
            for (int i = 0; i < _tasks.Count; i++)
            {
                _tasks[i].Refresh();
            }
        }

        public void ReleaseRunner(TaskRunner runner)
        {
            _taskPool.Release(runner);
        }

        public TaskRunner GetRunner()
        {
            return _taskPool.Get();
        }

        TaskRunner IObjectFactory<TaskRunner>.Create()
        {
            TaskRunner taskRunner = _tasks.Place(gameObject.AddComponent<TaskRunner>());
            return taskRunner.SetUp(this);
        }
    }
}
