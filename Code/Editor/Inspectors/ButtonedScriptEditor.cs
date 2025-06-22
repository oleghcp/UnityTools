using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using OlegHcp.CSharp;
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

            Span<bool> pressed = stackalloc bool[_methods.Count];

            for (int i = 0; i < _methods.Count; i++)
            {
                var (method, attribute) = _methods[i];
                string buttonName = attribute.ButtonName.IsNullOrEmpty() ? method.Name
                                                                         : attribute.ButtonName;
                pressed[i] = GUILayout.Button(buttonName, GUILayout.Height(attribute.Size));
            }

            for (int i = 0; i < pressed.Length; i++)
            {
                if (!pressed[i])
                    continue;

                MethodInfo method = _methods[i].Method;

                if (method.IsStatic)
                {
                    method.Invoke(null, null);
                    continue;
                }

                for (int j = 0; j < targets.Length; j++)
                {
                    method.Invoke(targets[j], null);
                }
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
            public MethodInfo Method;
            public InspectorButtonAttribute Attribute;

            public Data(MethodInfo method, InspectorButtonAttribute attribute)
            {
                Method = method;
                Attribute = attribute;
            }

            public void Deconstruct(out MethodInfo method, out InspectorButtonAttribute attribute)
            {
                method = Method;
                attribute = Attribute;
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
