using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace UnityUtilityEditor.Inspectors
{
    //[CustomEditor(typeof(Transform))]
    internal class TransformEditor : Editor
    {
        private static Action<bool> _onSwitched;

        private Editor _editor;
        private Action _onSceneGUI;

        private void OnEnable()
        {
            bool builtIn = EditorPrefs.GetBool(PrefsConstants.BUILTIN_TRANSFORM_EDITOR_KEY);
            var (editor, onSceneGUI) = CreateEditor(target, builtIn);
            _editor = editor;
            _onSceneGUI = onSceneGUI;

            _onSwitched = OnSwitched;
        }

        private void OnDisable()
        {
            _onSwitched = null;
            DestroyImmediate(_editor);
        }

        public static void SwitchType()
        {
            bool builtIn = !EditorPrefs.GetBool(PrefsConstants.BUILTIN_TRANSFORM_EDITOR_KEY);
            EditorPrefs.SetBool(PrefsConstants.BUILTIN_TRANSFORM_EDITOR_KEY, builtIn);

            _onSwitched?.Invoke(builtIn);
        }

        public override void OnInspectorGUI()
        {
            _editor.OnInspectorGUI();
        }

        private void OnSceneGUI()
        {
            _onSceneGUI?.Invoke();
        }

        private static (Editor editor, Action onSceneGUI) CreateEditor(UnityObject target, bool builtInType)
        {
            if (builtInType)
            {
                Assembly assembly = Assembly.GetAssembly(typeof(Editor));
                Type type = assembly.GetType("UnityEditor.TransformInspector");
                return (CreateEditor(target, type), null);
            }

            Editor editor = CreateEditor(target, typeof(CustomTransformEditor));
            Action onSceneGUI = Delegate.CreateDelegate(typeof(Action), editor, nameof(OnSceneGUI)) as Action;
            return (editor, onSceneGUI);
        }

        private void OnSwitched(bool builtIn)
        {
            DestroyImmediate(_editor);
            var (editor, onSceneGUI) = CreateEditor(target, builtIn);
            _editor = editor;
            _onSceneGUI = onSceneGUI;
            Repaint();
        }
    }
}
