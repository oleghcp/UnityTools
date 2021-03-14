using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityUtility;
using UnityUtility.Collections;
using UnityUtility.MathExt;

namespace UnityEditor
{
    public class DropDownList : EditorWindow
    {
        internal const string NOTHING_ITEM = "Nothing";
        internal const string EVERYTHING_ITEM = "Everything";

        private SearchField _searchField;
        private List<Data> _items = new List<Data>();

        private Vector2 _maxLabelSize;
        private Vector2 _scrollPos;
        private string _tapeString;
        private Data _selectedItem = new Data { Id = -1 };
        private bool _keyboardSelection;
        private IList<Data> _searchResult;

        private BitList _flags;
        private Action<BitList> _onClose;

        public int ItemsCount => _items.Count;

        private void Update()
        {
            Repaint();
        }

        private void OnDestroy()
        {
            if (MultiSelectable())
            {
                _onClose?.Invoke(_flags);

                if (focusedWindow != null)
                    focusedWindow.Repaint();
            }
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
                    GUI.color = Colours.Red;
                    if (GUILayout.Button("X", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(EditorGuiUtility.SmallButtonWidth)))
                        Close();
                    GUI.color = Colours.White;
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
                    minSize = maxSize = new Vector2(GetWidth(_searchResult), GetHeight(_searchResult.Count));
                }

                _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, false, false);
                DrawResults();
                EditorGUILayout.EndScrollView();
            }

            HandleEvents(Event.current);
        }

        #region List functions
        public static DropDownList Create()
        {
            return CreateInstance<DropDownList>();
        }

        internal static DropDownList Create(BitList flags, string[] displayedOptions, Action<BitList> onClose)
        {
            DropDownList popup = CreateInstance<DropDownList>();

            if (flags.Count != displayedOptions.Length)
                Debug.LogError("Flags count != displayedOptions count.");

            popup._flags = flags;
            popup._onClose = onClose;

            for (int i = 0; i < displayedOptions.Length; i++)
            {
                int index = i;
                Data item = Data.CreateItem(i, displayedOptions[i], false, () => flags.Switch(index));
                popup._items.Add(item);
            }

            popup._items.Insert(0, Data.CreateItem(popup._items.Count, NOTHING_ITEM, false, () => flags.SetAll(false)));
            popup._items.Insert(1, Data.CreateItem(popup._items.Count, EVERYTHING_ITEM, false, () => flags.SetAll(true)));
            popup._items.Insert(2, Data.CreateSeparator());

            return popup;
        }

        public void ShowMenu()
        {
            ShowMenu(new Rect(Event.current.mousePosition, Vector2.zero));
        }

        public void ShowMenu(Rect buttonRect)
        {
            if (_items.Count == 0)
                return;

            wantsMouseMove = true;
            buttonRect = GUIUtility.GUIToScreenRect(buttonRect);
            ShowAsDropDown(buttonRect, new Vector2(GetWidth(_items), GetHeight(_items.Count)));
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
                EditorGuiLayout.CenterLabel("No Results", EditorStyles.boldLabel);
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
                        EditorGUILayout.LabelField((string)null, GUI.skin.horizontalSlider, GUILayout.Width(_maxLabelSize.x));
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

                    EditorGUILayout.LabelField(item.Text, GUILayout.Width(_maxLabelSize.x));
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
                    case EVERYTHING_ITEM: return _flags.All();
                    default: return _flags[item.Id];
                }
            }

            return item.On;
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool Hovered(in Rect uiElementPos)
        {
            return uiElementPos.Contains(Event.current.mousePosition);
        }

        private float GetHeight(int itemsCount)
        {
            if (itemsCount == 0)
                itemsCount += 2;
            else
                itemsCount += 1;

            float linesHeight = EditorGUIUtility.singleLineHeight * itemsCount;
            float spacesHeight = EditorGUIUtility.standardVerticalSpacing * (itemsCount + 4);
            return (linesHeight + spacesHeight).CutAfter(Screen.currentResolution.height * 0.5f);
        }

        private float GetWidth(IList<Data> items)
        {
            Data itemWithLongestText = items.Where(item => !item.IsSeparator)
                                            .GetWithMax(item => item.Text.Length);
            _maxLabelSize = GUI.skin.label.CalcSize(EditorGuiUtility.TempContent(itemWithLongestText.Text));
            float lineWidth = EditorGuiUtility.StandardHorizontalSpacing * 5f + EditorGuiUtility.SmallButtonWidth + _maxLabelSize.x + 2f;
            return (lineWidth + lineWidth * 0.12f).Clamp(200f, Screen.currentResolution.width * 0.5f);
        }

        private bool MultiSelectable()
        {
            return _flags != null;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct Data
        {
            public string Text;
            public Action OnSelected;
            public int Id;
            public bool On;
            public bool Disabled;

            public bool IsSeparator => Text == null;

            public static Data CreateItem(int id, string content, bool on, Action onSelected)
            {
                return new Data
                {
                    Id = id,
                    Text = content ?? string.Empty,
                    On = on,
                    OnSelected = onSelected,
                };
            }

            public static Data CreateDisabledItem(int id, string content, bool on)
            {
                return new Data
                {
                    Id = id,
                    Text = content ?? string.Empty,
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
