using System.Collections.Generic;
using UnityEngine;

namespace UnityUtility.Async
{
    internal class TaskDispatcher : MonoBehaviour
    {
        private List<TaskRunner> _tasks = new List<TaskRunner>();

        private void Update()
        {
            for (int i = 0; i < _tasks.Count; i++)
            {
                _tasks[i].Refresh();
            }
        }

        public void AddTaskRunner(TaskRunner runner)
        {
            _tasks.Add(runner);
        }
    }
}
