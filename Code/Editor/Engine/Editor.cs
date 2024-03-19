using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using OlegHcp.CSharp;
using OlegHcp.Inspector;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace OlegHcpEditor.Engine
{
    public abstract class Editor<T> : Editor, IReadOnlyList<T> where T : UnityObject
    {
        private (MethodInfo method, InspectorButtonAttribute a)[] _methods;

#pragma warning disable IDE1006
        public new T target
        {
            get => base.target as T;
            set => base.target = value;
        }

        public new IReadOnlyList<T> targets => this;
#pragma warning restore IDE1006

        T IReadOnlyList<T>.this[int index] => (T)base.targets[index];
        int IReadOnlyCollection<T>.Count => base.targets.Length;

        private void OnEnable()
        {
            BindingFlags mask = BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;
            _methods = target.GetType()
                             .GetMethods(mask)
                             .Select(item => (item, item.GetCustomAttribute<InspectorButtonAttribute>(true)))
                             .Where(item => item.Item2 != null)
                             .ToArray();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (_methods.Length > 0 && targets.Count == 1)
            {
                EditorGUILayout.Space();
                DrawButtons(serializedObject.targetObject);
            }
        }

        private void DrawButtons(UnityObject targetObject)
        {
            for (int i = 0; i < _methods.Length; i++)
            {
                var (method, a) = _methods[i];

                if (GUILayout.Button(a.ButtonName.IsNullOrEmpty() ? method.Name : a.ButtonName, GUILayout.Height(a.Size)))
                    method.Invoke(method.IsStatic ? null : targetObject, null);
            }
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            UnityObject[] array = base.targets;

            for (int i = 0; i < array.Length; i++)
            {
                yield return (T)array[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return base.targets.GetEnumerator();
        }
    }
}
