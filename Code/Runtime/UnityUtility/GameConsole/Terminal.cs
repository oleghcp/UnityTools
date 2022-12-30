#if INCLUDE_UNITY_UI
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityUtility.CSharp;
using UnityUtility.CSharp.Collections;
using UnityUtility.CSharp.Text;
using UnityUtility.Engine;
using UnityUtility.Mathematics;
using UnityUtility.SingleScripts;
using UnityUtility.Tools;
using static UnityEngine.RectTransform;

namespace UnityUtility.GameConsole
{
    [DisallowMultipleComponent]
    public sealed class Terminal : SingleUiBehaviour<Terminal>
    {
        private const float TARGET_CANVAS_SIDE = 720f;

        private readonly Color _cmdColor = Colours.Cyan;
        private readonly Color _cmdErrorColor = Colours.Teal;

        public static event Action<bool> Switched_Event;

        [SerializeField]
        private CanvasScaler _canvasScaler;
        [SerializeField]
        private InputField _field;
        [SerializeField]
        private LogController _log;
        [SerializeField]
        private GameObject _closeButton;
        [SerializeField]
        private GameObject _border;

        private TerminalOptions _options;
        private ITerminalSwitchTrigger _switchTrigger;
        private PointerEventData _pointerEventData;
        private StringBuilder _stringBuilder = new StringBuilder();
        private Dictionary<string, MethodInfo> _commands = new Dictionary<string, MethodInfo>();
        private List<string> _cmdHistory = new List<string>();
        private bool _isOn;
        private TerminalCommands _cmdContainer;
        private int _curHistoryIndex;
        private float _ratio;
        private bool _initialized;

        public bool IsOn => _isOn;
        public TerminalOptions Options => _options;

        protected override void Construct()
        {
            transform.parent.Immortalize();
            _cmdHistory.Add(string.Empty);
            _log.SetUp(this);
        }

        private void Start()
        {
            if (!_initialized)
            {
                Init(new TerminalCommands(), new TerminalOptions(), new DefaultTrigger());

                if (EventSystem.current == null)
                    InstantiateEventSystem();
            }

            _pointerEventData = new PointerEventData(EventSystem.current);
        }

        protected override void Destruct()
        {
            Switched_Event = null;
            Application.logMessageReceived -= OnDebugLogMessageReceived;
        }

        private void Update()
        {
            if (_ratio != (float)Screen.height / Screen.width)
                OnAspectRatioChange();

            if (_switchTrigger.SwitchThisFrame)
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
            }
        }

        /// <summary>
        /// Creates a command line (aka console/terminal)
        /// Commands are just functions of the view:
        /// public string commandname(string[] options).
        /// It should return error description (if options are parsed with error) or null (if options parsed well or there are no options at all). 
        /// </summary>
        /// <param name="commands">An object which contans command functions.</param>
        public static void CreateTerminal(bool createEventSystem = false)
        {
            if (CreateTerminalInternal(createEventSystem))
                I.Init(new TerminalCommands(), new TerminalOptions(), new DefaultTrigger());
        }

        /// <summary>
        /// Creates a command line (aka console/terminal)
        /// Commands are just functions of the view:
        /// public string commandname(string[] options).
        /// It should return error description (if options are parsed with error) or null (if options parsed well or there are no options at all). 
        /// </summary>
        /// <param name="commands">An object which contans command functions.</param>
        public static void CreateTerminal(TerminalCommands commands, bool createEventSystem = false)
        {
            if (CreateTerminalInternal(createEventSystem))
                I.Init(commands, new TerminalOptions(), new DefaultTrigger());
        }

        /// <summary>
        /// Creates a command line (aka console/terminal)
        /// Commands are just functions of the view:
        /// public string commandname(string[] options).
        /// It should return error description (if options are parsed with error) or null (if options parsed well or there are no options at all). 
        /// </summary>
        /// <param name="commands">An object which contans command functions.</param>
        public static void CreateTerminal(TerminalCommands commands, TerminalOptions options, bool createEventSystem = false)
        {
            if (CreateTerminalInternal(createEventSystem))
                I.Init(commands, options, new DefaultTrigger());
        }

