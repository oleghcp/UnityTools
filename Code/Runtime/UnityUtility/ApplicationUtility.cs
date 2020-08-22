using System;
using UnityEngine;

namespace UnityUtility
{
    public class ApplicationUtility : MonoBehaviour
    {
        public static Action<bool> OnApplicationPause_Event;
        public static Action<bool> OnApplicationFocus_Event;
        public static Action OnApplicationQuitEvent;

        public static event Action OnUpdate_Event;
        public static event Action<float> OnTick_Event;
        public static event Action OnLateUpdate_Event;
        public static event Action<float> OnLateTick_Event;
        public static event Action OnFixedUpdate_Event;
        public static event Action<float> OnFixedTick_Event;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void f_initialize()
        {
            ComponentUtility.CreateInstance<ApplicationUtility>()
                            .gameObject
                            .Immortalize();
        }

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
            OnApplicationQuitEvent?.Invoke();
        }
    }
}
