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
        private ReorderableList _drawer;
        private IListElementDrawer<T> _elementDrawer;
        private string _label;

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

            _drawer = new ReorderableList(elements, typeof(T))
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
            _drawer.DoList(position);
            return (IList<T>)_drawer.list;
        }

        public IList<T> Draw()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(EditorGuiUtility.StandardHorizontalSpacing);
            _drawer.DoLayoutList();
            GUILayout.Space(EditorGuiUtility.StandardHorizontalSpacing);
            EditorGUILayout.EndHorizontal();
            return (IList<T>)_drawer.list;
        }

        public float GetHeight()
        {
            if (_drawer == null)
                return EditorGUIUtility.singleLineHeight;

            return _drawer.GetHeight();
        }

        private void OnDrawHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, _label);

            const float fieldWidth = 50f;
            rect.x += rect.width - fieldWidth;
            rect.width = fieldWidth;

            IList<T> list = _drawer.list as IList<T>;

            int newCount = EditorGUI.DelayedIntField(rect, list.Count);

            if (list.Count != newCount)
            {
                if (_drawer.list.IsFixedSize)
                    list = list.ToList();

                var change = list.Count > newCount ? (Action)(() => list.Pop())
                                                   : () => list.Add(Activator.CreateInstance<T>());

                while (list.Count != newCount)
                    change();

                if (_drawer.list.IsFixedSize)
                    _drawer.list = list.ToArray();
            }
        }

        private void OnDrawElement(Rect position, int index, bool isActive, bool isFocused)
        {
            if (index >= _drawer.list.Count)
                return;

            T element = (T)_drawer.list[index];
            _elementDrawer.OnDrawElement(position, ref element, isActive, isFocused);
            _drawer.list[index] = element;
        }

        private void OnAddElement(ReorderableList _)
        {
            IList<T> list = _drawer.list as IList<T>;

            if (_drawer.list.IsFixedSize)
                list = list.ToList();

            list.Add(Activator.CreateInstance<T>());

            if (_drawer.list.IsFixedSize)
                _drawer.list = list.ToArray();

            _drawer.Select(list.Count - 1);
        }

        private void OnRemoveElement(ReorderableList _)
        {
            IList<int> indices = _drawer.selectedIndices;
            IList<T> list = _drawer.list as IList<T>;

            if (_drawer.list.IsFixedSize)
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

            if (_drawer.list.IsFixedSize)
                _drawer.list = list.ToArray();

            if (list.Count > 0)
                _drawer.Select(list.Count - 1);
        }
    }
}