        /// <summary>
        /// Creates a command line (aka console/terminal)
        /// Commands are just functions of the view:
        /// public string commandname(string[] options).
        /// It should return error description (if options are parsed with error) or null (if options parsed well or there are no options at all). 
        /// </summary>
        /// <param name="commands">An object which contans command functions.</param>
        public static void CreateTerminal(TerminalCommands commands, ITerminalSwitchTrigger switchTrigger, bool createEventSystem = false)
        {
            if (CreateTerminalInternal(createEventSystem))
                I.Init(commands, new TerminalOptions(), switchTrigger);
        }

        /// <summary>
        /// Creates a command line (aka console/terminal)
        /// Commands are just functions of the view:
        /// public string commandname(string[] options).
        /// It should return error description (if options are parsed with error) or null (if options parsed well or there are no options at all). 
        /// </summary>
        /// <param name="commands">An object which contans command functions.</param>
        public static void CreateTerminal(TerminalCommands commands, TerminalOptions options, ITerminalSwitchTrigger switchTrigger, bool createEventSystem = false)
        {
            if (CreateTerminalInternal(createEventSystem))
                I.Init(commands, options, switchTrigger);
        }

        /// <summary>
        /// Sets a new command container.
        /// </summary>
        /// <param name="commands">An object which contans command functions.</param>
        public void SetCommands(TerminalCommands commands)
        {
            if (commands == null)
                throw Errors.NullParameter(nameof(commands));

            _cmdContainer = commands;
            _commands = _cmdContainer.GetType()
                                     .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                                     .Where(method => method.GetCustomAttribute(typeof(TerminalCommandAttribute)) != null)
                                     .ToDictionary(item => item.Name);
        }

        public void SetSwitchTrigger(ITerminalSwitchTrigger switchTrigger)
        {
            if (switchTrigger == null)
                throw Errors.NullParameter(nameof(switchTrigger));

            _switchTrigger = switchTrigger;
        }

        public void SetOptions(TerminalOptions options)
        {
            if (options == null)
                throw Errors.NullParameter(nameof(options));

            _options = options;
        }

        public void WriteCommanList()
        {
            FindCmd(string.Empty);
        }

        /// <summary>
        /// Show or hide terminal.
        /// </summary>
        public void Switch()
        {
            SwitchInternal();
        }

