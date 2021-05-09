using UnityEditor;
using UnityUtility;

namespace UnityUtilityEditor.Inspectors
{
    [CustomEditor(typeof(CameraFitter))]
    internal class CameraFitterEditor : Editor<CameraFitter>
    {
        private SerializedProperty _mode;
        private SerializedProperty _vertical;
        private SerializedProperty _horizontal;

        private void OnEnable()
        {
            _mode = serializedObject.FindProperty(CameraFitter.ModeFieldName);
            _vertical = serializedObject.FindProperty(CameraFitter.VerticalFieldName);
            _horizontal = serializedObject.FindProperty(CameraFitter.HorizontalFieldName);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            Draw();
            serializedObject.ApplyModifiedProperties();
        }

        private void Draw()
        {
            EditorGUILayout.PropertyField(_mode);

            if (_mode.enumValueIndex == ((int)AspectMode.FixedHeight))
                return;

            bool ortho = target.Camera.orthographic;

            if (_mode.enumValueIndex == ((int)AspectMode.FixedWidth))
            {
                EditorGUILayout.PropertyField(_horizontal, EditorGuiUtility.TempContent(ortho ? "Horizontal Size" : "Horizontal Fov"));
                return;
            }

            EditorGUILayout.PropertyField(_horizontal, EditorGuiUtility.TempContent(ortho ? "Target Horizontal Size" : "Target Horizontal Fov"));
            EditorGUILayout.PropertyField(_vertical, EditorGuiUtility.TempContent(ortho ? "Target Vertical Size" : "Target Vertical Fov"));
        }
    }
}
