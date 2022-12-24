#if INCLUDE_POST_PROCESSING
using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Scripting;

namespace UnityUtility.PostProcessing
{
    [Serializable]
    [PostProcess(typeof(FogRenderer), PostProcessEvent.BeforeTransparent, "UnityUtility/Fog")]
    public class VolumeFog : PostProcessEffectSettings
    {
        public FogModeParameter Mode = new FogModeParameter() { value = FogMode.ExponentialSquared };
        public ColorParameter FogColor = new ColorParameter() { value = Colours.White };
        public FloatParameter Param1 = new FloatParameter(); //fog density or linear start
        public FloatParameter Param2 = new FloatParameter(); //fog offset or linear end

        [Preserve]
        public class FogRenderer : PostProcessEffectRenderer<VolumeFog>
        {
            private const string COLOR_PROP = "_FogColor";
            private const string DENSITY_PROP = "_FogDensity";
            private const string OFFSET_PROP = "_FogOffset";
            private const string START_PROP = "_Start";
            private const string END_PROP = "_End";

            private Shader _shader;

            public override void Init()
            {
                switch (settings.Mode.value)
                {
                    case FogMode.Linear:
                        _shader = Shader.Find("Hidden/UnityUtility/PostProcessing/LinearFog");
                        break;

                    case FogMode.Exponential:
                        _shader = Shader.Find("Hidden/UnityUtility/PostProcessing/ExpFog");
                        break;

                    case FogMode.ExponentialSquared:
                        _shader = Shader.Find("Hidden/UnityUtility/PostProcessing/ESFog");
                        break;

                    default:
                        throw new UnsupportedValueException(settings.Mode.value);
                }
            }

            public override void Render(PostProcessRenderContext context)
            {
                PropertySheet sheet = context.propertySheets.Get(_shader);
                MaterialPropertyBlock properties = sheet.properties;

                properties.SetVector(COLOR_PROP, settings.FogColor);

                switch (settings.Mode.value)
                {
                    case FogMode.Linear:
                        properties.SetFloat(START_PROP, settings.Param1);
                        properties.SetFloat(END_PROP, settings.Param2);
                        break;

                    case FogMode.Exponential:
                    case FogMode.ExponentialSquared:
                        properties.SetFloat(DENSITY_PROP, settings.Param1);
                        properties.SetFloat(OFFSET_PROP, settings.Param2);
                        break;

                    default:
                        throw new UnsupportedValueException(settings.Mode.value);
                }

                context.command.BlitFullscreenTriangle(context.source,
                                                       context.destination,
                                                       sheet,
                                                       0,
                                                       preserveDepth: true);
            }

            public override DepthTextureMode GetCameraFlags()
            {
                return DepthTextureMode.Depth;
            }
        }
    }

    [Serializable]
    public sealed class FogModeParameter : ParameterOverride<FogMode> { }
}
#endif
