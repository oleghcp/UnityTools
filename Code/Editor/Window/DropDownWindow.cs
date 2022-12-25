using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityUtility.Collections;
using UnityUtility.CSharp;
using UnityUtility.CSharp.Collections;
using UnityUtility.Engine;
using UnityUtility.Mathematics;
using UnityUtilityEditor.Engine;

namespace UnityUtilityEditor.Window
{
    internal class DropDownWindow : EditorWindow
    {
        public const string NOTHING_ITEM = "Nothing";
        public const string EVERYTHING_ITEM = "Everything";
        private const float MIN_WINDOW_WIDTH = 150f;

        private SearchField _searchField;
        private List<Data> _items = new List<Data>();

        private float _maxLineLength;
        private Vector2 _scrollPos;
        private string _tapeString;
        private Data _selectedItem = new Data { Id = -1 };
        private bool _keyboardSelection;
        private IList<Data> _searchResult;

        private BitList _flags;
        private Action<BitList> _onClose;
        private EditorWindow _prevWindow;

        public int ItemsCount => _items.Count;

        private void OnEnable()
        {
            hideFlags = HideFlags.DontSave;
            wantsMouseMove = true;
        }

        private void Update()
        {
            Repaint();
        }

        private void OnDestroy()
        {
            if (MultiSelectable())
                _onClose?.Invoke(_flags);

            if (_prevWindow != null)
                _prevWindow.Repaint();
        }

        private void OnGUI()
        {
            if (_searchField == null)
            {
                _searchField = new SearchField();
                _searchField.SetFocus();
            }

            if (_searchResult == null)
                _searchResult = _items;

            using (new GUILayout.AreaScope(new Rect(Vector2.zero, position.size), (string)null, EditorStyles.helpBox))
            {
                string tapeString;

                if (MultiSelectable())
                {
                    EditorGUILayout.BeginHorizontal();
                    tapeString = _searchField.OnGUI(_tapeString);
                    if (GUILayout.Button("X", GUILayout.Height(16f), GUILayout.Width(EditorGuiUtility.SmallButtonWidth)))
                        Close();
                    EditorGUILayout.EndHorizontal();
                }
                else
                {
                    tapeString = _searchField.OnGUI(_tapeString);
                }

                if (_tapeString != tapeString)
                {
                    _tapeString = tapeString;
                    Search();
                    Vector2 size = GetWinSize(_searchResult);
                    minSize = size;
                    maxSize = size;
                }

                _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, false, false);
                DrawResults();
                EditorGUILayout.EndScrollView();
            }

            HandleEvents(Event.current);
        }

        #region List functions
        public static void CreateForFlags(BitList flags, string[] displayedOptions, Action<BitList> onClose)
        {
            CreateForFlags(GetButtonRect(Event.current.mousePosition), flags, displayedOptions, onClose);
        }

        public static void CreateForFlags(in Rect buttonRect, BitList flags, string[] displayedOptions, Action<BitList> onClose)
        {
            DropDownWindow popup = CreateInstance<DropDownWindow>();
            popup.CreateForFlagsInternal(buttonRect, flags, displayedOptions, onClose);
        }

        private void CreateForFlagsInternal(in Rect buttonRect, BitList flags, string[] displayedOptions, Action<BitList> onClose)
        {
            if (flags.Count != displayedOptions.Length)
            {
                Debug.LogError($"Flags count ({flags.Count}) != displayed options count ({displayedOptions.Length}).");
                return;
            }

            _flags = flags;
            _onClose = onClose;

            for (int i = 0; i < displayedOptions.Length; i++)
            {
                if (displayedOptions[i].IsNullOrEmpty())
                    continue;

                int index = i;
                Data item = Data.CreateItem(i, displayedOptions[i], false, () => _flags.Switch(index));
                _items.Add(item);
            }

            _items.Insert(0, Data.CreateItem(_flags.Count, NOTHING_ITEM, flags.IsEmpty(), () => _flags.SetAll(false)));
            _items.Insert(1, Data.CreateItem(_flags.Count, EVERYTHING_ITEM, allFor(), setAllTrue));
            _items.Insert(2, Data.CreateSeparator());

            ShowMenu(buttonRect);

            bool allFor()
            {
                for (int i = 0; i < displayedOptions.Length; i++)
                {
                    if (displayedOptions[i].HasAnyData() && !flags[i])
                        return false;
                }

                return true;
            }

            void setAllTrue()
            {
                for (int i = 0; i < _items.Count; i++)
                {
                    if ((uint)_items[i].Id >= (uint)_flags.Count)
                        continue;

                    if (_items[i].Text.HasAnyData())
                        _flags[_items[i].Id] = true;
                }
            }
        }

