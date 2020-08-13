using System;
using UnityUtility.MathExt;

namespace UnityUtility
{
    [Serializable]
    public struct TimeCounter
    {
        public float TimeInterval;
        public float CurrentTime;

        public float Ratio
        {
            get { return CurrentTime / TimeInterval; }
            set { CurrentTime = TimeInterval * value.Clamp(0f, 1f); }
        }

        public float Shortage
        {
            get { return (TimeInterval - CurrentTime).CutBefore(0f); }
        }

        public TimeCounter(float timeInterval, bool initStartTime = false)
        {
            TimeInterval = timeInterval;
            CurrentTime = initStartTime ? timeInterval : 0f;
        }

        public bool HardCheck(float deltaTime)
        {
            CurrentTime += deltaTime;

            if (CurrentTime >= TimeInterval)
            {
                CurrentTime = 0f;
                return true;
            }

            return false;
        }

        public bool SmoothCheck(float deltaTime)
        {
            CurrentTime += deltaTime;

            if (CurrentTime >= TimeInterval)
            {
                CurrentTime -= TimeInterval;
                return true;
            }

            return false;
        }

        public void Reset(bool toZero = false)
        {
            CurrentTime = toZero ? 0f : TimeInterval;
        }
    }
}
