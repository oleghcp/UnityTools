using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using OlegHcp.CSharp;
using OlegHcp.Inspector;
using UnityEditor;
using UnityEngine;

namespace OlegHcpEditor.Engine
{
    public abstract class MethodButtonsEditor : Editor
    {
        private static Dictionary<Type, (MethodInfo method, InspectorButtonAttribute a)[]> _globalStorage;

        private (MethodInfo method, InspectorButtonAttribute a)[] _methods;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            DrawMethodButtons();
        }

        protected void DrawMethodButtons()
        {
            var methods = GetMethodList();

            if (methods.Length == 0)
                return;

            EditorGUILayout.Space();

            Span<bool> pressed = stackalloc bool[methods.Length];

            for (int i = 0; i < methods.Length; i++)
            {
                var (method, a) = methods[i];
                string buttonName = a.ButtonName.IsNullOrEmpty() ? method.Name : a.ButtonName;

                pressed[i] = GUILayout.Button(buttonName, GUILayout.Height(a.Size));
            }

            for (int i = 0; i < pressed.Length; i++)
            {
                if (!pressed[i])
                    continue;

                MethodInfo method = methods[i].method;

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

        private (MethodInfo method, InspectorButtonAttribute a)[] GetMethodList()
        {
            if (_methods == null)
            {
                if (_globalStorage == null)
                    _globalStorage = new Dictionary<Type, (MethodInfo method, InspectorButtonAttribute a)[]>();

                Type targetType = target.GetType();

                if (!_globalStorage.TryGetValue(targetType, out _methods))
                {
                    _methods = targetType.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public)
                                         .Select(item => (item, item.GetCustomAttribute<InspectorButtonAttribute>(true)))
                                         .Where(item => item.Item2 != null)
                                         .ToArray();

                    _globalStorage.Add(targetType, _methods);
                }
            }

            return _methods;
        }
    }
}
