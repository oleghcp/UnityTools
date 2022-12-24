using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Scripting;
using UnityUtility;

namespace Engine.PostProcessing
{
    [Serializable]
    [PostProcess(typeof(FogRenderer), PostProcessEvent.BeforeTransparent, "Custom/Fog")]
    public class VolumeFog : PostProcessEffectSettings
    {
        public ColorParameter FogColor = new ColorParameter { value = Colours.White };
        [Range(0.0f, 1.0f)]
        public FloatParameter FogDensity = new FloatParameter { value = 0.001f };
        [UnityEngine.Rendering.PostProcessing.Min(0f)]
        public FloatParameter FogOffset = new FloatParameter { value = 0f };

        [Preserve]
        public class FogRenderer : PostProcessEffectRenderer<VolumeFog>
        {
            private const string COLOR_PROP = "_FogColor";
            private const string DENSITY_PROP = "_FogDensity";
            private const string OFFSET_PROP = "_FogOffset";

            private Shader _shader;

            public override void Init()
            {
                _shader = Shader.Find("Hidden/OlegHCP/ESFog");
            }

            public override void Render(PostProcessRenderContext context)
            {
                PropertySheet sheet = context.propertySheets.Get(_shader);

                sheet.properties.SetVector(COLOR_PROP, settings.FogColor);
                sheet.properties.SetFloat(DENSITY_PROP, settings.FogDensity);
                sheet.properties.SetFloat(OFFSET_PROP, settings.FogOffset);

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
}
