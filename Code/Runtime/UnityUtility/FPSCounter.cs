using System;

namespace UnityUtility
{
    /// <summary>
    /// Counts FPS.
    /// </summary>
    [Serializable]
    public sealed class FpsCounter
    {
        private int _frameCounter;
        private float _timeCounter;
        private float _curFps;

        public float RefreshTime;

        public float FrameRate => _curFps;

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
                _frameCounter++;
                _timeCounter += deltaTime;

                if (_timeCounter >= RefreshTime)
                {
                    _curFps = _frameCounter / _timeCounter;

                    _frameCounter = 0;
                    _timeCounter = 0f;
                }
            }
            else
            {
                _curFps = 1f / deltaTime;
            }
        }
    }
}
