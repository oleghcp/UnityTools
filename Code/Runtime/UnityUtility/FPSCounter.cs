namespace UnityUtility
{
    /// <summary>
    /// Counts FPS.
    /// </summary>
    public sealed class FpsCounter
    {
        private int m_frameCounter;
        private float m_timeCounter;
        private float m_curFps;

        public float RefreshTime;

        public float FrameRate
        {
            get { return m_curFps; }
        }

        public FpsCounter(float refreshTime = 0.5f)
        {
            RefreshTime = refreshTime;
        }

        /// <summary>
        /// Needs to be called in each frame.
        /// </summary>
        /// <param name="deltaTime">Time passed from previous frame.</param>
        public void Update(float deltaTime)
        {
            if (RefreshTime > 0f)
            {
                m_frameCounter++;
                m_timeCounter += deltaTime;

                if (m_timeCounter >= RefreshTime)
                {
                    m_curFps = m_frameCounter / m_timeCounter;

                    m_frameCounter = 0;
                    m_timeCounter = 0f;
                }
            }
            else
            {
                m_curFps = 1f / deltaTime;
            }
        }
    }
}
