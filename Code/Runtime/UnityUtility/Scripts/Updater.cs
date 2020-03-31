using System;
using UnityEngine;

namespace UnityUtility.Scripts
{
    internal class Updater : Script
    {
        public static event Action Frame_Event;

        static Updater()
        {
            Updater instance = CreateInstance<Updater>();
            instance.gameObject.Immortalize();
        }

        private void Update()
        {
            Frame_Event?.Invoke();
        }
    }
}
