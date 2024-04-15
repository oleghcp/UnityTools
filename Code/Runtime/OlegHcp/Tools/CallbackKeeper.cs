using System;
using UnityEngine;

namespace OlegHcp.Tools
{
    internal class CallbackKeeper : MonoBehaviour
    {
        public event Action<bool> OnApplicationPause_Event;
        public event Action<bool> OnApplicationFocus_Event;
        public event Action OnApplicationQuit_Event;

        public event Action OnUpdate_Event;
        public event Action<float> OnTick_Event;
        public event Action OnLateUpdate_Event;
        public event Action<float> OnLateTick_Event;
        public event Action OnFixedUpdate_Event;
        public event Action<float> OnFixedTick_Event;

#if UNITY_EDITOR
        private void OnDestroy()
        {
            OnApplicationPause_Event = null;
            OnApplicationFocus_Event = null;
            OnApplicationQuit_Event = null;
            OnUpdate_Event = null;
            OnTick_Event = null;
            OnLateUpdate_Event = null;
            OnLateTick_Event = null;
            OnFixedUpdate_Event = null;
            OnFixedTick_Event = null;
        }
#endif

        private void Update()
        {
            OnUpdate_Event?.Invoke();
            OnTick_Event?.Invoke(Time.deltaTime);
        }

        private void LateUpdate()
        {
            OnLateUpdate_Event?.Invoke();
            OnLateTick_Event?.Invoke(Time.deltaTime);
        }

        private void FixedUpdate()
        {
            OnFixedUpdate_Event?.Invoke();
            OnFixedTick_Event?.Invoke(Time.fixedDeltaTime);
        }

        private void OnApplicationPause(bool pause)
        {
            OnApplicationPause_Event?.Invoke(pause);
        }

        private void OnApplicationFocus(bool focus)
        {
            OnApplicationFocus_Event?.Invoke(focus);
        }

        private void OnApplicationQuit()
        {
            OnApplicationQuit_Event?.Invoke();
        }
    }
}
