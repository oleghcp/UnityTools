#if INCLUDE_POST_PROCESSING
using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Scripting;

namespace UnityUtility.PostProcessing
{
    [Serializable]
    [PostProcess(typeof(FogRenderer), PostProcessEvent.BeforeTransparent, "UnityUtility/Fog")]
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
            private const string COLOR_PROP = "_FogColor";
            private const string DENSITY_PROP = "_FogDensity";
            private const string OFFSET_PROP = "_FogOffset";
            private const string START_PROP = "_Start";
            private const string END_PROP = "_End";

            public override void Render(PostProcessRenderContext context)
            {
#if UNITY_EDITOR
                if (settings.Shader.value == null)
                    return;
#endif

                switch (settings.Mode.value)
                {
                    case FogMode.Linear:
                        RenderTarget(context, START_PROP, END_PROP);
                        break;

                    case FogMode.Exponential:
                    case FogMode.ExponentialSquared:
                        RenderTarget(context, DENSITY_PROP, OFFSET_PROP);
                        break;

                    default:
                        throw new UnsupportedValueException(settings.Mode.value);
                }
            }

            public override DepthTextureMode GetCameraFlags()
            {
                return DepthTextureMode.Depth;
            }

            private void RenderTarget(PostProcessRenderContext context, string param1, string param2)
            {
                PropertySheet sheet = context.propertySheets.Get(settings.Shader);
                MaterialPropertyBlock properties = sheet.properties;

                properties.SetVector(COLOR_PROP, settings.FogColor);
                properties.SetFloat(param1, settings.Param1);
                properties.SetFloat(param2, settings.Param2);

                context.command.BlitFullscreenTriangle(context.source,
                                                       context.destination,
                                                       sheet,
                                                       0,
                                                       preserveDepth: true);
            }
        }
    }
}
#endif
