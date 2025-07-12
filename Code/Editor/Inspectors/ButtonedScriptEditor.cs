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

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            InitMethodList();
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

            foreach (var (method, name, size) in _methods)
            {
                if (!GUILayout.Button(name, GUILayout.Height(size)))
                    continue;

                if (method.IsStatic)
                {
                    method.Invoke(null, null);
                }
                else
                {
                    for (int j = 0; j < targets.Length; j++)
                    {
                        method.Invoke(targets[j], null);
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

        private struct Data
        {
            private InspectorButtonAttribute _attribute;

            public MethodInfo Method { get; }

            public Data(MethodInfo method, InspectorButtonAttribute attribute)
            {
                Method = method;
                _attribute = attribute;

                if (_attribute.ButtonName == null)
                    _attribute.ButtonName = ObjectNames.NicifyVariableName(method.Name);
            }

            public void Deconstruct(out MethodInfo method, out string name, out float size)
            {
                method = Method;
                name = _attribute.ButtonName;
                size = _attribute.Size;
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
