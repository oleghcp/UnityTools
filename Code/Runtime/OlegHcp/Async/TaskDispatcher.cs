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
        private Stack<int> _indices = new Stack<int>();

        private void Awake()
        {
            _taskPool = new ObjectPool<TaskRunner>(this);
        }

        private void LateUpdate()
        {
            for (int i = 0; i < _activeTasks.Count; i++)
            {
                _activeTasks[i]?.Refresh();
            }
        }

        public void ReleaseRunner(TaskRunner runner, int index)
        {
            _indices.Push(index);
            _activeTasks[index] = null;
            _taskPool.Release(runner);
        }

        public TaskRunner GetRunner()
        {
            TaskRunner runner = _taskPool.Get();

            if (_indices.TryPop(out int index))
            {
                runner.SetIndex(index);
                return _activeTasks[index] = runner;
            }

            index = _activeTasks.Count;
            runner.SetIndex(index);
            return _activeTasks.Place(runner);
        }

        TaskRunner IObjectFactory<TaskRunner>.Create()
        {
            return gameObject.AddComponent<TaskRunner>()
                             .SetUp(this);
        }
    }
}
