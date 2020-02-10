using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Text;
using UU.Scripts;

namespace UU.GameConsole
{
    public class Terminal : SingleUIScript<Terminal>
    {
        private readonly Color CMD_COLOR = Color.white;
        private readonly Color UPS_COLOR = new Color(1f, 0.5f, 0f, 1f);

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

        private object[] m_invokeParams;
        private List<string> m_cmdKeys;

        private List<string> m_cmdHistory;
        private int m_curHistoryIndex;

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

            m_cmdKeys = new List<string>();
            m_invokeParams = new[] { m_cmdKeys };

            m_cmdHistory = new List<string>() { string.Empty };

            Msg.Log_Event += DebugLogHandler;
        }

        protected override void CleanUp()
        {
#if UNITY_EDITOR
            Msg.Log_Event -= DebugLogHandler;
#endif
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.BackQuote))
            {
                fSwitch();
                Switched_Event?.Invoke(m_isOn);
            }

            if (m_isOn)
            {
                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    fFindCmd(_field.text);
                }

                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    fSwitchHistoryLine(true);
                }
                else if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    fSwitchHistoryLine(false);
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
        /// public string commandname(List<string> options).
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

            fEnterCmd(_field.text);

            _field.text = string.Empty;
            _field.OnPointerClick(m_pointerEventData);
        }

        public void GetCmd()
        {
            fFindCmd(string.Empty);
        }

        public void Switch()
        {
            fSwitch();
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

        private void fEnterCmd(string text)
        {
            if (m_cmdRun == null)
            {
                WriteLine("Commands are not set.", Color.red);
                return;
            }

            if (text.Length == 0) { return; }

            text = text.ToLower();

            if (text.HasUsefulData())
            {
                m_cmdKeys.Clear();
                m_cmdKeys.AddRange(text.Split(' '));
                string cmd = m_cmdKeys.PullOut(0);

                if (m_commands.ContainsKey(cmd))
                {
                    object keys = m_commands[cmd].Invoke(m_cmdRun, m_invokeParams);
                    if (keys == null) { _log.WriteLine(text, CMD_COLOR); }
                    else { _log.WriteLine(string.Concat("cmd options: ", cmd, " ", keys), UPS_COLOR); }
                }
                else { _log.WriteLine("unknown: " + cmd, UPS_COLOR); }

                m_cmdHistory.Add(text);
                m_curHistoryIndex = 0;
            }
        }

        private void fFindCmd(string text)
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
                m_stringBuilder.Clear();
                for (int i = 0; i < cmds.Length; i++) { m_stringBuilder.Append(cmds[i] + "   "); }
                _log.WriteLine(m_stringBuilder.ToString(), CMD_COLOR);

                _field.text = text + fGetCommon(cmds, text.Length);
                _field.caretPosition = _field.text.Length;
            }
        }

        private string fGetCommon(string[] strings, int startIndex = 0)
        {
            string bStr = strings[0];

            int characters = 0;

            for (int j = startIndex; j < bStr.Length; j++)
            {
                for (int i = 1; i < strings.Length; i++)
                {
                    if (bStr[j] != strings[i][j])
                    {
                        return bStr.Substring(startIndex, characters);
                    }
                }

                characters++;
            }

            return bStr.Substring(startIndex, characters);
        }

        private void fSwitchHistoryLine(bool back)
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

        private void fSwitch()
        {
            StopAllCoroutines();
            StartCoroutine(Switch(!m_isOn));
        }

        private void DebugLogHandler(LogType logType, object msg)
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

            if (msg == null) { _log.WriteLine("null", colour); }
            else { _log.WriteLine(msg.ToString(), colour); }
        }

        ////////////
        //Routines//
        ////////////

        IEnumerator Switch(bool on)
        {
            m_isOn = on;

            _field.gameObject.SetActive(on);

            if (on)
            {
                _field.text = string.Empty;
                _field.OnPointerClick(m_pointerEventData);
            }
            else
            {
                m_curHistoryIndex = 0;
            }

            float startPos = rectTransform.anchoredPosition.y;
            float endPos = on ? -360f : 0f;

            float ratio = 0f;

            while (ratio < 1f)
            {
                ratio += Time.unscaledDeltaTime * 10f;

                rectTransform.anchoredPosition = new Vector2(0f, Mathf.Lerp(startPos, endPos, ratio));

                yield return null;
            }
        }
    }
}
