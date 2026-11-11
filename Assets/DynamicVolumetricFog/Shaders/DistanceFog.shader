
Shader "Custom/DynamicFog/DistanceFog"
{
    Properties
    {
        _FogColor ("Fog Color", Color) = (0.35, 0.40, 0.48, 1)
        _FogStart ("Fog Start", Float) = 3
        _FogEnd ("Fog End", Float) = 25
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
        }

        Pass
        {
            Name "DistanceFog"

            ZWrite Off
            ZTest Always
            Cull Off

            HLSLPROGRAM

            #pragma vertex Vert
            #pragma fragment Frag

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

#include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _FogColor;
                float _FogStart;
                float _FogEnd;
            CBUFFER_END

            float4 Frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                float4 color = SAMPLE_TEXTURE2D_X(
                    _BlitTexture,
                    sampler_LinearClamp,
                    input.texcoord
                );

                float rawDepth = SampleSceneDepth(input.texcoord);

                float eyeDepth = LinearEyeDepth(
                    rawDepth,
                    _ZBufferParams
                );

                float fogFactor = saturate(
                    (eyeDepth - _FogStart) /
                    max(_FogEnd - _FogStart, 0.001)
                );


                color.rgb = lerp(
                    color.rgb,
                    _FogColor.rgb,
                    fogFactor
                );

                return color;
            }

            ENDHLSL
        }
    }
}
