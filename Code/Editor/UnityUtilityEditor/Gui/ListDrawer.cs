using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public sealed class ListDrawer<T>
    {
        private ReorderableList _list;
        private IListElementDrawer<T> _elementDrawer;
        private string _label;

        public ReorderableList List
        {
            get => _list;
            set => _list = value;
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

        private ListDrawer(IList elements, string label, IListElementDrawer<T> elementDrawer)
        {
            if (elements == null || elementDrawer == null)
                throw new ArgumentNullException();

            _label = label;
            _elementDrawer = elementDrawer;

            _list = new ReorderableList(elements, typeof(T))
            {
                drawHeaderCallback = OnDrawHeader,
                drawElementCallback = OnDrawElement,
                elementHeightCallback = _elementDrawer.OnElementHeight,
                onAddCallback = OnAddElement,
                onRemoveCallback = OnRemoveElement,
            };
        }

        public ListDrawer(List<T> elements, string label, IListElementDrawer<T> elementDrawer)
            : this((IList)elements, label, elementDrawer)
        {

        }

        public ListDrawer(T[] elements, string label, IListElementDrawer<T> elementDrawer)
            : this((IList)elements, label, elementDrawer)
        {

        }

        public IList<T> Draw(Rect position)
        {
            position.x += EditorGuiUtility.StandardHorizontalSpacing;
            position.width -= EditorGuiUtility.StandardHorizontalSpacing * 2f;
            _list.DoList(position);
            return (IList<T>)_list.list;
        }

        public IList<T> Draw()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(EditorGuiUtility.StandardHorizontalSpacing);
            _list.DoLayoutList();
            GUILayout.Space(EditorGuiUtility.StandardHorizontalSpacing);
            EditorGUILayout.EndHorizontal();
            return (IList<T>)_list.list;
        }

        public float GetHeight()
        {
            if (_list == null)
                return EditorGUIUtility.singleLineHeight;

            return _list.GetHeight();
        }

        private void OnDrawHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, _label);

            const float fieldWidth = 50f;
            rect.x += rect.width - fieldWidth;
            rect.width = fieldWidth;

            IList<T> list = _list.list as IList<T>;

            int newCount = EditorGUI.DelayedIntField(rect, list.Count);

            if (list.Count != newCount)
            {
                if (_list.list.IsFixedSize)
                    list = list.ToList();

                Action change = list.Count > newCount ? (Action)(() => list.Pop())
                                                      : () => list.Add(Activator.CreateInstance<T>());

                while (list.Count != newCount)
                    change();

                if (_list.list.IsFixedSize)
                    _list.list = list.ToArray();
            }
        }

        private void OnDrawElement(Rect position, int index, bool isActive, bool isFocused)
        {
            if (index >= _list.list.Count)
                return;

            T element = (T)_list.list[index];
            _elementDrawer.OnDrawElement(position, ref element, isActive, isFocused);
            _list.list[index] = element;
        }

        private void OnAddElement(ReorderableList _)
        {
            IList<T> list = _list.list as IList<T>;

            if (_list.list.IsFixedSize)
                list = list.ToList();

            list.Add(Activator.CreateInstance<T>());

            if (_list.list.IsFixedSize)
                _list.list = list.ToArray();

            _list.Select(list.Count - 1);
        }

        private void OnRemoveElement(ReorderableList _)
        {
            IList<int> indices = _list.selectedIndices;
            IList<T> list = _list.list as IList<T>;

            if (_list.list.IsFixedSize)
                list = list.ToList();

            if (indices.Count == 0)
            {
                list.Pop();
            }
            else
            {
                for (int i = 0; i < indices.Count; i++)
                {
                    list.RemoveAt(indices[i]);
                }
            }

            if (_list.list.IsFixedSize)
                _list.list = list.ToArray();

            if (list.Count > 0)
                _list.Select(list.Count - 1);
        }
    }
}
