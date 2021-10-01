using UnityEditor;
using UnityEngine;
using UnityUtility;
using UnityUtility.MathExt;

namespace UnityUtilityEditor.Inspectors
{
    [CustomEditor(typeof(CameraFitter))]
    internal class CameraFitterEditor : Editor<CameraFitter>
    {
        private SerializedProperty _mode;
        private SerializedProperty _vertical;
        private SerializedProperty _horizontal;

        private bool _widthToHeight;

        private void OnEnable()
        {
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
            if (target.Camera.orthographic)
            {
                Transform transform = target.Camera.transform;
                Handles.color = Colours.Red;
                if (_mode.enumValueIndex == ((int)AspectMode.FixedHeight))
                {
                    DrawLine(transform.up, transform.position, _vertical.floatValue);
                }
                else if (_mode.enumValueIndex == ((int)AspectMode.FixedWidth))
                {
                    DrawLine(transform.right, transform.position, _horizontal.floatValue);
                }
                else
                {
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
                }
                Handles.color = Colours.White;
            }
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
                    DrawSize(_horizontal, "Horizontal Size");
                else
                    DrawFov(_horizontal, "Horizontal Fov");

                return;
            }

            if (ortho)
            {
                float calculatedRatio = GetRatio(_horizontal.floatValue, _vertical.floatValue);

                EditorGUILayout.BeginHorizontal();
                float ratio = EditorGUILayout.FloatField(GetRatioLabel(), calculatedRatio).CutBefore(0f);
                if (GUILayout.Button("*", GUILayout.Width(EditorGuiUtility.SmallButtonWidth), GUILayout.Height(EditorGUIUtility.singleLineHeight)))
                    _widthToHeight = !_widthToHeight;
                EditorGUILayout.EndHorizontal();

                if (calculatedRatio != ratio)
                    _horizontal.floatValue = GetWidth(_vertical.floatValue, ratio);

                DrawSize(_horizontal, "Target Horizontal Size");
                DrawSize(_vertical, "Target Vertical Size");
            }
            else
            {
                float hTan = ScreenUtility.GetHalfFovTan(_horizontal.floatValue);
                float vTan = ScreenUtility.GetHalfFovTan(_vertical.floatValue);
                float calculatedRatio = GetRatio(hTan, vTan);

                EditorGUILayout.BeginHorizontal();
                float ratio = EditorGUILayout.FloatField(GetRatioLabel(), calculatedRatio).CutBefore(0f);
                if (GUILayout.Button("*", GUILayout.Width(EditorGuiUtility.SmallButtonWidth), GUILayout.Height(EditorGUIUtility.singleLineHeight)))
                    _widthToHeight = !_widthToHeight;
                EditorGUILayout.EndHorizontal();

                if (calculatedRatio != ratio)
                    _horizontal.floatValue = ScreenUtility.GetFovFromHalfTan(GetWidth(vTan, ratio));

                DrawFov(_horizontal, "Target Horizontal Fov");
                DrawFov(_vertical, "Target Vertical Fov");
            }
        }

        private static void DrawSize(SerializedProperty property, string label)
        {
            property.floatValue = EditorGUILayout.FloatField(label, property.floatValue).CutBefore(0f);
        }

        private static void DrawFov(SerializedProperty property, string label)
        {
            property.floatValue = EditorGUILayout.Slider(label, property.floatValue, 1f, 179f);
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
    }
}
