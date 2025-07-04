﻿using System;
using OlegHcp.Mathematics;

namespace OlegHcp
{
    /// <summary>
    /// Counts FPS.
    /// </summary>
    [Serializable]
    public class FpsCounter
    {
        private const float DEFAULT_REFRESH_TIME = 0.5f;

        private IntervalChecker _checker;
        private int _frameCounter;
        private float _currentFps;

        public float RefreshTime
        {
            get => _checker.Interval;
            set => _checker.Interval = value;
        }

        public float FrameRate => _currentFps;

        public FpsCounter(float refreshTime)
        {
            _checker = new IntervalChecker(refreshTime);
        }

        public FpsCounter()
        {
            _checker = new IntervalChecker(DEFAULT_REFRESH_TIME);
        }

        /// <summary>
        /// Needs to be called in each frame.
        /// </summary>
        /// <param name="deltaTime">Time passed from previous frame.</param>
        public bool Update(float deltaTime)
        {
            if (_checker.Interval <= MathUtility.kEpsilon)
            {
                _currentFps = 1f / deltaTime;
                return true;
            }

            _frameCounter++;

            if (_checker.SmoothCheckDelta(deltaTime))
            {
                _currentFps = _frameCounter / _checker.Interval;
                _frameCounter = 0;
                return true;
            }

            return false;
        }
    }
}
