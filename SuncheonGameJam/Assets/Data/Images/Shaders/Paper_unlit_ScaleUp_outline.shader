Shader "Custom/Paper_SlightScale_Outline"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _NormalMap("Normal Map", 2D) = "bump" {}
        _ScaleAmount("Scale Amount", Range(0,0.5)) = 0.05

        _Outline("Enable Outline", Float) = 0
        _OutlineColor("Outline Color", Color) = (1,1,1,1)
        _OutlineSize("Outline Size", Range(0,0.1)) = 0.001
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" "PreviewType"="Plane"}
        LOD 100

        Pass
        {
            Name "Unlit_Outline"
            Tags { "LightMode"="UniversalForward" }

            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float4 _MainTex_ST;
            float4 _Color;
            float _ScaleAmount;

            float _Outline;
            float4 _OutlineColor;
            float _OutlineSize;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
                float4 color      : COLOR;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv          : TEXCOORD0;
                float4 color       : COLOR;
            };

            Varyings vert(Attributes v)
            {
                Varyings o;

                // 중앙 기준으로 살짝 확대
                float3 scaledPos = v.positionOS.xyz * (1.0 + _ScaleAmount);
                o.positionHCS = TransformObjectToHClip(scaledPos);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color * _Color;
                return o;
            }

            half4 frag(Varyings i) : SV_Target
            {
                half4 texCol = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv) * i.color;

                // ----------------------------
                // Outline 처리
                // ----------------------------
                if (_Outline > 0.5)
                {
                    // 외곽선 오프셋 계산 (간단하게 상하좌우 4방향)
                    float2 offsets[4] = {
                        float2(_OutlineSize,0),
                        float2(-_OutlineSize,0),
                        float2(0,_OutlineSize),
                        float2(0,-_OutlineSize)
                    };

                    half outlineAlpha = 0;
                    for (int k = 0; k < 4; k++)
                    {
                        outlineAlpha += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv + offsets[k]).a;
                    }
                    outlineAlpha = saturate(outlineAlpha);

                    // 외곽선 색상 적용
                    texCol.rgb = lerp(_OutlineColor.rgb, texCol.rgb, texCol.a);
                    texCol.a = max(texCol.a, outlineAlpha * _Outline);
                }

                return texCol;
            }
            ENDHLSL
        }
    }

    Fallback "Hidden/Universal Render Pipeline/Unlit"
}
