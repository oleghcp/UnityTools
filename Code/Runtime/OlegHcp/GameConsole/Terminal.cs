#if INCLUDE_UNITY_UI
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using OlegHcp.CSharp;
using OlegHcp.CSharp.Collections;
using OlegHcp.CSharp.Text;
using OlegHcp.Engine;
using OlegHcp.Mathematics;
using OlegHcp.SingleScripts;
using OlegHcp.Tools;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.RectTransform;

namespace OlegHcp.GameConsole
{
    [DisallowMultipleComponent]
    public sealed class Terminal : SingleUiBehaviour<Terminal>
    {
        private const float TARGET_CANVAS_SIDE = 720f;

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

        private readonly StringBuilder _stringBuilder = new StringBuilder();
        private readonly Dictionary<string, MethodInfo> _commands = new Dictionary<string, MethodInfo>();
        private readonly List<string> _cmdHistory = new List<string>() { string.Empty };
        private PointerEventData _pointerEventData;
        private TerminalOptions _options;
        private ITerminalSwitchTrigger _switchTrigger;
        private TerminalCommands _cmdContainer;
        private int _curHistoryIndex;
        private float _ratio;
        private bool _initialized;
        private bool _isOn;

        public bool IsOn => _isOn;
        public TerminalOptions Options => _options;

        protected override void Construct()
        {
            transform.parent.Immortalize();
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

            WriteLine("Up/Down:  types previous commands\nTab:      completes typing of a command or/and prints available commands\n", Colours.Lime);
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
        /// <param name="commands">An object which contains command functions.</param>
        public static void CreateTerminal(bool createEventSystem = false)
        {
            if (TryCreateTerminal(createEventSystem))
                I.Init(new TerminalCommands(), new TerminalOptions(), new DefaultTrigger());
        }

        /// <summary>
        /// Creates a command line (aka console/terminal)
        /// Commands are just functions of the view:
        /// public string commandname(string[] options).
        /// It should return error description (if options are parsed with error) or null (if options parsed well or there are no options at all). 
        /// </summary>
        /// <param name="commands">An object which contains command functions.</param>
        public static void CreateTerminal(TerminalCommands commands, bool createEventSystem = false)
        {
            if (TryCreateTerminal(createEventSystem))
                I.Init(commands, new TerminalOptions(), new DefaultTrigger());
        }

        /// <summary>
        /// Creates a command line (aka console/terminal)
        /// Commands are just functions of the view:
        /// public string commandname(string[] options).
        /// It should return error description (if options are parsed with error) or null (if options parsed well or there are no options at all). 
        /// </summary>
        /// <param name="commands">An object which contains command functions.</param>
        public static void CreateTerminal(TerminalCommands commands, TerminalOptions options, bool createEventSystem = false)
        {
            if (TryCreateTerminal(createEventSystem))
                I.Init(commands, options, new DefaultTrigger());
        }

        /// <summary>
        /// Creates a command line (aka console/terminal)
        /// Commands are just functions of the view:
        /// public string commandname(string[] options).
        /// It should return error description (if options are parsed with error) or null (if options parsed well or there are no options at all). 
        /// </summary>
        /// <param name="commands">An object which contains command functions.</param>
        public static void CreateTerminal(TerminalCommands commands, ITerminalSwitchTrigger switchTrigger, bool createEventSystem = false)
        {
            if (TryCreateTerminal(createEventSystem))
                I.Init(commands, new TerminalOptions(), switchTrigger);
        }

        /// <summary>
        /// Creates a command line (aka console/terminal)
        /// Commands are just functions of the view:
        /// public string commandname(string[] options).
        /// It should return error description (if options are parsed with error) or null (if options parsed well or there are no options at all). 
        /// </summary>
        /// <param name="commands">An object which contains command functions.</param>
        public static void CreateTerminal(TerminalCommands commands, TerminalOptions options, ITerminalSwitchTrigger switchTrigger, bool createEventSystem = false)
        {
            if (TryCreateTerminal(createEventSystem))
                I.Init(commands, options, switchTrigger);
        }

        /// <summary>
        /// Sets a new command container.
        /// </summary>
        /// <param name="commands">An object which contains command functions.</param>
        public void SetCommands(TerminalCommands commands)
        {
            if (commands == null)
                throw ThrowErrors.NullParameter(nameof(commands));

            _cmdContainer = commands;
            _commands.Clear();
            commands.GetType()
                    .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                    .ForEach(handleMethod);

            void handleMethod(MethodInfo method)
            {
                var attribute = method.GetCustomAttribute<TerminalCommandAttribute>();
                if (attribute != null)
                {
                    string name = attribute.CommandName.HasUsefulData() ? attribute.CommandName
                                                                        : method.Name;
                    _commands.Add(name, method);
                }
            }
        }

        public void SetSwitchTrigger(ITerminalSwitchTrigger switchTrigger)
        {
            if (switchTrigger == null)
                throw ThrowErrors.NullParameter(nameof(switchTrigger));

            _switchTrigger = switchTrigger;
        }

        public void SetOptions(TerminalOptions options)
        {
            if (options == null)
                throw ThrowErrors.NullParameter(nameof(options));

            _options = options;
        }

        public void WriteCommandList()
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
            _log.WriteLine(GetCommandColor(true), txt);
        }

