using System.Collections;

namespace UU.Async
{
    internal interface ITask
    {
        ulong Id { get; }
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
