using UnityEngine;

namespace UnityUtility.Async
{
    public class AsyncSystemSettings : ScriptableObject, IAsyncSettings
    {
        [SerializeField]
        private bool _canBeStopped = true;
        [SerializeField]
        private bool _canBeStoppedGlobaly;
        [SerializeField]
        private bool _dontDestroyOnLoad = true;

        public bool CanBeStopped => _canBeStopped;
        public bool CanBeStoppedGlobaly => _canBeStopped && _canBeStoppedGlobaly;
        public bool DoNotDestroyOnLoad => !_canBeStopped && _dontDestroyOnLoad;
    }
}
