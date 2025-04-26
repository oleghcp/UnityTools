using System;
using System.Collections;
using System.Runtime.CompilerServices;
using OlegHcp.Tools;

namespace OlegHcp.Async
{
    public readonly struct TaskResult
    {
        public object Result { get; }
        public bool Successful { get; }

        internal TaskResult(object result, bool successful)
        {
            Result = result;
            Successful = successful;
        }
    }

    public readonly struct TaskInfo : IEquatable<TaskInfo>, IEnumerator
    {
        private readonly long _id;
        private readonly RoutineIterator _coroutine;

        /// <summary>
        /// Provides task ID.
        /// </summary>
        public long TaskId => _id;

        /// <summary>
        /// Returns true if the task is alive. The task is alive while it runs.
        /// </summary>
        public bool IsAlive => IsAliveInternal();

        public bool CanBeStopped => _coroutine.CanBeStopped;

        object IEnumerator.Current => null;

        internal TaskInfo(RoutineIterator coroutine)
        {
            _id = (_coroutine = coroutine).Id;
        }

        public void AddCompleteListener(Action<TaskResult> onComplete)
        {
            if (IsAliveInternal())
                _coroutine.OnCompleted1_Event += onComplete;
            else
                throw ThrowErrors.DeadTask();
        }

        public void AddCompleteListener(Action onComplete)
        {
            if (IsAliveInternal())
                _coroutine.OnCompleted2_Event += onComplete;
            else
                throw ThrowErrors.DeadTask();
        }

        /// <summary>
        /// Stops the task and marks it as non-alive.
        /// </summary>
        public void Stop()
        {
            if (IsAliveInternal())
                _coroutine.Stop();
        }

        #region Regular stuff
        public override bool Equals(object obj)
        {
            return obj is TaskInfo taskInfo && Equals(taskInfo);
        }

        public bool Equals(TaskInfo other)
        {
            return other._id == _id;
        }

        public override int GetHashCode()
        {
            return _id.GetHashCode();
        }

        public static bool operator ==(TaskInfo a, TaskInfo b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(TaskInfo a, TaskInfo b)
        {
            return !a.Equals(b);
        }
        #endregion

        #region IEnumerator implementation
        bool IEnumerator.MoveNext()
        {
            return IsAliveInternal();
        }

        void IEnumerator.Reset()
        {
            throw new NotImplementedException();
        }
        #endregion

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsAliveInternal()
        {
            return _coroutine != null && _coroutine.Id == _id;
        }
    }
}
