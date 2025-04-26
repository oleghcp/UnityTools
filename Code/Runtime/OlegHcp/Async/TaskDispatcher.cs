using System.Collections.Generic;
using OlegHcp.CSharp.Collections;
using OlegHcp.Pool;
using UnityEngine;

namespace OlegHcp.Async
{
    [DefaultExecutionOrder(10000)]
    internal class TaskDispatcher : MonoBehaviour, IObjectFactory<TaskRunner>
    {
        private ObjectPool<TaskRunner> _taskPool;
        private List<TaskRunner> _activeTasks = new List<TaskRunner>();

        private void Awake()
        {
            _taskPool = new ObjectPool<TaskRunner>(this);
        }

        private void LateUpdate()
        {
            for (int i = 0; i < _activeTasks.Count; i++)
            {
                _activeTasks[i].Refresh();
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
            return _activeTasks.Place(gameObject.AddComponent<TaskRunner>())
                               .SetUp(this);
        }
    }
}
