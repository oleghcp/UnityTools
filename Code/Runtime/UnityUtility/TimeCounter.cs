using UnityUtility.MathExt;

namespace UnityUtility
{
    public struct TimeCounter
    {
        private float m_timeInterval;
        private float m_curTime;

        public float TimeInterval
        {
            get { return m_timeInterval; }
            set { m_timeInterval = value; }
        }

        public float CurTime
        {
            get { return m_curTime; }
            set { m_curTime = value; }
        }

        public float Shortage
        {
            get { return (m_timeInterval - m_curTime).CutBefore(0f); }
        }

        public TimeCounter(float timeInterval, bool initStartTime)
        {
            m_timeInterval = timeInterval;
            m_curTime = initStartTime ? timeInterval : 0f;
        }

        public bool HardChek(float deltaTime)
        {
            if (m_curTime >= m_timeInterval)
            {
                m_curTime = 0f;
                return true;
            }

            m_curTime += deltaTime;
            return false;
        }

        public bool SmoothChek(float deltaTime)
        {
            if (m_curTime >= m_timeInterval)
            {
                m_curTime -= m_timeInterval;
                return true;
            }

            m_curTime += deltaTime;
            return false;
        }

        public void Reset(bool toZero = false)
        {
            m_curTime = toZero ? 0f : m_timeInterval;
        }
    }
}
