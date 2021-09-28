using System;
using System.Collections;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace UnityUtilityEditor.Gui
{
    public interface IListElementDrawer<T>
    {
        void OnDrawElement(Rect position, ref T element, bool isActive, bool isFocused);
        float OnElementHeight(int index);
    }

    public class ListDrawer<T>
    {
        private const string ERROR = "List of elements has fixed size.";

        private ReorderableList _list;
        private IListElementDrawer<T> _elementDrawer;
        private string _label;

        public IList List
        {
            get => _list.list;
            set
            {
                if (value == null)
                    throw new ArgumentNullException();

                _list.list = value;
            }
        }

        public string Label
        {
            get => _label;
            set => _label = value;
        }

        public IListElementDrawer<T> ElementDrawer
        {
            get => _elementDrawer;
            set
            {
                if (value == null)
                    throw new ArgumentNullException();

                _elementDrawer = value;
            }
        }

        public ListDrawer(IList elements, string label, IListElementDrawer<T> elementDrawer)
        {
            if (elements == null || elementDrawer == null)
                throw new ArgumentNullException();

            _label = label;
            _list = new ReorderableList(elements, typeof(T));
            _elementDrawer = elementDrawer;

            _list.drawHeaderCallback = rect => EditorGUI.LabelField(rect, _label);
            _list.drawElementCallback = OnDrawElement;
            _list.elementHeightCallback = index => _elementDrawer.OnElementHeight(index);
        }

        public void Draw(Rect position)
        {
            if (_list.list.IsFixedSize)
            {
                EditorGUILayout.LabelField(_label, ERROR);
                return;
            }

            position.x += EditorGuiUtility.StandardHorizontalSpacing;
            position.width -= EditorGuiUtility.StandardHorizontalSpacing * 2f;
            _list.DoList(position);
        }

        public void Draw()
        {
            if (_list.list.IsFixedSize)
            {
                EditorGUILayout.LabelField(_label, ERROR);
                return;
            }

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(EditorGuiUtility.StandardHorizontalSpacing);
            _list.DoLayoutList();
            GUILayout.Space(EditorGuiUtility.StandardHorizontalSpacing);
            EditorGUILayout.EndHorizontal();
        }

        public float GetHeight()
        {
            if (_list == null)
                return EditorGUIUtility.singleLineHeight;

            return _list.GetHeight();
        }

        private void OnDrawElement(Rect position, int index, bool isActive, bool isFocused)
        {
            T element = (T)_list.list[index];
            _elementDrawer.OnDrawElement(position, ref element, isActive, isFocused);
            _list.list[index] = element;
        }
    }
}
