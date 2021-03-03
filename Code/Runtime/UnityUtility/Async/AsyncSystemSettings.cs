using UnityEngine;

namespace UnityUtility.Async
{
    public class AsyncSystemSettings : ScriptableObject, IAsyncSettings
    {
        [SerializeField]
        private bool _canBeStopped = true;
        [SerializeField]
        private bool _canBeStoppedGlobally = false;

        public bool CanBeStopped => _canBeStopped;
        public bool CanBeStoppedGlobally => _canBeStopped && _canBeStoppedGlobally;

#if UNITY_EDITOR
        public static string CanBeStoppedName => nameof(_canBeStopped);
        public static string CanBeStoppedGloballyName => nameof(_canBeStoppedGlobally);
#endif
    }
}