        public void ShowMenu()
        {
            ShowMenu(GetButtonRect(Event.current.mousePosition));
        }

        public void ShowMenu(Vector2 position)
        {
            ShowMenu(GetButtonRect(position));
        }

        public void ShowMenu(in Rect buttonRect)
        {
            _prevWindow = mouseOverWindow;

            if (_items.Count == 0)
                return;

            Vector2 size = GetWinSize(_items);
            maxSize = size;
            Rect rect = GUIUtility.GUIToScreenRect(buttonRect);

            ShowAsDropDown(rect, size);
        }

        public void AddItem(string content, Action onSelected)
        {
            _items.Add(Data.CreateItem(_items.Count, content, false, onSelected));
        }

        public void AddItem(string content, bool on, Action onSelected)
        {
            _items.Add(Data.CreateItem(_items.Count, content, on, onSelected));
        }

        public void AddDisabledItem(string content)
        {
            _items.Add(Data.CreateDisabledItem(_items.Count, content, false));
        }

        public void AddDisabledItem(string content, bool on)
        {
            _items.Add(Data.CreateDisabledItem(_items.Count, content, on));
        }

        public void AddSeparator()
        {
            _items.Add(Data.CreateSeparator());
        }
        #endregion

        private void HandleEvents(Event e)
        {
            switch (e.type)
            {
                case EventType.KeyDown:
                    if (e.keyCode == KeyCode.Escape)
                        Close();

                    if (e.keyCode == KeyCode.Return)
                        TryInvokeItem();

                    if (e.keyCode == KeyCode.DownArrow)
                        TryMoveSelector(1);

                    if (e.keyCode == KeyCode.UpArrow)
                        TryMoveSelector(-1);
                    break;

                case EventType.MouseDown:
                    TryInvokeItem();
                    break;

                case EventType.MouseMove:
                    _keyboardSelection = false;
                    break;
            }

            if (!_keyboardSelection)
                _selectedItem.Id = -1;
        }

        private void Search()
        {
            if (_tapeString.IsNullOrWhiteSpace())
            {
                _searchResult = _items;
                return;
            }

            _searchResult = _items.Where(item => !item.IsSeparator)
                                  .Where(item => item.Text.IndexOf(_tapeString, StringComparison.CurrentCultureIgnoreCase) >= 0)
                                  .ToArray();
        }

        private void DrawResults()
        {
            if (_searchResult.Count == 0)
            {
                GUI.enabled = false;
                EditorGuiLayout.BeginHorizontalCentering();
                GUILayout.Label("No Results", EditorStyles.boldLabel);
                EditorGuiLayout.EndHorizontalCentering();
                GUI.enabled = true;
                return;
            }

            foreach (Data item in _searchResult)
            {
                if (item.IsSeparator)
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        GUILayout.Space(EditorGuiUtility.SmallButtonWidth + EditorGuiUtility.StandardHorizontalSpacing * 2f);
                        EditorGUILayout.LabelField((string)null, GUI.skin.horizontalSlider, GUILayout.Width(_maxLineLength));
                    }
                    continue;
                }

                GUI.enabled = !item.Disabled;
                using (new EditorGUILayout.HorizontalScope())
                {
                    if (IsItemOn(item))
                        EditorGUILayout.LabelField("√", GUILayout.Width(EditorGuiUtility.SmallButtonWidth));
                    else
                        EditorGUILayout.LabelField(string.Empty, GUILayout.Width(EditorGuiUtility.SmallButtonWidth));

                    EditorGUILayout.LabelField(item.Text, GUILayout.Width(_maxLineLength));
                }
                GUI.enabled = true;

                if (item.Disabled)
                    continue;

                Rect linePos = GUILayoutUtility.GetLastRect()
                                               .GetExpanded(new Vector2(2f, 2f));

