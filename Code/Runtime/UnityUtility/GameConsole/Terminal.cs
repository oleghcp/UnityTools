using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityUtility.MathExt;
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

        private bool _isOn;
        private PointerEventData _pointerEventData;
        private StringBuilder _stringBuilder;

        private object _cmdRun;
        private Dictionary<string, MethodInfo> _commands;

        private List<string> _cmdHistory;
        private int _curHistoryIndex;

        private TerminalOptions _options;

        public bool IsOn => _isOn;
        public TerminalOptions Options => _options;

        ///////////////
        //Unity funcs//
        ///////////////

        protected override void Construct()
        {
            _log.SetUp(this);

            _pointerEventData = new PointerEventData(EventSystem.current);
            _stringBuilder = new StringBuilder();

            _cmdHistory = new List<string>() { string.Empty };

#if UNITY_EDITOR
            if (Application.isPlaying)
#endif
                Application.logMessageReceived += OnDebugLogMessageReceived;
        }

        protected override void CleanUp()
        {
#if UNITY_EDITOR
            Switched_Event = null;
            Application.logMessageReceived -= OnDebugLogMessageReceived;
#endif
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.BackQuote))
            {
                SwitchInternal();
                Switched_Event?.Invoke(_isOn);
            }

            if (_isOn)
            {
                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    FindCmd(_field.text);
                }

                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    SwitchHistoryLine(true);
                }
                else if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    SwitchHistoryLine(false);
                }

                float axis = Input.GetAxis("Mouse ScrollWheel");
                if (axis != 0) { _log.Scroll(axis); }

                if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
                {
                    EnterCmd(_field.text);
                    _field.text = string.Empty;
                    _field.OnPointerClick(_pointerEventData);
                }
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
            CreateTerminalInternal();
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
            CreateTerminalInternal();
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
            CreateTerminalInternal();
            I.SetCommands(commands);
            I.SetOptions(options);
        }

        /// <summary>
        /// Sets a new command container.
        /// </summary>
        /// <param name="commands">An object which contans command functions.</param>
        public void SetCommands(object commands)
        {
            _cmdRun = commands;
            Type t = _cmdRun.GetType();
            MethodInfo[] funcs = t.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
            _commands = funcs.ToDictionary(itm => itm.Name, itm => itm);
        }

        public void SetOptions(TerminalOptions options)
        {
            _options = options;
        }

        public void GetCmd()
        {
            FindCmd(string.Empty);
        }

        public void Switch()
        {
            SwitchInternal();
        }

        public void WriteLine(string txt, Color color)
        {
            _log.WriteLine(color, txt);
        }

        public void WriteLine(string txt, LogType logType)
        {
            _log.WriteLine(GetTextColor(logType), txt);
        }

        public void Clear()
        {
            _log.Clear();
        }

        public void OnDone(string _)
        {
            _field.OnPointerClick(_pointerEventData);
        }

        ///////////////
        //Inner funcs//
        ///////////////

        private void EnterCmd(string text)
        {
            if (NoCommands())
                return;

            if (text.HasUsefulData())
            {
                text = text.ToLower();

                string[] words = text.Split(' ');
                string command = words[0];

                if (_commands.TryGetValue(command, out MethodInfo method))
                {
                    string[][] keys = null;

                    if (method.GetParameters().Length > 0)
                        keys = new[] { words.Length > 1 ? words.GetSubArray(1) : new string[0] };

                    object cmdKeysParseError = method.Invoke(_cmdRun, keys);

                    if (cmdKeysParseError == null)
                        _log.WriteLine(CMD_COLOR, text);
                    else
                        _log.WriteLine(CMD_ERROR_COLOR, "cmd options error: " + cmdKeysParseError);
                }
                else
                {
                    _log.WriteLine(CMD_ERROR_COLOR, "unknown: " + command);
                }

                _cmdHistory.Add(text);
                _curHistoryIndex = 0;
            }
        }

        private void FindCmd(string text)
        {
            if (NoCommands())
                return;

            text = text.ToLower();

            string[] cmds = _commands.Keys.Where(itm => itm.IndexOf(text) == 0).OrderBy(itm => itm).ToArray();

            if (cmds.Length == 0)
            {
                _field.text = string.Empty;
            }
            else if (cmds.Length == 1)
            {
                _field.text = _options.AddSpaceAfterName ? cmds[0] + " " : cmds[0];
                _field.caretPosition = _field.text.Length;
            }
            else
            {
                for (int i = 0; i < cmds.Length; i++)
                {
                    _stringBuilder.Append(cmds[i]).Append("   ");
                }

                string line = _stringBuilder.ToString();
                _stringBuilder.Clear();

                _log.WriteLine(CMD_COLOR, line);
                _field.text = text + getCommon(cmds, text.Length);
                _field.caretPosition = _field.text.Length;
            }

            string getCommon(string[] strings, int startIndex = 0)
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
        }

        private void SwitchHistoryLine(bool back)
        {
            if (_cmdHistory.Count == 0) { return; }

            if (back)
                _curHistoryIndex--;
            else
                _curHistoryIndex++;

            _curHistoryIndex = _curHistoryIndex.Repeat(_cmdHistory.Count);

            _field.text = _cmdHistory[_curHistoryIndex];
            _field.caretPosition = _field.text.Length;
        }

        private void SwitchInternal()
        {
            StopAllCoroutines();
            StartCoroutine(Switch(!_isOn));
        }

        private Color GetTextColor(LogType logType)
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

        private void OnDebugLogMessageReceived(string msg, string stackTrace, LogType logType)
        {
            if (_options.ShowDebugLogs)
            {
                _log.WriteLine(GetTextColor(logType), msg, stackTrace);
            }
        }

        private static void CreateTerminalInternal()
        {
            Resources.Load<GameObject>("Terminal")
                     .Install()
                     .Immortalize();

            if (EventSystem.current == null)
            {
                GameObject eventSystemRoot = ComponentUtility.CreateInstance<EventSystem>().gameObject;
                eventSystemRoot.AddComponent<StandaloneInputModule>();
                eventSystemRoot.Immortalize();
            }
        }

        private bool NoCommands()
        {
            if (_cmdRun == null)
            {
                _log.WriteLine(GetTextColor(LogType.Error), "Commands are not set.");
                return true;
            }

            return false;
        }

        ////////////
        //Routines//
        ////////////

        private IEnumerator Switch(bool on)
        {
            float targetHeight = 720f * _options.TargetHeight;

            _field.gameObject.SetActive(_isOn = on);

            if (on)
                _field.text = string.Empty;
            else
                _curHistoryIndex = 0;

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
                _field.OnPointerClick(_pointerEventData);
        }
    }
}
