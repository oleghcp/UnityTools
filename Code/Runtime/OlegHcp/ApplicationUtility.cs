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
            add { CallbackKeeper.OnApplicationPause_Event += value; }
            remove { CallbackKeeper.OnApplicationPause_Event -= value; }
        }

        public static event Action<bool> OnApplicationFocus_Event
        {
            add { CallbackKeeper.OnApplicationFocus_Event += value; }
            remove { CallbackKeeper.OnApplicationFocus_Event -= value; }
        }

        public static event Action OnApplicationQuit_Event
        {
            add { CallbackKeeper.OnApplicationQuit_Event += value; }
            remove { CallbackKeeper.OnApplicationQuit_Event -= value; }
        }

        public static event Action OnUpdate_Event
        {
            add { CallbackKeeper.OnUpdate_Event += value; }
            remove { CallbackKeeper.OnUpdate_Event -= value; }
        }

        public static event Action<float> OnTick_Event
        {
            add { CallbackKeeper.OnTick_Event += value; }
            remove { CallbackKeeper.OnTick_Event -= value; }
        }

        public static event Action OnLateUpdate_Event
        {
            add { CallbackKeeper.OnLateUpdate_Event += value; }
            remove { CallbackKeeper.OnLateUpdate_Event -= value; }
        }

        public static event Action<float> OnLateTick_Event
        {
            add { CallbackKeeper.OnLateTick_Event += value; }
            remove { CallbackKeeper.OnLateTick_Event -= value; }
        }

        public static event Action OnFixedUpdate_Event
        {
            add { CallbackKeeper.OnFixedUpdate_Event += value; }
            remove { CallbackKeeper.OnFixedUpdate_Event -= value; }
        }

        public static event Action<float> OnFixedTick_Event
        {
            add { CallbackKeeper.OnFixedTick_Event += value; }
            remove { CallbackKeeper.OnFixedTick_Event -= value; }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            ComponentUtility.CreateInstance<CallbackKeeper>()
                            .gameObject
                            .Immortalize();
        }
    }
}
