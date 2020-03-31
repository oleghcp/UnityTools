using System.Collections;

namespace UnityUtility.Async
{
    internal interface ITask
    {
        long Id { get; }
        bool IsPaused { get; }
        TaskInfo RunAsync(IEnumerator routine);
        void Pause();
        void Resume();
        void Add(IEnumerator routine);
        void StartRunning();
        void SkipCurrent();
        void Stop();
        void OnCoroutineEnded();
    }
}
