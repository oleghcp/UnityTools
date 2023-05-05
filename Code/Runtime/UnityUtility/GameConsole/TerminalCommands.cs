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
        private void help()
        {
            Terminal.I.WriteCommanList();
        }
#endif

        [TerminalCommand, Preserve]
        private void clear()
        {
            Terminal.I.Clear();
        }

        [TerminalCommand, Preserve]
        private void endian()
        {
            string message = BitConverter.IsLittleEndian ? "little" : "big";
            Terminal.I.WriteLine(message, LogType.Log);
        }

        [TerminalCommand, Preserve]
        private bool quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
            return false;
        }

        [TerminalCommand, Preserve]
        private void time_scale(string[] opt)
        {
            ParseFloat(opt, scale => Time.timeScale = scale);
        }

        [TerminalCommand, Preserve]
        private void frame_rate(string[] opt)
        {
            ParseInt(opt, frameRate => Application.targetFrameRate = frameRate);
        }

        [TerminalCommand, Preserve]
        private bool vsync(string[] opt)
        {
            return ParseOnOff(opt, value => QualitySettings.vSyncCount = value.ToInt());
        }
    }
}
#endif
