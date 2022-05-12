using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace UnityUtilityEditor.Inspectors
{
    [CustomEditor(typeof(Transform))]
    internal class TransformEditor : Editor
    {
        private Editor _editor;
        private Action _onSceneGUI;

        private void OnEnable()
        {
            bool builtInType = EditorPrefs.GetBool(PrefsConstants.BUILTIN_TRANSFORM_EDITOR_KEY);

            if (builtInType)
            {
                Assembly assembly = Assembly.GetAssembly(typeof(Editor));
                Type type = assembly.GetType("UnityEditor.TransformInspector");
                _editor = CreateEditor(target, type);
            }
            else
            {
                _editor = CreateEditor(target, typeof(CustomTransformEditor));
                _onSceneGUI = (Action)Delegate.CreateDelegate(typeof(Action), _editor, nameof(OnSceneGUI));
            }
        }

        private void OnDisable()
        {
            DestroyImmediate(_editor);
        }

        public override void OnInspectorGUI()
        {
            _editor.OnInspectorGUI();
        }

        private void OnSceneGUI()
        {
            _onSceneGUI?.Invoke();
        }
    }
}
