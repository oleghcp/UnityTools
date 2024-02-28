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
        private SerializedProperty _vertical;
        private SerializedProperty _horizontal;

        private Func<Vector2> _getSizeOfMainGameView;
        private bool _widthToHeight;

        private bool _orthographic;
        private float _currentViewRatio;

        private void OnEnable()
        {
            _mode = serializedObject.FindProperty(CameraFitter.ModeFieldName);
            _vertical = serializedObject.FindProperty(CameraFitter.VerticalFieldName);
            _horizontal = serializedObject.FindProperty(CameraFitter.HorizontalFieldName);

            SerializedProperty cameraProp = serializedObject.FindProperty(CameraFitter.CameraFieldName);

            if (cameraProp.objectReferenceValue == null)
                cameraProp.objectReferenceValue = target.GetComponent<Camera>();

            EditorApplication.update += OnUpdate;

            _widthToHeight = EditorPrefs.GetBool(PrefsKeys.WIDTH_TO_HEIGHT);

            _camera = (Camera)cameraProp.objectReferenceValue;
            _getSizeOfMainGameView = Delegate.CreateDelegate(typeof(Func<Vector2>), Type.GetType("UnityEditor.GameView,UnityEditor"), "GetSizeOfMainGameView") as Func<Vector2>;

            _currentViewRatio = GetGameViewRatio();
            _orthographic = _camera.orthographic;

            if (_vertical.floatValue == 0f)
            {
                _vertical.floatValue = _orthographic ? _camera.orthographicSize
                                                     : _camera.fieldOfView;
            }

            if (_horizontal.floatValue == 0f)
                _horizontal.floatValue = _vertical.floatValue;

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

            bool ortho = _camera.orthographic;
            int mode = _mode.enumValueIndex;

            switch ((AspectMode)mode)
            {
                case AspectMode.FixedHeight:
                    drawFixedSide(_vertical, "Vertical");
                    break;

                case AspectMode.FixedWidth:
                    drawFixedSide(_horizontal, "Horizontal");
                    break;

                case AspectMode.EnvelopeAspect:
                    if (ortho)
                    {
                        float ratio = drawRatio(_horizontal.floatValue, _vertical.floatValue);
                        drawSize(_vertical, "Target Vertical Size");

                        _horizontal.floatValue = GetWidth(_vertical.floatValue, ratio);
                        drawSize(_horizontal, "Target Horizontal Size");
                    }
                    else
                    {
                        float hTan = ScreenUtility.GetHalfFovTan(_horizontal.floatValue);
                        float vTan = ScreenUtility.GetHalfFovTan(_vertical.floatValue);
                        float ratio = drawRatio(hTan, vTan);
                        drawFov(_vertical, "Target Vertical Fov");

                        vTan = ScreenUtility.GetHalfFovTan(_vertical.floatValue);
                        _horizontal.floatValue = ScreenUtility.GetFovFromHalfTan(GetWidth(vTan, ratio));
                        drawFov(_horizontal, "Target Horizontal Fov");
                    }
                    break;

                default:
                    throw new UnsupportedValueException((AspectMode)_mode.enumValueIndex);
            }

            void drawFixedSide(SerializedProperty property, string side)
            {
                if (ortho)
                    drawSize(property, $"{side} Size");
                else
                    drawFov(property, $"{side} Fov");
            }

            void drawSize(SerializedProperty property, string label)
            {
                property.floatValue = EditorGUILayout.FloatField(label, property.floatValue).ClampMin(0f);
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
