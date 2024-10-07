using System;
using System.Reflection;
using OlegHcpEditor;
using OlegHcpEditor.Engine;
using OlegHcpEditor.Gui;
using UnityEditor;
using UnityEngine;

namespace Inspectors
{
    [CustomEditor(typeof(MeshRenderer))]
    internal class MeshRendererEditor : Editor<MeshRenderer>
    {
        private SortingLayerDrawTool _drawer;
        private Editor _builtInEditor;
        private static bool _expanded;

        private void OnEnable()
        {
            _drawer = new SortingLayerDrawTool();
            Type type = Assembly.GetAssembly(typeof(Editor))
                                .GetType("UnityEditor.MeshRendererEditor");
            _builtInEditor = CreateEditor(target, type);
        }

        private void OnDisable()
        {
            _builtInEditor.DestroyImmediate();
        }

        public override void OnInspectorGUI()
        {
            _builtInEditor.OnInspectorGUI();

            _expanded = EditorGUILayout.Foldout(_expanded, "Sorting (ext.)", true);

            if (!_expanded)
                return;

            using (new EditorGUI.IndentLevelScope(1))
            {
                _drawer.Draw(target);
            }
        }
    }
}
