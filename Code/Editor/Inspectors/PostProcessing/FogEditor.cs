#if INCLUDE_POST_PROCESSING
using UnityEditor.Rendering.PostProcessing;
using UnityEngine;
using UnityUtility;
using UnityUtility.Mathematics;
using UnityUtility.PostProcessing;
using UnityUtilityEditor.Engine;

namespace UnityUtilityEditor.Inspectors.PostProcessing
{
    [PostProcessEditor(typeof(Fog))]
    public class FogEditor : PostProcessEffectEditor<Fog>
    {
        private SerializedParameterOverride _mode;
        private SerializedParameterOverride _color;
        private SerializedParameterOverride _param1;
        private SerializedParameterOverride _param2;
        private SerializedParameterOverride _shader;

        private int _fogMode = int.MinValue;

        public override void OnEnable()
        {
            _mode = FindParameterOverride(item => item.Mode);
            _color = FindParameterOverride(item => item.FogColor);
            _param1 = FindParameterOverride(item => item.Param1);
            _param2 = FindParameterOverride(item => item.Param2);
            _shader = FindParameterOverride(item => item.Shader);
        }

        public override void OnInspectorGUI()
        {
            PropertyField(_mode);
            HandleFogMode();
            PropertyField(_color);

            if (_mode.value.intValue == (int)FogMode.Linear)
            {
                PropertyField(_param1, EditorGuiUtility.TempContent("Start"));
                PropertyField(_param2, EditorGuiUtility.TempContent("End"));
                _param2.value.floatValue = _param2.value.floatValue.ClampMin(_param1.value.floatValue);
            }
            else
            {
                PropertyField(_param1, EditorGuiUtility.TempContent("Density"));
                _param1.value.floatValue = _param1.value.floatValue.Clamp01();

                PropertyField(_param2, EditorGuiUtility.TempContent("Offset"));
                _param2.value.floatValue = _param2.value.floatValue.ClampMin(0f);
            }
        }

        private void HandleFogMode()
        {
            int mode = _mode.value.intValue;

            if (_fogMode != mode)
            {
                _fogMode = mode;
                FogMode fogMode = (FogMode)mode;
                _shader.value.objectReferenceValue = Shader.Find(Fog.GetFogShaderPath(fogMode));

                switch (fogMode)
                {
                    case FogMode.Linear:
                        _param1.value.floatValue = 0f;
                        _param2.value.floatValue = 100f;
                        break;

                    case FogMode.Exponential:
                    case FogMode.ExponentialSquared:
                        _param1.value.floatValue = 0.001f;
                        _param2.value.floatValue = 0f;
                        break;

                    default:
                        throw new UnsupportedValueException(fogMode);
                }
            }
        }
    }
}
#endif
