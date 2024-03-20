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
    public abstract class BaseEditor<T> : Editor<T>, IReadOnlyList<T> where T : UnityObject
    {
        private (MethodInfo method, InspectorButtonAttribute a)[] _methods;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            DrawMethodButtons();
        }

        protected void DrawMethodButtons()
        {
            var methods = GetMethodList();

            if (methods.Length == 0 && targets.Count > 1)
                return;

            EditorGUILayout.Space();

            UnityObject targetObject = serializedObject.targetObject;

            for (int i = 0; i < methods.Length; i++)
            {
                var (method, a) = methods[i];

                if (GUILayout.Button(a.ButtonName.IsNullOrEmpty() ? method.Name : a.ButtonName, GUILayout.Height(a.Size)))
                    method.Invoke(method.IsStatic ? null : targetObject, null);
            }
        }

        private (MethodInfo method, InspectorButtonAttribute a)[] GetMethodList()
        {
            if (_methods == null)
            {
                _methods = target.GetType()
                                 .GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public)
                                 .Select(item => (item, item.GetCustomAttribute<InspectorButtonAttribute>(true)))
                                 .Where(item => item.Item2 != null)
                                 .ToArray();
            }

            return _methods;
        }
    }
}
