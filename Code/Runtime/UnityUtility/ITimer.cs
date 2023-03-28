using System;

namespace UnityUtility
{
    public interface ITimer
    {
        event Action<ITimer> Elapsed_Event;

        bool IsRunning { get; }
        float TargetTime { get; }
        float CurrentTime { get; }
        float Progress { get; }
        bool ConsiderGlobalTimeScale { get; set; }

        void Start(float time);
        void Prolong(float extraTime);
        void Stop();
    }
}
