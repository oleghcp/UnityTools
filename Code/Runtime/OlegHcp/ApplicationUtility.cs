using System;
using OlegHcp.Engine;
using OlegHcp.Tools;
using UnityEngine;

namespace OlegHcp
{
    public static class ApplicationUtility
    {
        public static event Action<bool> OnApplicationPause_Event
        {
            add { _callbacks.OnApplicationPause_Event += value; }
            remove { _callbacks.OnApplicationPause_Event -= value; }
        }

        public static event Action<bool> OnApplicationFocus_Event
        {
            add { _callbacks.OnApplicationFocus_Event += value; }
            remove { _callbacks.OnApplicationFocus_Event -= value; }
        }

        public static event Action OnApplicationQuit_Event
        {
            add { _callbacks.OnApplicationQuit_Event += value; }
            remove { _callbacks.OnApplicationQuit_Event -= value; }
        }

        public static event Action OnUpdate_Event
        {
            add { _callbacks.OnUpdate_Event += value; }
            remove { _callbacks.OnUpdate_Event -= value; }
        }

        public static event Action<float> OnTick_Event
        {
            add { _callbacks.OnTick_Event += value; }
            remove { _callbacks.OnTick_Event -= value; }
        }

        public static event Action OnLateUpdate_Event
        {
            add { _callbacks.OnLateUpdate_Event += value; }
            remove { _callbacks.OnLateUpdate_Event -= value; }
        }

        public static event Action<float> OnLateTick_Event
        {
            add { _callbacks.OnLateTick_Event += value; }
            remove { _callbacks.OnLateTick_Event -= value; }
        }

        public static event Action OnFixedUpdate_Event
        {
            add { _callbacks.OnFixedUpdate_Event += value; }
            remove { _callbacks.OnFixedUpdate_Event -= value; }
        }

        public static event Action<float> OnFixedTick_Event
        {
            add { _callbacks.OnFixedTick_Event += value; }
            remove { _callbacks.OnFixedTick_Event -= value; }
        }

        private static CallbackKeeper _callbacks;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            _callbacks = ComponentUtility.CreateInstance<CallbackKeeper>();
            _callbacks.gameObject.Immortalize();
        }
    }
}
