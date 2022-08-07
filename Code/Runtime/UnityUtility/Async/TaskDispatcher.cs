using System.Collections.Generic;
using UnityEngine;
using UnityUtility.Pool;

namespace UnityUtility.Async
{
    internal class TaskDispatcher : MonoBehaviour
    {
        private ObjectPool<TaskRunner> _taskPool;
        private List<TaskRunner> _tasks = new List<TaskRunner>();

        private void Update()
        {
            for (int i = 0; i < _tasks.Count; i++)
            {
                _tasks[i].Refresh();
            }
        }

        private void OnDestroy()
        {
            _taskPool?.Clear();
        }

        public void SetUp(ObjectPool<TaskRunner> taskPool)
        {
            _taskPool = taskPool;
        }

        public void AddTaskRunner(TaskRunner runner)
        {
            _tasks.Add(runner);
        }
    }
}
