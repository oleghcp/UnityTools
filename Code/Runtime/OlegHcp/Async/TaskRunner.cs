using System.Collections;
using System.Collections.Generic;
using System.Threading;
using OlegHcp.CSharp.Collections;
using OlegHcp.Pool;
using UnityEngine;

namespace OlegHcp.Async
{
    [DefaultExecutionOrder(10000)]
    internal class TaskRunner : MonoBehaviour
    {
        private ObjectPool<RoutineIterator> _iteratorPool;
        private bool _global;
        private List<RoutineIterator> _activeTasks = new List<RoutineIterator>();
        private Stack<int> _indices = new Stack<int>();

        public IReadOnlyList<RoutineIterator> ActiveTasks => _activeTasks;
        public int PoolCount => _iteratorPool.Count;

#if UNITY_EDITOR
        public int Version { get; private set; }
#endif

        private void OnDestroy()
        {
            if (_global)
                return;

            for (int i = 0; i < _activeTasks.Count; i++)
            {
                if (_activeTasks[i] != null)
                    _iteratorPool.Release(_activeTasks[i]);
            }
        }

        private void LateUpdate()
        {
            for (int i = 0; i < _activeTasks.Count; i++)
            {
                _activeTasks[i]?.Refresh();
            }
        }

        public TaskRunner SetUp(ObjectPool<RoutineIterator> iteratorPool, bool global)
        {
            _iteratorPool = iteratorPool;
            _global = global;
            return this;
        }

        public TaskInfo RunAsync(IEnumerator routine, long id, bool unstoppable, in CancellationToken token)
        {
            RoutineIterator iterator = GetIterator(out int index);
            iterator.Initialize(this, routine, id, unstoppable, index, token);
            TaskInfo task = new TaskInfo(iterator);
            StartCoroutine(iterator);
            return task;
        }

        public void ReleaseRunner(RoutineIterator iterator, int index)
        {
#if UNITY_EDITOR
            Version++;
#endif
            _indices.Push(index);
            _activeTasks[index] = null;
            _iteratorPool.Release(iterator);
        }

        private RoutineIterator GetIterator(out int index)
        {
#if UNITY_EDITOR
            Version++;
#endif
            if (_indices.TryPop(out index))
                return _activeTasks[index] = _iteratorPool.Get();

            index = _activeTasks.Count;
            return _activeTasks.Place(_iteratorPool.Get());
        }
    }
}
