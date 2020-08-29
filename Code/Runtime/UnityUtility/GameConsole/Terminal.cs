using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityUtility.SingleScripts;
using static UnityEngine.RectTransform;

#pragma warning disable CS0649
namespace UnityUtility.GameConsole
{
    public class TerminalOptions
    {
        /// <summary>Value of termial height relative screen (from 0f to 1f).</summary>
        public float TargetHeight = 0.75f;
        public int LinesLimit = 100;
        public bool AddSpaceAfterName = true;
        public bool ShowDebugLogs = true;
        public bool ShowCallStackForLogs = true;
    }

    public sealed class Terminal : SingleUiBehaviour<Terminal>
    {
        private readonly Color CMD_COLOR = Colours.White;
        private readonly Color CMD_ERROR_COLOR = Colours.Orange;

        public static event Action<bool> Switched_Event;

        [SerializeField]
        private InputField _field;
        [SerializeField]
        private LogController _log;

        private bool m_isOn;
        private PointerEventData m_pointerEventData;
        private StringBuilder m_stringBuilder;

        private object m_cmdRun;
        private Dictionary<string, MethodInfo> m_commands;

        private string[][] m_invokeParams;

        private List<string> m_cmdHistory;
        private int m_curHistoryIndex;

        private TerminalOptions m_options;

        public bool IsOn
        {
            get { return m_isOn; }
        }

        public TerminalOptions Options
        {
            get { return m_options; }
        }

        ///////////////
        //Unity funcs//
        ///////////////

        protected override void Construct()
        {
            _log.SetUp(this);

            m_pointerEventData = new PointerEventData(EventSystem.current);
            m_stringBuilder = new StringBuilder();

            m_invokeParams = new string[1][];

            m_cmdHistory = new List<string>() { string.Empty };

#if UNITY_EDITOR
            if (Application.isPlaying)
#endif
                Application.logMessageReceived += DebugLogHandler;
        }

        protected override void CleanUp()
        {
#if UNITY_EDITOR
            Switched_Event = null;
            Application.logMessageReceived -= DebugLogHandler;
#endif
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.BackQuote))
            {
                f_switch();
                Switched_Event?.Invoke(m_isOn);
            }

