using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using OlegHcp.CSharp.Collections;
using OlegHcp.Inspector;
using UnityEditor;
using UnityEngine;

namespace OlegHcpEditor.Inspectors
{
    internal abstract class ButtonedScriptEditor : Editor
    {
        private static Dictionary<Type, List<Data>> _globalStorage;
        private List<Data> _methods;

        protected abstract Type ParentType { get; }

        private void OnEnable()
        {
            InitMethodList();

            if (_methods.Count > 0)
                EditorApplication.pauseStateChanged += OnPauseStateChanged;
        }

        private void OnDisable()
        {
            if (_methods.Count > 0)
                EditorApplication.pauseStateChanged -= OnPauseStateChanged;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            DrawButtons();
        }

        private void InitMethodList()
        {
            if (_methods != null)
                return;

            Type targetType = target.GetType();

            if (_globalStorage == null)
                _globalStorage = new Dictionary<Type, List<Data>>();
            else if (_globalStorage.TryGetValue(targetType, out _methods))
                return;

            _methods = _globalStorage.Place(targetType, new List<Data>());
            BindingFlags mask = BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly;

            do
            {
                _methods.AddRange(EnumerateMethods(targetType, mask));
                targetType = targetType.BaseType;
            } while (targetType != ParentType);
        }

        private void DrawButtons()
        {
            if (_methods.Count == 0)
                return;

            EditorGUILayout.Space();

            foreach (Data data in _methods)
            {
                if (!data.CanDraw)
                    continue;

                if (!GUILayout.Button(data.Name, GUILayout.Height(data.Size)))
                    continue;

                if (data.IsStatic)
                {
                    data.InvokeStatic();
                }
                else
                {
                    for (int j = 0; j < targets.Length; j++)
                    {
                        data.Invoke(targets[j]);
                    }
                }

                break;
            }
        }

        private static IEnumerable<Data> EnumerateMethods(Type targetType, BindingFlags mask)
        {
            return targetType.GetMethods(mask)
                             .Where(method => method.IsDefined(typeof(InspectorButtonAttribute)))
                             .Select(method => new Data(method, method.GetCustomAttribute<InspectorButtonAttribute>()));
        }

        private void OnPauseStateChanged(PauseState state)
        {
            Repaint();
        }

        private struct Data
        {
            private InspectorButtonAttribute _attribute;
            private MethodInfo _method;

            public bool IsStatic => _method.IsStatic;
            public float Size => _attribute.Size;
            public string Name => _attribute.ButtonName;

            public bool CanDraw
            {
                get
                {
                    switch (_attribute.ShowState)
                    {
                        case EditorPlayState.PlayModeActive: return EditorApplication.isPlaying && !EditorApplication.isPaused;
                        case EditorPlayState.PlayModeStopped: return !EditorApplication.isPlaying || EditorApplication.isPaused;
                        default: return true;
                    }
                }
            }

            public Data(MethodInfo method, InspectorButtonAttribute attribute)
            {
                _method = method;
                _attribute = attribute;

                if (_attribute.ButtonName == null)
                    _attribute.ButtonName = ObjectNames.NicifyVariableName(method.Name);
            }

            public void Invoke(object obj)
            {
                _method.Invoke(obj, null);
            }

            public void InvokeStatic()
            {
                _method.Invoke(null, null);
            }
        }

        [CustomEditor(typeof(MonoBehaviour), true, isFallback = true)]
        [CanEditMultipleObjects]
        private class MonoBehaviourEditor : ButtonedScriptEditor
        {
            protected override Type ParentType => typeof(MonoBehaviour);
        }

        [CustomEditor(typeof(ScriptableObject), true, isFallback = true)]
        [CanEditMultipleObjects]
        private class ScriptableObjectEditor : ButtonedScriptEditor
        {
            protected override Type ParentType => typeof(ScriptableObject);
        }
    }
}
