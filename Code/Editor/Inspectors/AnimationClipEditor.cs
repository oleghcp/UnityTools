using System;
using System.Reflection;
using OlegHcpEditor.Engine;
using UnityEditor;
using UnityEngine;

namespace OlegHcpEditor.Inspectors
{
    [CustomEditor(typeof(AnimationClip))]
    internal class AnimationClipEditor : Editor<AnimationClip>
    {
        private Editor _builtInEditor;

        private void OnEnable()
        {
            Type type = Assembly.GetAssembly(typeof(Editor))
                                .GetType("UnityEditor.AnimationClipEditor");
            _builtInEditor = CreateEditor(target, type);
        }

        private void OnDisable()
        {
            _builtInEditor.DestroyImmediate();
        }

        public override bool HasPreviewGUI()
        {
            return _builtInEditor.HasPreviewGUI();
        }

        public override void OnPreviewSettings()
        {
            _builtInEditor.OnPreviewSettings();
        }

        public override void OnInteractivePreviewGUI(Rect r, GUIStyle background)
        {
            _builtInEditor.OnInteractivePreviewGUI(r, background);
        }

        public override void OnInspectorGUI()
        {
            bool toggleValue = EditorGUILayout.Toggle("Legacy", target.legacy);

            if (toggleValue == target.legacy)
            {
                _builtInEditor.OnInspectorGUI();
                return;
            }

            Undo.RegisterCompleteObjectUndo(target, "Switch Legacy");
            target.legacy = !target.legacy;
            EditorUtility.SetDirty(target);
        }
    }
}
