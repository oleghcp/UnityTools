using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityUtility.MathExt;

namespace UnityEditor
{
    public class DropDownList : EditorWindow
    {
        private List<Data> _items = new List<Data>();
        private SearchField _searchField;

        private Vector2 _scrollPos;
        private string _tapeString;
        private IEnumerable<Data> _searchResult;
        private Vector2 _maxLabelSize;

        public int ItemsCount => _items.Count;

        public static DropDownList Create()
        {
            return CreateInstance<DropDownList>();
        }

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

            if (Event.current.keyCode == KeyCode.Escape)
            {
                Close();
                return;
            }

            using (new GUILayout.AreaScope(new Rect(Vector2.zero, position.size), (string)null, EditorStyles.helpBox))
            {
                string tapeString = _searchField.OnGUI(_tapeString);

                if (_tapeString != tapeString)
                {
                    _tapeString = tapeString;
                    _searchResult = Search(tapeString);
                }

                _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, false, false);
                DrawResults(_searchResult);
                EditorGUILayout.EndScrollView();
            }
        }

        #region List functions
        public void ShowMenu()
        {
            ShowMenu(new Rect(Event.current.mousePosition, Vector2.zero));
        }

        public void ShowMenu(Rect buttonRect)
        {
            if (_items.Count == 0)
                return;

            buttonRect = GUIUtility.GUIToScreenRect(buttonRect);
            ShowAsDropDown(buttonRect, new Vector2(getWidth(), getHeight()));

            float getHeight()
            {
                float linesHeight = EditorGUIUtility.singleLineHeight * (_items.Count + 1);
                float spacesHeight = EditorGUIUtility.standardVerticalSpacing * _items.Count;
                return (linesHeight + spacesHeight).CutAfter(Screen.currentResolution.height * 0.5f);
            }

            float getWidth()
            {
                Data itemWithLongestText = _items.Where(item => !item.IsSeparator).GetWithMax(item => item.Text.Length);
                _maxLabelSize = GUI.skin.label.CalcSize(new GUIContent(itemWithLongestText.Text));
                float lineWidth = EditorGUIUtilityExt.StandardHorizontalSpacing * 5f + EditorGUIUtilityExt.SmallButtonWidth + _maxLabelSize.x + 2f;
                return (lineWidth + lineWidth * 0.12f).Clamp(200f, Screen.currentResolution.width * 0.5f);
            }
        }

        public void AddDisabledItem(string content)
        {
            content = content ?? string.Empty;
            _items.Add(new Data { Text = content, Disabled = true, });
        }

        public void AddDisabledItem(string content, bool on)
        {
            content = content ?? string.Empty;
            _items.Add(new Data { Text = content, Disabled = true, On = on, });
        }

        public void AddItem(string content, Action onSelected)
        {
            content = content ?? string.Empty;
            _items.Add(new Data { Text = content, OnSelected = onSelected, });
        }

        public void AddItem(string content, bool on, Action onSelected)
        {
            content = content ?? string.Empty;
            _items.Add(new Data { Text = content, On = on, OnSelected = onSelected, });
        }

        public void AddSeparator()
        {
            _items.Add(new Data());
        }
        #endregion

        private IEnumerable<Data> Search(string tapeString)
        {
            if (tapeString.IsNullOrWhiteSpace())
                return _items;

            return _items.Where(item => !item.IsSeparator && item.Text.IndexOf(tapeString, StringComparison.CurrentCultureIgnoreCase) == 0);
        }

        private void DrawResults(IEnumerable<Data> searchResult)
        {
            foreach (Data item in searchResult)
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
                        GUILayout.Label("√", "Button", GUILayout.Width(EditorGUIUtilityExt.SmallButtonWidth));
                    else
                        GUILayout.Label(string.Empty, GUILayout.Width(EditorGUIUtilityExt.SmallButtonWidth));

                    GUILayout.Label(item.Text);
                }
                GUI.enabled = true;

                if (!item.Disabled)
                {
                    Rect linePos = GUILayoutUtility.GetLastRect();
                    linePos.x -= 1f;
                    linePos.width += 2f;

                    if (Hovered(linePos))
                        GUI.Box(linePos, string.Empty, EditorStyles.helpBox);

                    if (GetClick(linePos))
                    {
                        item.OnSelected();
                        Close();
                    }
                }
            }
        }

        private bool Hovered(in Rect uiElementPos)
        {
            Event curEvent = Event.current;
            return uiElementPos.Contains(curEvent.mousePosition);
        }

        private bool GetClick(in Rect uiElementPos)
        {
            Event curEvent = Event.current;
            return curEvent.type == EventType.MouseDown && uiElementPos.Contains(curEvent.mousePosition);
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct Data
        {
            public string Text;
            public Action OnSelected;
            public bool On;
            public bool Disabled;

            public bool IsSeparator => Text == null;
        }
    }
}
