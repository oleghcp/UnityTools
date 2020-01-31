namespace UU
{
    /// <summary>
    /// Counts FPS.
    /// </summary>
    public sealed class FPSCounter
    {
        private float m_refreshTime;

        private int m_frameCounter;
        private float m_timeCounter;

        private float m_curFPS;

        public float RefreshTime
        {
            get { return m_refreshTime; }
            set { m_refreshTime = value; }
        }

        public float FrameRate
        {
            get { return m_curFPS; }
        }

        public FPSCounter(float refreshTime = 0.5f)
        {
            m_refreshTime = refreshTime;
        }

        /// <summary>
        /// Needs to be called in each frame.
        /// </summary>
        /// <param name="deltaTime">Time passed from previous frame.</param>
        public void Update(float deltaTime)
        {
            if (m_refreshTime > 0f)
            {
                m_frameCounter++;
                m_timeCounter += deltaTime;

                if (m_timeCounter >= m_refreshTime)
                {
                    m_curFPS = m_frameCounter / m_timeCounter;

                    m_frameCounter = 0;
                    m_timeCounter = 0f;
                }
            }
            else
            {
                m_curFPS = 1f / deltaTime;
            }
        }
    }
}