        public void WriteLine(string txt, in Color color)
        {
            _log.WriteLine(color, txt);
        }

        public void WriteLine(string txt, LogType logType = LogType.Log)
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
            if (text.IsNullOrWhiteSpace())
                return;

            text = text.ToLower();

            string[] options = split(text, out string command);

            if (_commands.TryGetValue(command, out MethodInfo method))
            {
                string[][] parameters = null;

                if (method.GetParameters().Length > 0)
                    parameters = new[] { options };

                try
                {
                    object success = method.Invoke(_cmdContainer, parameters);

                    if (success == null || (bool)success)
                        _log.WriteLine(GetCommandColor(), text);
                }
                catch (Exception)
                {
                    WriteCmdError($"command {command} performed with errors");
                    throw;
                }
            }
            else
            {
                WriteCmdError($"unknown command: {command}");
            }

            if (_cmdHistory.Count == 0 || _cmdHistory.FromEnd(0) != text)
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
            text = text.ToLower();
            string[] commands = _commands.Keys
                                         .Where(itm => itm.IndexOf(text) == 0)
                                         .ToArray();
            switch (commands.Length)
            {
                case 0:
                    _field.text = string.Empty;
                    break;

                case 1:
                    _field.text = _options.AddSpaceAfterName ? $"{commands[0]} " : commands[0];
                    break;

                default:
                {
                    commands.Sort();

                    _stringBuilder.AppendLine();
                    for (int i = 0; i < commands.Length; i++)
                    {
                        _stringBuilder.Append(' ')
                                      .AppendLine(commands[i]);
                    }

                    WriteLine(_stringBuilder.Cut(), GetCommandColor());
                    _field.text = $"{text}{getCommon(commands, text.Length)}";

                    break;
                }
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
                case LogType.Log: return Colours.White;
                case LogType.Warning: return Colours.Yellow;
                case LogType.Error: return Colours.Orange;
                case LogType.Exception: return Colours.Red;
                case LogType.Assert: return Colours.Magenta;
                default: throw new SwitchExpressionException(logType);
            }
        }

        private Color GetCommandColor(bool error = false)
        {
            return error ? Colours.Teal : Colours.Cyan;
        }

        private void OnDebugLogMessageReceived(string msg, string stackTrace, LogType logType)
        {
            if (_options.ShowDebugLogs)
                _log.WriteLine(GetTextColor(logType), msg, stackTrace);
        }

        private static bool TryCreateTerminal(bool createEventSystem)
        {
            if (Exists)
            {
                Debug.LogError($"Terminal is already created.");
                return false;
            }

            GameObject terminalAsset = Resources.Load<GameObject>("Terminal");

            if (terminalAsset == null)
            {
                Debug.LogError($"No Terminal.prefab found. Create terminal prefab using {nameof(OlegHcp)} menu item.");
                return false;
            }

            if (createEventSystem && EventSystem.current == null)
                InstantiateEventSystem();

            terminalAsset.Install();
            return true;
        }

        private static void InstantiateEventSystem()
        {
            EventSystem eventSystem = ComponentUtility.CreateInstance<EventSystem>();
            eventSystem.gameObject.AddComponent<StandaloneInputModule>();
            eventSystem.Immortalize();
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
                ratio += Time.unscaledDeltaTime * _options.MoveSpeed;
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
