using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityUtility.Scripts;

#pragma warning disable CS0649
namespace UnityUtility.GameConsole
{
    public class Terminal : SingleUIScript<Terminal>
    {
        private readonly Color CMD_COLOR = Color.white;
        private readonly Color CMD_ERROR_COLOR = new Color(1f, 0.5f, 0f, 1f);

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

        private bool m_showCallStack;

        public bool IsOn
        {
            get { return m_isOn; }
        }

        ///////////////
        //Unity funcs//
        ///////////////

        protected override void Construct()
        {
            m_pointerEventData = new PointerEventData(EventSystem.current);
            m_stringBuilder = new StringBuilder();

            m_invokeParams = new string[1][];

            m_cmdHistory = new List<string>() { string.Empty };

            Application.logMessageReceived += DebugLogHandler;
        }

        protected override void CleanUp()
        {
#if UNITY_EDITOR
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
        /// <param name="commands">An object which contans command functions.</param>
        public static void CreateTerminal(object commands)
        {
            GameObject newCanvas = Resources.Load<GameObject>("TerminalCanvas").Install();
            newCanvas.Immortalize();
            Terminal terminal = newCanvas.GetComponentInChildren<Terminal>();
            terminal.SetCommands(commands);
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

        public void OnDone(string msg)
        {
            if (!m_isOn) { return; }

            f_enterCmd(_field.text);

            _field.text = string.Empty;
            _field.OnPointerClick(m_pointerEventData);
        }

        public void ShowCallStackForLogs(bool enable)
        {
            m_showCallStack = enable;
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

        public void Clear()
        {
            _log.Clear();
        }

        ///////////////
        //Inner funcs//
        ///////////////

        private void f_enterCmd(string text)
        {
            if (m_cmdRun == null)
            {
                WriteLine("Commands are not set.", Color.red);
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
                WriteLine("Commands are not set.", Color.red);
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
                _field.text = cmds[0];
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

        private void DebugLogHandler(string msg, string stackTrace, LogType logType)
        {
            Color colour;

            switch (logType)
            {
                case LogType.Error:
                case LogType.Exception:
                    colour = Color.red;
                    break;

                case LogType.Assert:
                    colour = Color.green;
                    break;

                case LogType.Warning:
                    colour = Color.yellow;
                    break;

                default:
                    colour = Color.cyan;
                    break;
            }

            _log.WriteLine(msg, colour);

            if (m_showCallStack)
                _log.WriteLine(stackTrace, Color.gray);
        }

        ////////////
        //Routines//
        ////////////

        private IEnumerator Switch(bool on)
        {
            _field.gameObject.SetActive(m_isOn = on);

            if (on)
                _field.text = string.Empty;
            else
                m_curHistoryIndex = 0;

            float startPos = rectTransform.anchoredPosition.y;
            float endPos = on ? -360f : 0f;

            float ratio = 0f;

            while (ratio < 1f)
            {
                ratio += Time.unscaledDeltaTime * 10f;
                rectTransform.anchoredPosition = new Vector2(0f, Mathf.Lerp(startPos, endPos, ratio));

                yield return null;
            }

            if (on)
                _field.OnPointerClick(m_pointerEventData);
        }
    }
}