                if (_keyboardSelection)
                {
                    if (item.Id == _selectedItem.Id)
                        GUI.Box(linePos, string.Empty, EditorStyles.helpBox);
                }
                else if (Hovered(linePos))
                {
                    _selectedItem = item;
                    GUI.Box(linePos, string.Empty, EditorStyles.helpBox);
                }
            }
        }

        private bool IsItemOn(in Data item)
        {
            if (MultiSelectable())
            {
                switch (item.Text)
                {
                    case NOTHING_ITEM: return _flags.IsEmpty();
                    case EVERYTHING_ITEM: return all();
                    default: return _flags[item.Id];
                }
            }

            return item.On;

            bool all()
            {
                for (int i = 0; i < _items.Count; i++)
                {
                    if ((uint)_items[i].Id >= (uint)_flags.Count)
                        continue;

                    if (_items[i].Text.HasAnyData() && !_flags[_items[i].Id])
                        return false;
                }

                return true;
            }
        }

        private void TryInvokeItem()
        {
            if (_selectedItem.Id >= 0)
            {
                _selectedItem.OnSelected();

                if (!MultiSelectable())
                    Close();
            }
        }

        private void TryMoveSelector(int direction)
        {
            if (_searchResult.Count > 0)
            {
                _keyboardSelection = true;
                int index = _searchResult.IndexOf(item => item.Id == _selectedItem.Id);

                if (index < 0 && direction < 0)
                    index = _searchResult.Count;

                index = (index + direction).Repeat(_searchResult.Count);
                _selectedItem = _searchResult[index];

                if (index == _searchResult.Count - 1 && direction < 0)
                    _scrollPos.y = float.PositiveInfinity;
                else if (index == 0 && direction > 0)
                    _scrollPos.y = 0f;
                else
                    _scrollPos.y += EditorGUIUtility.singleLineHeight * direction;
                GUI.changed = true;
            }
        }

        private bool Hovered(in Rect uiElementPos)
        {
            return uiElementPos.Contains(Event.current.mousePosition);
        }

        private Vector2 GetWinSize(IList<Data> items)
        {
            return new Vector2(getWidth(), getHeight());

            float getHeight()
            {
                int linesCount = getLinesCount();
                float linesHeight = EditorGUIUtility.singleLineHeight * linesCount;
                float spacesHeight = EditorGUIUtility.standardVerticalSpacing * (linesCount + 4);
                return (linesHeight + spacesHeight).ClampMax(Screen.currentResolution.height * 0.5f);

                int getLinesCount()
                {
                    if (items.Count == 0)
                        return 2;

                    return items.Count + 1;
                }
            }

            float getWidth()
            {
                if (items.Count == 0)
                    return MIN_WINDOW_WIDTH;

                Data longestLine = items.Where(item => !item.IsSeparator)
                                        .GetWithMax(item => item.Text.Length);
                try
                {
                    Vector2 size = GUI.skin.label.CalcSize(EditorGuiUtility.TempContent(longestLine.Text));
                    _maxLineLength = size.x;
                }
                catch (ArgumentException)
                {
                    _maxLineLength = longestLine.Text.Length * 10f;
                }

                float lineWidth = EditorGuiUtility.StandardHorizontalSpacing * 5f + EditorGuiUtility.SmallButtonWidth + _maxLineLength + 2f;
                return (lineWidth + lineWidth * 0.12f).Clamp(MIN_WINDOW_WIDTH, Screen.currentResolution.width * 0.5f);
            }
        }

        private bool MultiSelectable()
        {
            return _flags != null;
        }

        private static Rect GetButtonRect(Vector2 position)
        {
            return new Rect(position, Vector2.zero);
        }


        private struct Data
        {
            public Action OnSelected;
            public string Text;
            public int Id;
            public bool On;
            public bool Disabled;

            public bool IsSeparator => Text == null;

            public static Data CreateItem(int id, string content, bool on, Action onSelected)
            {
                if (content == null)
                    return CreateSeparator();

                return new Data
                {
                    Id = id,
                    Text = content,
                    On = on,
                    OnSelected = onSelected,
                };
            }

            public static Data CreateDisabledItem(int id, string content, bool on)
            {
                if (content == null)
                    return CreateSeparator();

                return new Data
                {
                    Id = id,
                    Text = content,
                    Disabled = true,
                    On = on,
                };
            }

            public static Data CreateSeparator()
            {
                return new Data { Id = -1, };
            }
        }
    }
}
