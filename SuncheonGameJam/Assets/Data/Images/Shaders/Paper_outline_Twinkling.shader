Shader "Custom/PaperGlowSprite2D_OutlineOnly"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _Outline ("Enable Outline", Float) = 0
        _OutlineColor ("Outline Color", Color) = (1,1,1,1)
        _OutlineSize ("Outline Size", Float) = 1
        _TwinkleSpeed("Twinkle Speed", Float) = 5
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        Pass
        {
            Name "Unlit_Outline"
            Tags { "LightMode"="SRPDefaultUnlit" }

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

            float _Outline;
            float4 _OutlineColor;
            float _OutlineSize;
            float _TwinkleSpeed;

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
                o.positionHCS = TransformObjectToHClip(v.positionOS.xyz);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color * _Color;
                return o;
            }

            half4 frag(Varyings i) : SV_Target
            {
                half4 texCol = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv) * i.color;

                if (_Outline > 0.5)
                {
                    // 외곽선 영역 계산
                    float2 offsets[4] = {
                        float2(_OutlineSize/50.0, 0),
                        float2(-_OutlineSize/50.0, 0),
                        float2(0, _OutlineSize/50.0),
                        float2(0, -_OutlineSize/50.0)
                    };

                    half outlineAlpha = 0;
                    for (int k = 0; k < 4; k++)
                    {
                        outlineAlpha += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv + offsets[k]).a;
                    }
                    outlineAlpha = saturate(outlineAlpha * (1.0 - texCol.a)); // 내부 픽셀 제외

                    // 반짝임 계산
                    half twinkle = abs(sin(_Time.y * _TwinkleSpeed));

                    // 외곽선 색상만 덮기
                    texCol.rgb = lerp(texCol.rgb, _OutlineColor.rgb, outlineAlpha * twinkle);
                    texCol.a = max(texCol.a, outlineAlpha * twinkle);
                }

                return texCol;
            }
            ENDHLSL
        }
    }

    Fallback "Hidden/Universal Render Pipeline/Unlit"
}
