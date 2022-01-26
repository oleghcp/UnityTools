using UnityEditor;
using UnityEngine;
using UnityUtility;
using UnityUtility.MathExt;

namespace UnityUtilityEditor.Inspectors
{
    [CustomEditor(typeof(CameraFitter))]
    internal class CameraFitterEditor : Editor<CameraFitter>
    {
        private Camera _camera;

        private SerializedProperty _mode;
        private SerializedProperty _vertical;
        private SerializedProperty _horizontal;

        private bool _widthToHeight;

        private void OnEnable()
        {
            SerializedProperty cameraProp = serializedObject.FindProperty(CameraFitter.CameraFieldName);
            if (cameraProp.objectReferenceValue == null)
            {
                cameraProp.objectReferenceValue = target.GetComponent<Camera>();
                serializedObject.ApplyModifiedPropertiesWithoutUndo();
            }

            _camera = (Camera)cameraProp.objectReferenceValue;

            _mode = serializedObject.FindProperty(CameraFitter.ModeFieldName);
            _vertical = serializedObject.FindProperty(CameraFitter.VerticalFieldName);
            _horizontal = serializedObject.FindProperty(CameraFitter.HorizontalFieldName);
            _widthToHeight = EditorPrefs.GetBool(PrefsConstants.WIDTH_TO_HEIGHT_KEY);
        }

        private void OnDisable()
        {
            EditorPrefs.SetBool(PrefsConstants.WIDTH_TO_HEIGHT_KEY, _widthToHeight);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            Draw();
            serializedObject.ApplyModifiedProperties();
        }

        private void OnSceneGUI()
        {
            if (!target.Camera.orthographic)
                return;

            if (_mode.enumValueIndex != ((int)AspectMode.EnvelopeAspect))
                return;

            Transform transform = target.Camera.transform;
            Handles.color = Colours.Red;

            Vector3 upVector = transform.up * _vertical.floatValue;
            Vector3 rightVector = transform.right * _horizontal.floatValue;

            Vector3 left = transform.position - rightVector;
            Vector3 right = transform.position + rightVector;

            Vector3 a = left - upVector;
            Vector3 b = left + upVector;
            Vector3 c = right + upVector;
            Vector3 d = right - upVector;

            Handles.DrawLine(a, b);
            Handles.DrawLine(b, c);
            Handles.DrawLine(c, d);
            Handles.DrawLine(d, a);

            Handles.color = Colours.White;
        }

        private void Draw()
        {
            EditorGUILayout.PropertyField(_mode);

            if (_mode.enumValueIndex == ((int)AspectMode.FixedHeight))
                return;

            bool ortho = target.Camera.orthographic;

            if (_mode.enumValueIndex == ((int)AspectMode.FixedWidth))
            {
                if (ortho)
                    drawSize(_horizontal, "Horizontal Size");
                else
                    drawFov(_horizontal, "Horizontal Fov");

                return;
            }

            if (ortho)
            {
                float calculatedRatio = GetRatio(_horizontal.floatValue, _vertical.floatValue);

                drawSize(_vertical, "Target Vertical Size");
                drawSize(_horizontal, "Target Horizontal Size");
                drawRatio(calculatedRatio);
            }
            else
            {
                float hTan = ScreenUtility.GetHalfFovTan(_horizontal.floatValue);
                float vTan = ScreenUtility.GetHalfFovTan(_vertical.floatValue);
                float calculatedRatio = GetRatio(hTan, vTan);

                drawFov(_vertical, "Target Vertical Fov");
                drawFov(_horizontal, "Target Horizontal Fov");
                drawRatio(calculatedRatio);
            }

            void drawSize(SerializedProperty property, string label)
            {
                property.floatValue = EditorGUILayout.FloatField(label, property.floatValue).CutBefore(0f);
            }

            void drawFov(SerializedProperty property, string label)
            {
                property.floatValue = EditorGUILayout.Slider(label, property.floatValue, 1f, 179f);
            }

            void drawRatio(float calculatedRatio)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(GetRatioLabel(), calculatedRatio.ToString());
                if (GUILayout.Button("-/-", GUILayout.Width(30f), GUILayout.Height(EditorGUIUtility.singleLineHeight)))
                    _widthToHeight = !_widthToHeight;
                EditorGUILayout.EndHorizontal();
            }
        }

        private static void DrawLine(Vector3 axis, Vector3 center, float size)
        {
            Vector3 up = axis * size;
            Vector3 p1 = center - up;
            Vector3 p2 = center + up;
#if UNITY_2020_2_OR_NEWER
            Handles.DrawLine(p1, p2, 2f);
#else
            Handles.DrawLine(p1, p2);
#endif
        }

        private float GetRatio(float width, float height)
        {
            return _widthToHeight ? width / height : height / width;
        }

        private float GetWidth(float height, float ratio)
        {
            return _widthToHeight ? ratio * height : height / ratio;
        }

        private string GetRatioLabel()
        {
            return _widthToHeight ? "Width / Height" : "Height / Width";
        }

        [MenuItem(MenuItems.CONTEXT_MENU_NAME + nameof(CameraFitter) + "/" + MenuItems.RESET_ITEM_NAME)]
        private static void ResetMenuItem()
        {

        }
    }
}
