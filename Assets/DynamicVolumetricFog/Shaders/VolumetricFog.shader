
Shader "Custom/VolumetricFog"
{
    Properties
    {
        _FogColor ("Fog Color", Color) = (0.35, 0.38, 0.42, 1)
        _FogAbsorption ("Fog Absorption", Range(0.01, 2)) = 0.12
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType" = "Opaque"
        }

        Pass
        {
            ZWrite Off
            ZTest Always
            Cull Off

            HLSLPROGRAM

            #pragma target 4.5
            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

            TEXTURE3D(_FogDensityTexture);
            SAMPLER(sampler_FogDensityTexture);

            float4x4 _FogWorldToLocal;
            float3 _FogVolumeCenter;
            float3 _FogVolumeSize;

            float4 _FogColor;
            float _FogAbsorption;

            float4 Frag(Varyings input) : SV_Target
            {
                float2 uv = input.texcoord;

                // Couleur originale de l'image.
                float4 sceneColor = SAMPLE_TEXTURE2D_X(
                    _BlitTexture,
                    sampler_LinearClamp,
                    uv
                );

                // Profondeur de la scène.
                float rawDepth = SampleSceneDepth(uv);

                #if !UNITY_REVERSED_Z
                    rawDepth = lerp(
                        UNITY_NEAR_CLIP_VALUE,
                        1.0,
                        rawDepth
                    );
                #endif

                // Position du point visible dans le monde.
                float3 worldEnd = ComputeWorldSpacePosition(
                    uv,
                    rawDepth,
                    UNITY_MATRIX_I_VP
                );

                float3 worldStart = GetCameraPositionWS();

                // Conversion des positions dans l'espace local du volume.
                float3 localStart = mul(
                    _FogWorldToLocal,
                    float4(worldStart, 1.0)
                ).xyz;

                float3 localEnd = mul(
                    _FogWorldToLocal,
                    float4(worldEnd, 1.0)
                ).xyz;

                float3 localDelta = localEnd - localStart;

                // Limites locales du volume.
                float3 boxMin = _FogVolumeCenter - _FogVolumeSize * 0.5;
                float3 boxMax = _FogVolumeCenter + _FogVolumeSize * 0.5;

                // Évite une division par zéro lorsque le rayon est parallèle à une face.
                float3 safeDelta = float3(
                    abs(localDelta.x) < 0.00001 ? 0.00001 : localDelta.x,
                    abs(localDelta.y) < 0.00001 ? 0.00001 : localDelta.y,
                    abs(localDelta.z) < 0.00001 ? 0.00001 : localDelta.z
                );

                float3 t1 = (boxMin - localStart) / safeDelta;
                float3 t2 = (boxMax - localStart) / safeDelta;

                float3 tMin = min(t1, t2);
                float3 tMax = max(t1, t2);

                float tEnter = max(
                    max(tMin.x, tMin.y),
                    max(tMin.z, 0.0)
                );

                float tExit = min(
                    min(tMax.x, tMax.y),
                    min(tMax.z, 1.0)
                );

                // Le rayon ne traverse pas le volume.
                if (tExit <= tEnter)
                    return sceneColor;

                // Parcours du volume et accumulation du brouillard.
                const int STEP_COUNT = 64;

                float worldDistance = distance(worldStart, worldEnd);
                float stepLength =
                    worldDistance * (tExit - tEnter) / STEP_COUNT;

                float transmittance = 1.0;

                for (int i = 0; i < STEP_COUNT; i++)
                {
                    float t = tEnter +
                        (i + 0.5) * (tExit - tEnter) / STEP_COUNT;

                    float3 localPosition = localStart + localDelta * t;

                    float3 fogUV =
                        (localPosition - _FogVolumeCenter) /
                        _FogVolumeSize + 0.5;

                    float density = SAMPLE_TEXTURE3D_LOD(
                        _FogDensityTexture,
                        sampler_FogDensityTexture,
                        fogUV,
                        0
                    ).r;

                    transmittance *= exp(
                        -density * _FogAbsorption * stepLength
                    );
                }

                float fogAmount = 1.0 - transmittance;

                float3 finalColor = lerp(
                    sceneColor.rgb,
                    _FogColor.rgb,
                    fogAmount
                );

                return float4(finalColor, sceneColor.a);
            }

            ENDHLSL
        }
    }
}
