using System.Collections;
using System.Collections.Generic;
using System.Threading;
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

        public IReadOnlyList<TaskRunner> ActiveTasks => _activeTasks;

#if UNITY_EDITOR
        public int Version { get; private set; }
#endif

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

        public TaskInfo RunAsync(IEnumerator routine, long id, bool unstoppable, in CancellationToken token)
        {
            return GetRunner().RunAsync(routine, id, unstoppable, token);
        }

        public void ReleaseRunner(TaskRunner runner, int index)
        {
#if UNITY_EDITOR
            Version++;
#endif
            Debug.Log($"Count: {_activeTasks.Count} | index: {index}");
            _indices.Push(index);
            _activeTasks[index] = null;
            _taskPool.Release(runner);
        }

        TaskRunner IObjectFactory<TaskRunner>.Create()
        {
            return gameObject.AddComponent<TaskRunner>()
                             .SetUp(this);
        }

        private TaskRunner GetRunner()
        {
#if UNITY_EDITOR
            Version++;
#endif
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
    }
}
