using System;
using System.Reflection;
using OlegHcpEditor;
using OlegHcpEditor.Engine;
using OlegHcpEditor.Inspectors;
using UnityEditor;
using UnityEngine;
using static OlegHcpEditor.Drawers.Attributes.SortingLayerIDDrawer;

namespace Inspectors
{
    [CustomEditor(typeof(MeshRenderer))]
    internal class MeshRendererEditor : Editor<MeshRenderer>
    {
        private DrawTool _drawer;
        private Editor _builtInEditor;

        private void OnEnable()
        {
            _drawer = new DrawTool();
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
            RenderSorterEditor.Draw(_drawer, target);
        }
    }
}