        public void WriteCmdError(string txt)
        {
            _log.WriteLine(_cmdErrorColor, txt);
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

        /// <summary>
        /// This method is called by input field. Don't use it.
        /// </summary>
        public void OnDone()
        {
            if (!Input.GetKey(KeyCode.BackQuote))
                EnterCmd(_field.text);

            _field.text = string.Empty;
            _field.OnPointerClick(_pointerEventData);
        }

        /// <summary>
        /// This method is called by input field. Don't use it.
        /// </summary>
        public void OnScroll()
        {
            _log.OnScroll();
        }

        private void Init(TerminalCommands commands, TerminalOptions options, ITerminalSwitchTrigger switchTrigger)
        {
            SetCommands(commands);
            SetOptions(options);
            SetSwitchTrigger(switchTrigger);
            Application.logMessageReceived += OnDebugLogMessageReceived;
            _initialized = true;
        }

        private void EnterCmd(string text)
        {
            if (NoCommands() || text.IsNullOrWhiteSpace())
                return;

            text = text.ToLower();

            string[] options = split(text, out string command);

            if (_commands.TryGetValue(command, out MethodInfo method))
            {
                string[][] parameters = null;

                if (method.GetParameters().Length > 0)
                    parameters = new[] { options };

                object success = method.Invoke(_cmdContainer, parameters);

                if (success == null || (bool)success)
                    _log.WriteLine(_cmdColor, text);
            }
            else
            {
                _log.WriteLine(_cmdErrorColor, $"unknown command: {command}");
            }

            _cmdHistory.Add(text);
            _curHistoryIndex = 0;

            string[] split(string line, out string commandWord)
            {
                string[] words = line.Trim(' ')
                                     .Split(' ');
                commandWord = words[0];
                return words.Length == 1 ? Array.Empty<string>()
                                         : words.GetSubArray(1);
            }
        }

        private void FindCmd(string text)
        {
            if (NoCommands())
                return;

            text = text.ToLower();
            string[] cmds = _commands.Keys.Where(itm => itm.IndexOf(text) == 0).ToArray();

            if (cmds.Length == 0)
            {
                _field.text = string.Empty;
            }
            else if (cmds.Length == 1)
            {
                _field.text = _options.AddSpaceAfterName ? $"{cmds[0]} " : cmds[0];
            }
            else
            {
                cmds.Sort();

                for (int i = 0; i < cmds.Length; i++)
                {
                    _stringBuilder.Append(cmds[i])
                                  .Append("   ");
                }

                _log.WriteLine(_cmdColor, _stringBuilder.Cut());
                _field.text = $"{text}{getCommon(cmds, text.Length)}";
            }

            _field.caretPosition = _field.text.Length;

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
            StartCoroutine(GetSwitchRoutine());
        }

        private Color GetTextColor(LogType logType)
        {
            switch (logType)
            {
                case LogType.Error:
                case LogType.Assert:
                    return Colours.Orange;

                case LogType.Exception:
                    return Colours.Red;

                case LogType.Warning:
                    return Colours.Yellow;

                case LogType.Log:
                    return Colours.White;

                default:
                    throw new UnsupportedValueException(logType);
            }
        }

        private void OnDebugLogMessageReceived(string msg, string stackTrace, LogType logType)
        {
            if (_options.ShowDebugLogs)
                _log.WriteLine(GetTextColor(logType), msg, stackTrace);
        }

        private static bool CreateTerminalInternal(bool createEventSystem)
        {
            if (Exists)
            {
                Debug.LogError($"Terminal is already created.");
                return false;
            }

            GameObject terminal = Resources.Load<GameObject>("Terminal");

            if (terminal == null)
            {
                Debug.LogError($"No Terminal.prefab found. Create terminal prefab using {nameof(UnityUtility)} menu item.");
                return false;
            }

            if (createEventSystem && EventSystem.current == null)
                InstantiateEventSystem();

            return terminal.Install();
        }

        private static void InstantiateEventSystem()
        {
            EventSystem eventSystem = ComponentUtility.CreateInstance<EventSystem>();
            eventSystem.gameObject.AddComponent<StandaloneInputModule>();
            eventSystem.Immortalize();
        }

        private bool NoCommands()
        {
            if (_cmdContainer == null)
            {
                _log.WriteLine(GetTextColor(LogType.Error), "Commands are not set.");
                return true;
            }

            return false;
        }

        private void OnAspectRatioChange()
        {
            _ratio = (float)Screen.height / Screen.width;
            _canvasScaler.referenceResolution = _ratio < 1f ? new Vector2(TARGET_CANVAS_SIDE / _ratio, TARGET_CANVAS_SIDE)
                                                            : new Vector2(TARGET_CANVAS_SIDE, TARGET_CANVAS_SIDE * _ratio);
        }

        private IEnumerator GetSwitchRoutine()
        {
            _isOn = !_isOn;
            float consoleHeight = _canvasScaler.referenceResolution.y * _options.TargetHeight;
            rectTransform.SetSizeWithCurrentAnchors(Axis.Vertical, consoleHeight);

            _field.gameObject.SetActive(_isOn);
            _closeButton.SetActive(_isOn);

            if (_isOn)
            {
                _field.text = string.Empty;
#if UNITY_EDITOR || UNITY_STANDALONE
                _field.OnPointerClick(_pointerEventData);
#endif
                _border.SetActive(true);
            }
            else
            {
                _curHistoryIndex = 0;
            }

            float hStart = _isOn ? 0f : consoleHeight;
            float hEnd = _isOn ? consoleHeight : 0f;

            float ratio = 0f;

            while (ratio < 1f)
            {
                ratio += Time.unscaledDeltaTime * 10f;
                rectTransform.anchoredPosition = new Vector2(0f, -Mathf.Lerp(hStart, hEnd, ratio));

                yield return null;
            }

            if (!_isOn)
                _border.SetActive(false);
        }

        private class DefaultTrigger : ITerminalSwitchTrigger
        {
            public bool SwitchThisFrame => Input.GetKeyDown(KeyCode.BackQuote);
        }
    }
}
#endif
