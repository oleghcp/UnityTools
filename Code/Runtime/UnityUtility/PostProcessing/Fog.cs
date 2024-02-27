#if INCLUDE_POST_PROCESSING
using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Scripting;

namespace OlegHcp.PostProcessing
{
    [Serializable]
    [PostProcess(typeof(FogRenderer), PostProcessEvent.BeforeTransparent, "OlegHcp/Fog")]
    public class Fog : PostProcessEffectSettings
    {
        public ShaderParameter Shader = new ShaderParameter() { overrideState = true };
        public FogModeParameter Mode = new FogModeParameter() { value = FogMode.ExponentialSquared };
        public ColorParameter FogColor = new ColorParameter() { value = Colours.White };
        public FloatParameter Param1 = new FloatParameter(); //fog density or linear start
        public FloatParameter Param2 = new FloatParameter(); //fog offset or linear end

        public static string GetFogShaderPath(FogMode mode)
        {
            switch (mode)
            {
                case FogMode.Linear: return "Hidden/UnityUtility/PostProcessing/LinearFog";
                case FogMode.Exponential: return "Hidden/UnityUtility/PostProcessing/ExpFog";
                case FogMode.ExponentialSquared: return "Hidden/UnityUtility/PostProcessing/ESFog";
                default: throw new UnsupportedValueException(mode);
            }
        }

        [Serializable]
        public sealed class FogModeParameter : ParameterOverride<FogMode> { }

        [Serializable]
        public sealed class ShaderParameter : ParameterOverride<Shader> { }

        [Preserve]
        public class FogRenderer : PostProcessEffectRenderer<Fog>
        {
            private int _colorId;
            private int _densityId;
            private int _offsetId;
            private int _startId;
            private int _endId;

            public override void Init()
            {
                _colorId = UnityEngine.Shader.PropertyToID("_FogColor");
                _densityId = UnityEngine.Shader.PropertyToID("_FogDensity");
                _offsetId = UnityEngine.Shader.PropertyToID("_FogOffset");
                _startId = UnityEngine.Shader.PropertyToID("_Start");
                _endId = UnityEngine.Shader.PropertyToID("_End");
            }

            public override void Render(PostProcessRenderContext context)
            {
#if UNITY_EDITOR
                if (settings.Shader.value == null)
                    return;
#endif
                PropertySheet sheet = context.propertySheets.Get(settings.Shader);
                MaterialPropertyBlock properties = sheet.properties;

                properties.SetVector(_colorId, settings.FogColor);

                switch (settings.Mode.value)
                {
                    case FogMode.Linear:
                        properties.SetFloat(_startId, settings.Param1);
                        properties.SetFloat(_endId, settings.Param2);
                        break;

                    case FogMode.Exponential:
                    case FogMode.ExponentialSquared:
                        properties.SetFloat(_densityId, settings.Param1);
                        properties.SetFloat(_offsetId, settings.Param2);
                        break;

                    default:
                        throw new UnsupportedValueException(settings.Mode.value);
                }

                context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0, preserveDepth: true);
            }

            public override DepthTextureMode GetCameraFlags()
            {
                return DepthTextureMode.Depth;
            }
        }
    }
}
#endif
