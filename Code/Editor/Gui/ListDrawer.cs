using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityUtility.CSharp.Collections;
using UnityUtilityEditor.Engine;

namespace UnityUtilityEditor.Gui
{
    public interface IListElementDrawer<T>
    {
        T OnDrawElement(Rect position, int index, T element, bool isActive, bool isFocused);
        float OnElementHeight(int index);
    }

    public sealed class ListDrawer<T>
    {
        private CustomReorderableList _drawer;
        private IListElementDrawer<T> _elementDrawer;
        private string _label;
        private T[] _array = Array.Empty<T>();
        private List<T> _list = new List<T>();
        private bool _changed;

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

        public ListDrawer(string label, IListElementDrawer<T> elementDrawer)
        {
            if (elementDrawer == null)
                throw new ArgumentNullException();

            _label = label;
            _elementDrawer = elementDrawer;

            _drawer = new CustomReorderableList()
            {
                drawHeaderCallback = OnDrawHeader,
                drawElementCallback = OnDrawElement,
                elementHeightCallback = _elementDrawer.OnElementHeight,
                onAddCallback = OnAddElement,
                onRemoveCallback = OnRemoveElement,
                onChangedCallback = OnChanged,
            };
        }

        public T[] Draw(in Rect position, T[] array)
        {
            return (T[])DrawInternal(position, array);
        }

        public List<T> Draw(in Rect position, List<T> list)
        {
            return (List<T>)DrawInternal(position, list);
        }

        public T[] Draw(T[] array)
        {
            return (T[])DrawInternal(array);
        }

        public List<T> Draw(List<T> list)
        {
            return (List<T>)DrawInternal(list);
        }

        public float GetHeight()
        {
            if (_drawer == null)
                return EditorGUIUtility.singleLineHeight;

            return _drawer.GetHeight();
        }

        private IList<T> DrawInternal(Rect position, IList<T> list)
        {
            Copy(list);

            position.x += EditorGuiUtility.StandardHorizontalSpacing;
            position.width -= EditorGuiUtility.StandardHorizontalSpacing * 2f;
            _drawer.DoList(position);

            return GetChanges(list);
        }

        private IList<T> DrawInternal(IList<T> list)
        {
            Copy(list);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(EditorGuiUtility.StandardHorizontalSpacing);

#if !UNITY_2020_3_OR_NEWER
            using (new EditorGUILayout.VerticalScope())
#endif
                _drawer.DoLayoutList();

            GUILayout.Space(EditorGuiUtility.StandardHorizontalSpacing);
            EditorGUILayout.EndHorizontal();

            return GetChanges(list);
        }

        private void Copy(IList<T> list)
        {
            if (list is Array)
            {
                if (_array.Length != list.Count)
                    _array = new T[list.Count];

                list.CopyTo(_array, 0);
                _drawer.list = _array;
            }
            else
            {
                _list.Clear();
                _list.AddRange(list);
                _drawer.list = _list;
            }
        }

        private IList<T> GetChanges(IList<T> list)
        {
            if (_changed)
            {
                _changed = false;

                if (list is Array)
                    return _drawer.List.ToArray();

                return list.ToList();
            }

            return list;
        }

        private void OnDrawHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, _label);

            const float fieldWidth = 50f;
            rect.x += rect.width - fieldWidth;
            rect.width = fieldWidth;

            IList<T> list = _drawer.List;

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

                _changed = true;
            }
        }

        private void OnDrawElement(Rect position, int index, bool isActive, bool isFocused)
        {
            if (index >= _drawer.list.Count)
                return;

            EditorGUI.BeginChangeCheck();

            _drawer.list[index] = _elementDrawer.OnDrawElement(position, index, (T)_drawer.list[index], isActive, isFocused);

            if (EditorGUI.EndChangeCheck())
                _changed = true;
        }

        private void OnAddElement(ReorderableList _)
        {
            IList<T> list = _drawer.List;

            if (_drawer.list.IsFixedSize)
                list = list.ToList();

            list.Add(Activator.CreateInstance<T>());

            if (_drawer.list.IsFixedSize)
                _drawer.list = list.ToArray();

#if UNITY_2021_1_OR_NEWER
            _drawer.Select(list.Count - 1);
#endif
        }

        private void OnRemoveElement(ReorderableList _)
        {
#if UNITY_2021_1_OR_NEWER
            IList<int> indices = _drawer.selectedIndices;
#else
            IList<int> indices = new[] { _drawer.index };
#endif
            IList<T> list = _drawer.List;

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

#if UNITY_2021_1_OR_NEWER
            if (list.Count > 0)
                _drawer.Select(list.Count - 1);
#endif
        }

        private void OnChanged(ReorderableList _)
        {
            _changed = true;
        }

        private class CustomReorderableList : ReorderableList
        {
            public IList<T> List
            {
                get => (IList<T>)list;
                set => list = (IList)value;
            }

            public CustomReorderableList() : base(null, typeof(T))
            {
            }
        }
    }
}
