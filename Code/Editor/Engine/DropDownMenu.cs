using System;
using UnityEngine;
using UnityUtilityEditor.Window;

namespace UnityUtilityEditor.Engine
{
    public class DropDownMenu
    {
        private DropDownWindow _window;

        public DropDownMenu()
        {
            _window = ScriptableObject.CreateInstance<DropDownWindow>();
        }

        public void ShowMenu()
        {
            _window.ShowMenu();
        }

        public void ShowMenu(Vector2 position)
        {
            _window.ShowMenu(position);
        }

        public void ShowMenu(in Rect buttonRect)
        {
            _window.ShowMenu(buttonRect);
        }

        public void AddItem(string content, Action onSelected)
        {
            _window.AddItem(content, onSelected);
        }

        public void AddItem(string content, bool on, Action onSelected)
        {
            _window.AddItem(content, on, onSelected);
        }

        public void AddDisabledItem(string content)
        {
            _window.AddDisabledItem(content);
        }

        public void AddDisabledItem(string content, bool on)
        {
            _window.AddDisabledItem(content, on);
        }

        public void AddSeparator()
        {
            _window.AddSeparator();
        }
    }
}
