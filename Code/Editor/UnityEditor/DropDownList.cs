using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityUtility.MathExt;

namespace UnityEditor
{
    public class DropDownList : EditorWindow
    {
        private SearchField _searchField;
        private List<Data> _items = new List<Data>();

        private Vector2 _maxLabelSize;
        private Vector2 _scrollPos;
        private string _tapeString;
        private int _selectedId = -1;
        private bool _keyboardSelection;
        private IList<Data> _searchResult;

        public int ItemsCount => _items.Count;

        private void Update()
        {
            Repaint();
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
                string tapeString = _searchField.OnGUI(_tapeString);

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
            Data data = new Data
            {
                Id = _items.Count,
                Text = content ?? string.Empty,
                OnSelected = onSelected,
            };

            _items.Add(data);
        }

        public void AddItem(string content, bool on, Action onSelected)
        {
            Data data = new Data
            {
                Id = _items.Count,
                Text = content ?? string.Empty,
                On = on,
                OnSelected = onSelected,
            };

            _items.Add(data);
        }

        public void AddDisabledItem(string content)
        {
            Data data = new Data
            {
                Id = _items.Count,
                Text = content ?? string.Empty,
                Disabled = true,
            };

            _items.Add(data);
        }

        public void AddDisabledItem(string content, bool on)
        {
            Data data = new Data
            {
                Id = _items.Count,
                Text = content ?? string.Empty,
                Disabled = true,
                On = on,
            };

            _items.Add(data);
        }

        public void AddSeparator()
        {
            _items.Add(new Data { Id = -1, });
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
                _selectedId = -1;
        }

        private void Search()
        {
            if (_tapeString.IsNullOrWhiteSpace())
            {
                _searchResult = _items;
                return;
            }

            _searchResult = _items.Where(item => !item.IsSeparator)
                                  .Where(item => item.Text.IndexOf(_tapeString, StringComparison.CurrentCultureIgnoreCase) == 0)
                                  .ToArray();
        }

        private void DrawResults()
        {
            if (_searchResult.Count == 0)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();
                    GUI.enabled = false;
                    GUILayout.Label("No Results");
                    GUI.enabled = true;
                    GUILayout.FlexibleSpace();
                }
                return;
            }

            foreach (Data item in _searchResult)
            {
                if (item.IsSeparator)
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        GUILayout.Space(EditorGUIUtilityExt.SmallButtonWidth + EditorGUIUtilityExt.StandardHorizontalSpacing * 2f);
                        EditorGUILayout.LabelField((string)null,
                                                   GUI.skin.horizontalSlider,
                                                   GUILayout.Width(_maxLabelSize.x),
                                                   GUILayout.Height(EditorGUIUtility.singleLineHeight - EditorGUIUtility.standardVerticalSpacing));
                    }
                    continue;
                }

                GUI.enabled = !item.Disabled;
                using (new EditorGUILayout.HorizontalScope())
                {
                    if (item.On)
                        GUILayout.Label("√", GUILayout.Width(EditorGUIUtilityExt.SmallButtonWidth));
                    else
                        GUILayout.Label(string.Empty, GUILayout.Width(EditorGUIUtilityExt.SmallButtonWidth));

                    GUILayout.Label(item.Text);
                }
                GUI.enabled = true;

                if (!item.Disabled)
                {
                    Rect linePos = GUILayoutUtility.GetLastRect();

                    if (_keyboardSelection)
                    {
                        if (item.Id == _selectedId)
                            GUI.Box(linePos, string.Empty, EditorStyles.helpBox);
                    }
                    else if (Hovered(linePos))
                    {
                        _selectedId = item.Id;
                        linePos.x -= 1f;
                        linePos.width += 2f;
                        GUI.Box(linePos, string.Empty, EditorStyles.helpBox);
                    }
                }
            }
        }

        private void TryInvokeItem()
        {
            if (_selectedId >= 0)
            {
                _items[_selectedId].OnSelected();
                Close();
            }
        }

        private void TryMoveSelector(int direction)
        {
            if (_searchResult.Count > 0)
            {
                _keyboardSelection = true;
                int index = _searchResult.IndexOf(item => item.Id == _selectedId);

                if (index < 0 && direction < 0)
                    index = _searchResult.Count;

                index = (index + direction).Repeat(_searchResult.Count);
                _selectedId = _searchResult[index].Id;

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
            itemsCount += itemsCount == 0 ? 2 : 1;
            float linesHeight = EditorGUIUtility.singleLineHeight * itemsCount;
            float spacesHeight = EditorGUIUtility.standardVerticalSpacing * (itemsCount + 4);
            return (linesHeight + spacesHeight).CutAfter(Screen.currentResolution.height * 0.5f);
        }

        private float GetWidth(IList<Data> items)
        {
            Data itemWithLongestText = items.Where(item => !item.IsSeparator)
                                            .GetWithMax(item => item.Text.Length);
            _maxLabelSize = GUI.skin.label.CalcSize(new GUIContent(itemWithLongestText.Text));
            float lineWidth = EditorGUIUtilityExt.StandardHorizontalSpacing * 5f + EditorGUIUtilityExt.SmallButtonWidth + _maxLabelSize.x + 2f;
            return (lineWidth + lineWidth * 0.12f).Clamp(200f, Screen.currentResolution.width * 0.5f);
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
        }
    }
}
