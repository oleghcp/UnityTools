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
        public FogModeParameter Mode = new FogModeParameter() { value = FogMode.ExponentialSquared };

        public ColorParameter FogColor = new ColorParameter() { value = Colours.White };
        public FloatParameter Param1 = new FloatParameter(); //fog density or linear start
        public FloatParameter Param2 = new FloatParameter(); //fog offset or linear end

        public ShaderParameter ShaderLinear = new ShaderParameter();
        public ShaderParameter ShaderExponential = new ShaderParameter();
        public ShaderParameter ShaderExpSquared = new ShaderParameter();

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
                switch (settings.Mode.value)
                {
                    case FogMode.Linear:
                        RenderTarget(context, settings.ShaderLinear, START_PROP, END_PROP);
                        break;

                    case FogMode.Exponential:
                        RenderTarget(context, settings.ShaderExponential, DENSITY_PROP, OFFSET_PROP);
                        break;

                    case FogMode.ExponentialSquared:
                        RenderTarget(context, settings.ShaderExpSquared, DENSITY_PROP, OFFSET_PROP);
                        break;

                    default:
                        throw new UnsupportedValueException(settings.Mode.value);
                }
            }

            public override DepthTextureMode GetCameraFlags()
            {
                return DepthTextureMode.Depth;
            }

            private void RenderTarget(PostProcessRenderContext context, Shader shader, string param1, string param2)
            {
                PropertySheet sheet = context.propertySheets.Get(shader);
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
