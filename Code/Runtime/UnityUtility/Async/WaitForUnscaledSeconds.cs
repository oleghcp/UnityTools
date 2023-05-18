using UnityEngine;

namespace UnityUtility.Async
{
    public class WaitForUnscaledSeconds : CustomYieldInstruction
    {
        private float _targetTime;
        private float _waitUntilTime = -1f;

        public override bool keepWaiting
        {
            get
            {
                if (_waitUntilTime < 0f)
                    _waitUntilTime = Time.realtimeSinceStartup + _targetTime;

                if (Time.realtimeSinceStartup < _waitUntilTime)
                    return true;

                _waitUntilTime = -1f;
                return false;
            }
        }

        public WaitForUnscaledSeconds(float time)
        {
            _targetTime = time;
        }
    }
}