            if (m_isOn)
            {
                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    f_findCmd(_field.text);
                }

                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    f_switchHistoryLine(true);
                }
                else if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    f_switchHistoryLine(false);
                }

                float axis = Input.GetAxis("Mouse ScrollWheel");
                if (axis != 0) { _log.Scroll(axis); }
            }
        }

        ////////////////
        //Public funcs//
        ////////////////

        /// <summary>
        /// Creates a command line (aka console/terminal). Takes command container.
        /// Commands are just functions of the view:
        /// public string commandname(string[] options).
        /// It should return error description (if options are parsed with error) or null (if options parsed well or there are no options at all). 
        /// </summary>
        public static void CreateTerminal()
        {
            f_CreateTerminal();
            I.SetOptions(new TerminalOptions());
        }

        /// <summary>
        /// Creates a command line (aka console/terminal). Takes command container.
        /// Commands are just functions of the view:
        /// public string commandname(string[] options).
        /// It should return error description (if options are parsed with error) or null (if options parsed well or there are no options at all). 
        /// </summary>
        /// <param name="commands">An object which contans command functions.</param>
        public static void CreateTerminal(object commands)
        {
            f_CreateTerminal();
            I.SetOptions(new TerminalOptions());
            I.SetCommands(commands);
        }

        /// <summary>
        /// Creates a command line (aka console/terminal). Takes command container.
        /// Commands are just functions of the view:
        /// public string commandname(string[] options).
        /// It should return error description (if options are parsed with error) or null (if options parsed well or there are no options at all). 
        /// </summary>
        /// <param name="commands">An object which contans command functions.</param>
        public static void CreateTerminal(object commands, TerminalOptions options)
        {
            f_CreateTerminal();
            I.SetCommands(commands);
            I.SetOptions(options);
        }

        /// <summary>
        /// Sets a new command container.
        /// </summary>
        /// <param name="commands">An object which contans command functions.</param>
        public void SetCommands(object commands)
        {
            m_cmdRun = commands;
            Type t = m_cmdRun.GetType();
            MethodInfo[] funcs = t.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
            m_commands = funcs.ToDictionary(itm => itm.Name, itm => itm);
        }

        public void SetOptions(TerminalOptions options)
        {
            m_options = options;
        }

        public void GetCmd()
        {
            f_findCmd(string.Empty);
        }

        public void Switch()
        {
            f_switch();
        }

        public void WriteLine(string txt, Color color)
        {
            _log.WriteLine(txt, color);
        }

        public void WriteLine(string txt, LogType logType)
        {
            _log.WriteLine(txt, f_getTextColor(logType));
        }

        public void Clear()
        {
            _log.Clear();
        }

        public void OnDone(string msg)
        {
            if (!m_isOn) { return; }

            f_enterCmd(_field.text);

            _field.text = string.Empty;
            _field.OnPointerClick(m_pointerEventData);
        }

        ///////////////
        //Inner funcs//
        ///////////////

        private void f_enterCmd(string text)
        {
            if (m_cmdRun == null)
            {
                _log.WriteLine("Commands are not set.", Colours.Red);
                return;
            }

            if (text.HasUsefulData())
            {
                text = text.ToLower();

                string[] words = text.Split(' ');
                string command = words[0];

                if (m_commands.TryGetValue(command, out MethodInfo method))
                {
                    if (words.Length > 1)
                    {
                        m_invokeParams[0] = new string[words.Length - 1];
                        Array.Copy(words, 1, m_invokeParams[0], 0, m_invokeParams[0].Length);
                    }

                    object cmdKeysParseError = method.Invoke(m_cmdRun, m_invokeParams);
                    m_invokeParams[0] = null;

                    if (cmdKeysParseError == null)
                        _log.WriteLine(text, CMD_COLOR);
                    else
                        _log.WriteLine("cmd options error: " + cmdKeysParseError, CMD_ERROR_COLOR);
                }
                else
                {
                    _log.WriteLine("unknown: " + command, CMD_ERROR_COLOR);
                }

                m_cmdHistory.Add(text);
                m_curHistoryIndex = 0;
            }
        }

        private void f_findCmd(string text)
        {
            if (m_cmdRun == null)
            {
                _log.WriteLine("Commands are not set.", Colours.Red);
                return;
            }

            text = text.ToLower();

            string[] cmds = m_commands.Keys.Where(itm => itm.IndexOf(text) == 0).OrderBy(itm => itm).ToArray();

            if (cmds.Length == 0)
            {
                _field.text = string.Empty;
            }
            else if (cmds.Length == 1)
            {
                _field.text = m_options.AddSpaceAfterName ? cmds[0] + " " : cmds[0];
                _field.caretPosition = _field.text.Length;
            }
            else
            {
                for (int i = 0; i < cmds.Length; i++)
                {
                    m_stringBuilder.Append(cmds[i]).Append("   ");
                }

                string line = m_stringBuilder.ToString();
                m_stringBuilder.Clear();

                _log.WriteLine(line, CMD_COLOR);
                _field.text = text + f_getCommon(cmds, text.Length);
                _field.caretPosition = _field.text.Length;
            }
        }

        private string f_getCommon(string[] strings, int startIndex = 0)
        {
            string baseStr = strings[0];

            int characters = 0;

            for (int j = startIndex; j < baseStr.Length; j++)
            {
                for (int i = 1; i < strings.Length; i++)
                {
                    if (baseStr[j] != strings[i][j])
                    {
                        return baseStr.Substring(startIndex, characters);
                    }
                }

                characters++;
            }

            return baseStr.Substring(startIndex, characters);
        }

        private void f_switchHistoryLine(bool back)
        {
            if (m_cmdHistory.Count == 0) { return; }

            if (back)
            {
                m_curHistoryIndex = m_curHistoryIndex - 1;
                if (m_curHistoryIndex < 0) { m_curHistoryIndex = m_cmdHistory.Count - 1; }
            }
            else
            {
                m_curHistoryIndex = (m_curHistoryIndex + 1) % m_cmdHistory.Count;
            }

            _field.text = m_cmdHistory[m_curHistoryIndex];
            _field.caretPosition = _field.text.Length;
        }

        private void f_switch()
        {
            StopAllCoroutines();
            StartCoroutine(Switch(!m_isOn));
        }

        private Color f_getTextColor(LogType logType)
        {
            switch (logType)
            {
                case LogType.Error:
                case LogType.Exception:
                case LogType.Assert:
                    return Colours.Red;

                case LogType.Warning:
                    return Colours.Yellow;

                case LogType.Log:
                    return Colours.Cyan;

                default:
                    throw new UnsupportedValueException(logType);
            }
        }

        private void DebugLogHandler(string msg, string stackTrace, LogType logType)
        {
            if (m_options.ShowDebugLogs)
            {
                _log.WriteLine(msg, f_getTextColor(logType));

                if (m_options.ShowCallStackForLogs)
                    _log.WriteLine(stackTrace, Colours.Silver);
            }
        }

        private static void f_CreateTerminal()
        {
            Resources.Load<GameObject>("TerminalCanvas")
                     .Install()
                     .Immortalize();
        }

        ////////////
        //Routines//
        ////////////

        private IEnumerator Switch(bool on)
        {
            float targetHeight = 720f * m_options.TargetHeight;

            _field.gameObject.SetActive(m_isOn = on);

            if (on)
                _field.text = string.Empty;
            else
                m_curHistoryIndex = 0;

            float hStart = on ? 0f : targetHeight;
            float hEnd = on ? targetHeight : 0f;

            float ratio = 0f;

            while (ratio < 1f)
            {
                ratio += Time.unscaledDeltaTime * 10f;
                rectTransform.SetSizeWithCurrentAnchors(Axis.Vertical, Mathf.Lerp(hStart, hEnd, ratio));

                yield return null;
            }

            if (on)
                _field.OnPointerClick(m_pointerEventData);
        }
    }
}
