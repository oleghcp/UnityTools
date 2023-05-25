#if INCLUDE_UNITY_UI
using System;
using UnityEngine;
using UnityEngine.Scripting;
using UnityUtility.Mathematics;
using static UnityUtility.GameConsole.CommandsHelper;

namespace UnityUtility.GameConsole
{
    public class TerminalCommands
    {
#if UNITY_EDITOR || TOUCH_SCREEN
        [TerminalCommand, Preserve]
        protected void help()
        {
            Terminal.I.WriteCommanList();
        }
#endif

        [TerminalCommand, Preserve]
        protected void clear()
        {
            Terminal.I.Clear();
        }

        [TerminalCommand, Preserve]
        protected void endian()
        {
            string message = BitConverter.IsLittleEndian ? "little" : "big";
            Terminal.I.WriteLine(message, LogType.Log);
        }

        [TerminalCommand, Preserve]
        protected bool quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
            return false;
        }

        [TerminalCommand, Preserve]
        protected bool time_scale(string[] opt)
        {
            return ParseFloat(opt, scale => Time.timeScale = scale);
        }

        [TerminalCommand, Preserve]
        protected bool frame_rate(string[] opt)
        {
            return ParseInt(opt, frameRate => Application.targetFrameRate = frameRate);
        }

        [TerminalCommand, Preserve]
        protected bool vsync(string[] opt)
        {
            return ParseOnOff(opt, value => QualitySettings.vSyncCount = value.ToInt());
        }
    }
}
#endif
