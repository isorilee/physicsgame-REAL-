Shader "Custom/URP Fake Liquid"
{
    Properties
    {
        _BaseColor ("Liquid Color", Color) = (0.6, 0.1, 1, 0.45)
        _FoamColor ("Foam Color", Color) = (1, 1, 1, 0.6)
        _NormalMap ("Normal Map", 2D) = "bump" {}
        _WaveSpeed ("Wave Speed", Float) = 0.4
        _WaveStrength ("Wave Strength", Float) = 0.05
        _FresnelPower ("Fresnel Power", Float) = 2.0
        _Alpha ("Transparency", Range(0,1)) = 0.45
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="Transparent"
            "Queue"="Transparent"
        }

        Pass
        {
            Name "FakeLiquid"
            Tags { "LightMode"="UniversalForward" }

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Back

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 normalWS : TEXCOORD0;
                float3 viewDirWS : TEXCOORD1;
                float2 uv : TEXCOORD2;
                float3 positionWS : TEXCOORD3;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float4 _FoamColor;
                float4 _NormalMap_ST;
                float _WaveSpeed;
                float _WaveStrength;
                float _FresnelPower;
                float _Alpha;
            CBUFFER_END

            TEXTURE2D(_NormalMap);
            SAMPLER(sampler_NormalMap);

            Varyings vert(Attributes IN)
            {
                Varyings OUT;

                float3 posOS = IN.positionOS.xyz;

                float wave =
                    sin((posOS.x + _Time.y * _WaveSpeed) * 8.0) *
                    cos((posOS.z + _Time.y * _WaveSpeed) * 8.0);

                posOS.y += wave * _WaveStrength;

                OUT.positionWS = TransformObjectToWorld(posOS);
                OUT.positionHCS = TransformWorldToHClip(OUT.positionWS);
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                OUT.viewDirWS = GetWorldSpaceViewDir(OUT.positionWS);
                OUT.uv = TRANSFORM_TEX(IN.uv, _NormalMap);

                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float3 normalWS = normalize(IN.normalWS);
                float3 viewDir = normalize(IN.viewDirWS);

                float fresnel = pow(1.0 - saturate(dot(normalWS, viewDir)), _FresnelPower);

                float foamLine = smoothstep(0.45, 0.5, IN.uv.y) * smoothstep(0.6, 0.5, IN.uv.y);

                float4 col = _BaseColor;
                col.rgb += fresnel * 0.35;
                col.rgb = lerp(col.rgb, _FoamColor.rgb, foamLine * 0.5);
                col.a = _Alpha + fresnel * 0.2;

                return col;
            }
            ENDHLSL
        }
    }
}