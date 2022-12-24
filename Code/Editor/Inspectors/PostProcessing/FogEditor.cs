#if INCLUDE_POST_PROCESSING
using UnityEditor.Rendering.PostProcessing;
using UnityEngine;
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
        private SerializedParameterOverride _aParam;
        private SerializedParameterOverride _bParam;
        private SerializedParameterOverride _shaderLinear;
        private SerializedParameterOverride _shaderExponential;
        private SerializedParameterOverride _shaderExpSquared;

        public override void OnEnable()
        {
            _mode = FindParameterOverride(item => item.Mode);
            _color = FindParameterOverride(item => item.FogColor);
            _aParam = FindParameterOverride(item => item.Param1);
            _bParam = FindParameterOverride(item => item.Param2);
            _shaderLinear = FindParameterOverride(item => item.ShaderLinear);
            _shaderExponential = FindParameterOverride(item => item.ShaderExponential);
            _shaderExpSquared = FindParameterOverride(item => item.ShaderExpSquared);
        }

        public override void OnInspectorGUI()
        {
            _shaderLinear.overrideState.boolValue = true;
            _shaderExponential.overrideState.boolValue = true;
            _shaderExpSquared.overrideState.boolValue = true;

            if (_shaderLinear.value.objectReferenceValue == null)
                _shaderLinear.value.objectReferenceValue = Shader.Find("Hidden/UnityUtility/PostProcessing/LinearFog");

            if (_shaderExponential.value.objectReferenceValue == null)
                _shaderExponential.value.objectReferenceValue = Shader.Find("Hidden/UnityUtility/PostProcessing/ExpFog");           

            if (_shaderExpSquared.value.objectReferenceValue == null)
                _shaderExpSquared.value.objectReferenceValue = Shader.Find("Hidden/UnityUtility/PostProcessing/ESFog");

            PropertyField(_mode);
            PropertyField(_color);
            int mode = _mode.value.intValue;

            if (mode == (int)FogMode.Linear)
            {
                PropertyField(_aParam, EditorGuiUtility.TempContent("Start"));
                PropertyField(_bParam, EditorGuiUtility.TempContent("End"));
                _bParam.value.floatValue = _bParam.value.floatValue.ClampMin(_aParam.value.floatValue);
            }
            else
            {
                PropertyField(_aParam, EditorGuiUtility.TempContent("Density"));
                _aParam.value.floatValue = _aParam.value.floatValue.Clamp01();

                PropertyField(_bParam, EditorGuiUtility.TempContent("Offset"));
                _bParam.value.floatValue = _bParam.value.floatValue.ClampMin(0f);
            }
        }
    }
}
#endif
