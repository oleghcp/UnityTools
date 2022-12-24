Shader "Hidden/OlegHCP/ExpFog"
{
    HLSLINCLUDE
            
        #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"
        
        TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
        TEXTURE2D_SAMPLER2D(_CameraDepthTexture, sampler_CameraDepthTexture);
            
        float4 _FogColor;
        float _FogDensity;
        float _FogOffset;

        float4 Frag(VaryingsDefault i) : SV_Target
        {            
            float4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoordStereo);
            float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, i.texcoordStereo);
            float viewDistance = Linear01Depth(depth) * _ProjectionParams.z;
            float fogFactor = _FogDensity * max(0.0f, viewDistance - _FogOffset);
            float4 fogOutput = lerp(_FogColor, col, saturate(exp2(-fogFactor)));

            return fogOutput;
        }

    ENDHLSL

    SubShader
    {
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            HLSLPROGRAM

                #pragma vertex VertDefault
                #pragma fragment Frag

            ENDHLSL
        }        
    }
}
