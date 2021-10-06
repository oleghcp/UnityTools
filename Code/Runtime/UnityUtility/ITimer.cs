using System;

namespace UnityUtility
{
    public interface ITimer
    {
        bool IsRunning { get; }
        float TargetTime { get; }
        float CurrentTime { get; }
        float Progress { get; }
        float TimeScale { get; set; }

        void InitCallback(Action callback);

        void StartCountdown(float time, float timeScale = 1f);
        void StartCountdown(float time, Action callback);

        void Prolong(float extraTime);

        void StopCountdown();
    }
}
