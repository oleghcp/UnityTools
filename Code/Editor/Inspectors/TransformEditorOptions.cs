using UnityEditor;
using UnityEngine;

namespace OlegHcpEditor.Inspectors
{
    internal class TransformEditorOptions
    {
        public readonly string UndoName = "Transform";
        public readonly string ButtonName = "X";

        public readonly string[] ToolbarNames = new[]
        {
            "Local",
            "World",
        };

        public readonly string PivotModeWarning = $"- {PivotMode.Center}";
        public readonly string PivotRotationWarning = $"- {PivotRotation.Global}";

        public readonly GUILayoutOption[] ModeOptions = new[] { GUILayout.Height(20f) };

        public readonly GUILayoutOption[] ButtonOptions = new[]
        {
            GUILayout.Height(EditorGUIUtility.singleLineHeight),
            GUILayout.Width(EditorGUIUtility.singleLineHeight),
        };

        public readonly GUILayoutOption[] AreaOptions = new[] { GUILayout.Width(EditorGUIUtility.singleLineHeight) };
        public readonly GUILayoutOption[] LabelOptions = new[] { GUILayout.Width(60f) };

        public bool World;
    }
}
