#if INCLUDE_UNITY_UI
using System;
using OlegHcp.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;
using static OlegHcp.GameConsole.CommandsHelper;

namespace OlegHcp.GameConsole
{
    public class TerminalCommands
    {
        private const string NULL = "Null";

#if UNITY_EDITOR || TOUCH_SCREEN
        [TerminalCommand, Preserve]
        internal bool help()
        {
            Terminal.I.WriteCommandList();
            return false;
        }
#endif

        [TerminalCommand, Preserve]
        internal bool clear()
        {
            Terminal.I.Clear();
            return false;
        }

        [TerminalCommand, Preserve]
        internal bool endian()
        {
            string endianType = BitConverter.IsLittleEndian ? "little" : "big";
            WriteLine($"Endian: {endianType}", LogType.Log);
            return false;
        }

        [TerminalCommand, Preserve]
        internal bool quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
            return false;
        }

        [TerminalCommand, Preserve]
        internal bool time_scale(string[] opt)
        {
            return ParseFloat(opt, scale => Time.timeScale = scale);
        }

        [TerminalCommand, Preserve]
        internal bool frame_rate(string[] opt)
        {
            return ParseInt(opt, frameRate => Application.targetFrameRate = frameRate);
        }

        [TerminalCommand, Preserve]
        internal bool vsync(string[] opt)
        {
            return ParseOnOff(opt, value => QualitySettings.vSyncCount = value.ToInt());
        }

        [TerminalCommand, Preserve]
        internal bool set_resolution(string[] opt)
        {
            bool successState = true;

            if (opt.Length != 2)
            {
                logError();
                return false;
            }

            Span<int> nums = stackalloc int[opt.Length];

            for (int i = 0; i < opt.Length; i++)
            {
                successState &= int.TryParse(opt[i], out nums[i]);
            }

            if (successState)
                Screen.SetResolution(nums[0], nums[1], Screen.fullScreen);
            else
                logError();

            return successState;

            void logError()
            {
                Terminal.I.WriteCmdError("Options: width height");
            }
        }

        [TerminalCommand, Preserve]
        internal void switch_fullscreen()
        {
            Screen.fullScreen = !Screen.fullScreen;
        }

        protected void WriteLine(string text)
        {
            Terminal.I.WriteLine(text ?? NULL);
        }

        protected void WriteLine(string text, in Color color)
        {
            Terminal.I.WriteLine(text ?? NULL, color);
        }

        protected void WriteLine(string text, LogType logType)
        {
            Terminal.I.WriteLine(text ?? NULL, logType);
        }

        protected void WriteLine(object obj)
        {
            Terminal.I.WriteLine(obj?.ToString() ?? NULL);
        }
    }
}
#endif
