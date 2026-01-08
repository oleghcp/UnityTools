using System.Runtime.CompilerServices;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#else
using UnityEngine;
#endif

namespace OlegHcp.Tools
{
    internal static class ButtonUtility
    {
        public static bool BackQuoteDown
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if ENABLE_INPUT_SYSTEM
            get => Keyboard.current[Key.Backquote].wasPressedThisFrame;
#else
            get => Input.GetKeyDown(KeyCode.BackQuote);
#endif
        }

        public static bool BackQuotePressed
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if ENABLE_INPUT_SYSTEM
            get => Keyboard.current[Key.Backquote].isPressed;
#else
            get => Input.GetKey(KeyCode.BackQuote);
#endif
        }

        public static bool TabDown
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if ENABLE_INPUT_SYSTEM
            get => Keyboard.current[Key.Tab].wasPressedThisFrame;
#else
            get => Input.GetKeyDown(KeyCode.Tab);
#endif
        }

        public static bool UpArrowDown
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if ENABLE_INPUT_SYSTEM
            get => Keyboard.current[Key.UpArrow].wasPressedThisFrame;
#else
            get => Input.GetKeyDown(KeyCode.UpArrow);
#endif
        }

        public static bool DownArrowDown
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if ENABLE_INPUT_SYSTEM
            get => Keyboard.current[Key.DownArrow].wasPressedThisFrame;
#else
            get => Input.GetKeyDown(KeyCode.DownArrow);
#endif
        }
    }
}
