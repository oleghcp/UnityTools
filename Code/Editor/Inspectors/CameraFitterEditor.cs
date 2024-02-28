using System;
using OlegHcp;
using OlegHcp.Mathematics;
using OlegHcpEditor.Engine;
using OlegHcpEditor.MenuItems;
using UnityEditor;
using UnityEngine;

namespace OlegHcpEditor.Inspectors
{
    [CustomEditor(typeof(CameraFitter))]
    internal class CameraFitterEditor : Editor<CameraFitter>
    {
        private Camera _camera;

        private SerializedProperty _mode;
        private SerializedProperty _verticalSize;
        private SerializedProperty _horizontalSize;
        private SerializedProperty _verticalFov;
        private SerializedProperty _horizontalFov;
        private Func<Vector2> _getSizeOfMainGameView;
        private bool _widthToHeight;

        private bool _orthographic;
        private float _currentViewRatio;

        private void OnEnable()
        {
            _mode = serializedObject.FindProperty(CameraFitter.ModeFieldName);
            _verticalSize = serializedObject.FindProperty(CameraFitter.VerticalFieldSizeName);
            _horizontalSize = serializedObject.FindProperty(CameraFitter.HorizontalFieldSizeName);
            _verticalFov = serializedObject.FindProperty(CameraFitter.VerticalFieldFovName);
            _horizontalFov = serializedObject.FindProperty(CameraFitter.HorizontalFieldFovName);

            SerializedProperty cameraProp = serializedObject.FindProperty(CameraFitter.CameraFieldName);

            if (cameraProp.objectReferenceValue == null)
                cameraProp.objectReferenceValue = target.GetComponent<Camera>();

            EditorApplication.update += OnUpdate;

            _widthToHeight = EditorPrefs.GetBool(PrefsKeys.WIDTH_TO_HEIGHT);

            _camera = (Camera)cameraProp.objectReferenceValue;
            _getSizeOfMainGameView = Delegate.CreateDelegate(typeof(Func<Vector2>), Type.GetType("UnityEditor.GameView,UnityEditor"), "GetSizeOfMainGameView") as Func<Vector2>;

            _currentViewRatio = GetGameViewRatio();
            _orthographic = _camera.orthographic;

            if (_verticalSize.floatValue == 0f)
                _verticalSize.floatValue = _camera.orthographicSize;

            if (_verticalFov.floatValue == 0f)
                _verticalFov.floatValue = _camera.fieldOfView;

            if (_horizontalSize.floatValue == 0f)
                _horizontalSize.floatValue = _verticalSize.floatValue;

            if (_horizontalFov.floatValue == 0f)
                _horizontalFov.floatValue = _verticalFov.floatValue;

            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private void OnDisable()
        {
            EditorPrefs.SetBool(PrefsKeys.WIDTH_TO_HEIGHT, _widthToHeight);
            EditorApplication.update -= OnUpdate;
        }

        private void OnUpdate()
        {
            CheckAndApplyParamChanges(false);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            Draw();
            serializedObject.ApplyModifiedProperties();

            CheckAndApplyParamChanges(GUI.changed);
        }

        private void OnSceneGUI()
        {
            if (!_camera.orthographic)
                return;

            if (_mode.enumValueIndex != ((int)AspectMode.EnvelopeAspect))
                return;

            Transform transform = target.transform;
            Handles.color = Colours.Red;

            Vector3 upVector = transform.up * _verticalSize.floatValue;
            Vector3 rightVector = transform.right * _horizontalSize.floatValue;

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

            bool ortho = _camera.orthographic;

            switch ((AspectMode)_mode.enumValueIndex)
            {
                case AspectMode.FixedHeight:
                    if (ortho)
                        drawSize(_verticalSize, "Vertical Size");
                    else
                        drawFov(_verticalFov, "Vertical Fov");
                    break;

                case AspectMode.FixedWidth:
                    if (ortho)
                        drawSize(_horizontalSize, "Horizontal Size");
                    else
                        drawFov(_horizontalFov, "Horizontal Fov");
                    break;

                case AspectMode.EnvelopeAspect:
                    if (ortho)
                    {
                        float ratio = drawRatio(_horizontalSize.floatValue, _verticalSize.floatValue);
                        drawSize(_verticalSize, "Target Vertical Size");

                        _horizontalSize.floatValue = GetWidth(_verticalSize.floatValue, ratio);
                        drawSize(_horizontalSize, "Target Horizontal Size");
                    }
                    else
                    {
                        float hTan = ScreenUtility.GetHalfFovTan(_horizontalFov.floatValue);
                        float vTan = ScreenUtility.GetHalfFovTan(_verticalFov.floatValue);
                        float ratio = drawRatio(hTan, vTan);
                        drawFov(_verticalFov, "Target Vertical Fov");

                        vTan = ScreenUtility.GetHalfFovTan(_verticalFov.floatValue);
                        _horizontalFov.floatValue = ScreenUtility.GetFovFromHalfTan(GetWidth(vTan, ratio));
                        drawFov(_horizontalFov, "Target Horizontal Fov");
                    }
                    break;

                default:
                    throw new UnsupportedValueException((AspectMode)_mode.enumValueIndex);
            }

            void drawSize(SerializedProperty property, string label)
            {
                property.floatValue = EditorGUILayout.FloatField(label, property.floatValue).ClampMin(0.01f);
            }

            void drawFov(SerializedProperty property, string label)
            {
                property.floatValue = EditorGUILayout.Slider(label, property.floatValue, 1f, 179f);
            }

            float drawRatio(float width, float height)
            {
                if (GUILayout.Button("- / -", GUILayout.Width(60f), GUILayout.Height(EditorGUIUtility.singleLineHeight)))
                    _widthToHeight = !_widthToHeight;
                float calculatedRatio = GetRatio(width, height);
                float ratio = EditorGUILayout.FloatField(GetRatioLabel(), calculatedRatio).ClampMin(0f);
                return ratio;
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

        private float GetGameViewRatio()
        {
            Vector2 res = _getSizeOfMainGameView();
            return res.y / res.x;
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

        private void CheckAndApplyParamChanges(bool anythingElse)
        {
            float ratio = GetGameViewRatio();
            bool ortho = _camera.orthographic;

            if (ratio != _currentViewRatio || _orthographic != ortho || anythingElse)
            {
                _currentViewRatio = ratio;
                _orthographic = ortho;
                target.ApplyChanges(ratio, ortho);
                EditorUtility.SetDirty(_camera);
            }
        }

        [MenuItem(MenuItemsUtility.CONTEXT_MENU_NAME + nameof(CameraFitter) + "/" + MenuItemsUtility.RESET_ITEM_NAME)]
        private static void ResetMenuItem() { }
    }
}
