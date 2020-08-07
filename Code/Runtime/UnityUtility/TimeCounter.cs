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

        public bool HardChek(float deltaTime)
        {
            if (CurrentTime >= TimeInterval)
            {
                CurrentTime = 0f;
                return true;
            }

            CurrentTime += deltaTime;
            return false;
        }

        public bool SmoothChek(float deltaTime)
        {
            if (CurrentTime >= TimeInterval)
            {
                CurrentTime -= TimeInterval;
                return true;
            }

            CurrentTime += deltaTime;
            return false;
        }

        public void Reset(bool toZero = false)
        {
            CurrentTime = toZero ? 0f : TimeInterval;
        }
    }
}
