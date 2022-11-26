using System;
using UnityEditor;
using UnityEngine;
using UnityUtility;
using UnityUtility.Mathematics;

namespace UnityUtilityEditor.Inspectors
{
    [CustomEditor(typeof(CameraFitter))]
    internal class CameraFitterEditor : Editor<CameraFitter>
    {
        private Camera _camera;

        private SerializedProperty _mode;
        private SerializedProperty _vertical;
        private SerializedProperty _horizontal;

        private Func<Vector2> _getSizeOfMainGameView;
        private float _currentViewRatio;
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
            _widthToHeight = EditorPrefs.GetBool(PrefsKeys.WIDTH_TO_HEIGHT);

            _getSizeOfMainGameView = Delegate.CreateDelegate(typeof(Func<Vector2>), Type.GetType("UnityEditor.GameView,UnityEditor"), "GetSizeOfMainGameView") as Func<Vector2>;
            _currentViewRatio = GetGeameViewRatio();
        }

        private void OnDisable()
        {
            EditorPrefs.SetBool(PrefsKeys.WIDTH_TO_HEIGHT, _widthToHeight);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            Draw();
            serializedObject.ApplyModifiedProperties();

            float ratio = GetGeameViewRatio();
            if (ratio != _currentViewRatio || GUI.changed)
            {
                _currentViewRatio = ratio;
                target.ApplyChanges(ratio);
                EditorUtility.SetDirty(_camera);
            }
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

            bool ortho = target.Camera.orthographic;

            switch ((AspectMode)_mode.enumValueIndex)
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

        private float GetGeameViewRatio()
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

        [MenuItem(MenuItems.CONTEXT_MENU_NAME + nameof(CameraFitter) + "/" + MenuItems.RESET_ITEM_NAME)]
        private static void ResetMenuItem()
        {

        }
    }
}
