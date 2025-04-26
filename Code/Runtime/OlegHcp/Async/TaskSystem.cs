using System;
using System.Collections;
using System.Threading;
using OlegHcp.IdGenerating;
using OlegHcp.Pool;
using OlegHcp.Tools;

namespace OlegHcp.Async
{
    /// <summary>
    /// Static coroutine runner. Allows to run coroutines from non-behaviuor objects.
    /// </summary>
    public static class TaskSystem
    {
        private static readonly Data _data = new Data();

        /// <summary>
        /// The same as MonoBehaviour's StartCoroutine.
        /// </summary>
        public static TaskInfo StartAsync(IEnumerator routine, in CancellationToken token)
        {
            return _data.GetGlobal().RunAsync(routine, _data.GetNewId(), false, token);
        }

        /// <summary>
        /// The same as MonoBehaviour's StartCoroutine. Call of Stop() method is prohibited.
        /// </summary>
        public static TaskInfo StartAsync(IEnumerator routine, bool unstoppable = false)
        {
            return _data.GetGlobal().RunAsync(routine, _data.GetNewId(), unstoppable, default);
        }

        /// <summary>
        /// The same as MonoBehaviour's StartCoroutine (just for current scene).
        /// </summary>
        public static TaskInfo StartAsyncLocally(IEnumerator routine, in CancellationToken token)
        {
            return _data.GetLocal().RunAsync(routine, _data.GetNewId(), false, token);
        }

        /// <summary>
        /// The same as MonoBehaviour's StartCoroutine (just for current scene). Call of Stop() method is prohibited.
        /// </summary>
        public static TaskInfo StartAsyncLocally(IEnumerator routine, bool unstoppable = false)
        {
            return _data.GetLocal().RunAsync(routine, _data.GetNewId(), unstoppable, default);
        }

        /// <summary>
        /// Runs a referenced function after delay.
        /// </summary>
        public static TaskInfo RunDelayed(float time, Action run, bool scaledTime = true, in CancellationToken token = default)
        {
            return _data.GetGlobal().RunAsync(CoroutineUtility.GetRunDelayedRoutine(time, run, scaledTime), _data.GetNewId(), false, token);
        }

        /// <summary>
        /// Runs a referenced function after delay (just for current scene).
        /// </summary>
        public static TaskInfo RunDelayedLocally(float time, Action run, bool scaledTime = true, in CancellationToken token = default)
        {
            return _data.GetLocal().RunAsync(CoroutineUtility.GetRunDelayedRoutine(time, run, scaledTime), _data.GetNewId(), false, token);
        }

        /// <summary>
        /// Runs a referenced function after specified frames count.
        /// </summary>
        public static TaskInfo RunAfterFrames(int frames, Action run, in CancellationToken token = default)
        {
            return _data.GetGlobal().RunAsync(CoroutineUtility.GetRunAfterFramesRoutine(frames, run), _data.GetNewId(), false, token);
        }

        /// <summary>
        /// Runs a referenced function after specified frames count (just for current scene).
        /// </summary>
        public static TaskInfo RunAfterFramesLocally(int frames, Action run, in CancellationToken token = default)
        {
            return _data.GetLocal().RunAsync(CoroutineUtility.GetRunAfterFramesRoutine(frames, run), _data.GetNewId(), false, token);
        }

        /// <summary>
        /// Runs a referenced function when <paramref name="condition"/> is true.
        /// </summary>
        public static TaskInfo RunByCondition(Func<bool> condition, Action run, in CancellationToken token = default)
        {
            return _data.GetGlobal().RunAsync(CoroutineUtility.GetRunByConditionRoutine(condition, run), _data.GetNewId(), false, token);
        }

        /// <summary>
        /// Runs a referenced function when <paramref name="condition"/> is true (just for current scene).
        /// </summary>
        public static TaskInfo RunByConditionLocally(Func<bool> condition, Action run, in CancellationToken token = default)
        {
            return _data.GetLocal().RunAsync(CoroutineUtility.GetRunByConditionRoutine(condition, run), _data.GetNewId(), false, token);
        }

        /// <summary>
        /// Repeats a referenced function each frame while <paramref name="condition"/> is true.
        /// </summary>
        public static TaskInfo Repeat(Func<bool> condition, Action run, in CancellationToken token = default)
        {
            return _data.GetGlobal().RunAsync(CoroutineUtility.GetRunWhileRoutine(condition, run), _data.GetNewId(), false, token);
        }

        /// <summary>
        /// Repeats a referenced function each frame while <paramref name="condition"/> is true (just for current scene).
        /// </summary>
        public static TaskInfo RepeatLocally(Func<bool> condition, Action run, in CancellationToken token = default)
        {
            return _data.GetLocal().RunAsync(CoroutineUtility.GetRunWhileRoutine(condition, run), _data.GetNewId(), false, token);
        }

        private class Data : IObjectFactory<RoutineIterator>
        {
            private LongIdGenerator _idProvider = new LongIdGenerator();
            private ObjectPool<RoutineIterator> _iteratorPool;
            private TaskRunner _globals;
            private TaskRunner _locals;

            internal ObjectPool<RoutineIterator> IteratorPool => _iteratorPool;

            public Data()
            {
                _iteratorPool = new ObjectPool<RoutineIterator>(this);
            }

            public long GetNewId()
            {
                return _idProvider.GetNewId();
            }

            public TaskRunner GetGlobal()
            {
                if (_globals == null)
                {
                    _globals = SceneTool.GetGlobal()
                                        .AddComponent<TaskRunner>()
                                        .SetUp(_iteratorPool, true);
                }

                return _globals;
            }

            public TaskRunner GetLocal()
            {
                if (_locals == null)
                {
                    _locals = SceneTool.GetLocal()
                                       .AddComponent<TaskRunner>()
                                       .SetUp(_iteratorPool, false);
                }

                return _locals;
            }

            RoutineIterator IObjectFactory<RoutineIterator>.Create()
            {
                return new RoutineIterator();
            }
        }
    }
}
