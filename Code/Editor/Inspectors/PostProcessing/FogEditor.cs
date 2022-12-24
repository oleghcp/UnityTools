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

        public override void OnEnable()
        {
            _mode = FindParameterOverride(item => item.Mode);
            _color = FindParameterOverride(item => item.FogColor);
            _aParam = FindParameterOverride(item => item.Param1);
            _bParam = FindParameterOverride(item => item.Param2);
        }

        public override void OnInspectorGUI()
        {
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
