// Shader를 Universal Render Pipeline/Fullscreen/Distortion으로 정의
Shader "Fullscreen/UnderwaterDistortion"
{
    Properties
    {
        // 쉐이더 외부에서 설정할 속성들
        [Header(Distortion Settings)]
        _NoiseTex ("Noise Texture (RG)", 2D) = "white" {} // 왜곡에 사용할 노이즈 텍스처
        _DistortionStrength ("Strength", Range(0.0, 0.1)) = 0.01 // 왜곡 강도
        _ScrollSpeed ("Scroll Speed (XY)", Vector) = (0.1, 0.05, 0, 0) // 노이즈 스크롤 속도
    }

    SubShader
    {
        // URP 렌더링에 필요한 태그 설정
        Tags { "RenderPipeline" = "UniversalPipeline" "RenderType" = "Opaque" }
        LOD 100

        Pass
        {
            Name "UnderwaterDistortionPass"
            
            // 포스트 프로세싱을 위한 설정
            ZTest Always ZWrite Off Cull Off

            HLSLPROGRAM
            // 사용할 URP 라이브러리 및 헬퍼 함수 임포트
            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            // ------------------------------------
            // 쉐이더 속성 변수 선언
            // ------------------------------------
            sampler2D _MainTex; // 원본 화면 텍스처 (Unity 기본 제공)
            sampler2D _NoiseTex; // Noise Texture
            float4 _NoiseTex_ST; // Noise Texture의 Tiling/Offset 정보 (C# 스크립트에서 자동 전달)
            float _DistortionStrength; // 왜곡 강도
            float2 _ScrollSpeed; // 스크롤 속도

            // ------------------------------------
            // Vertex Shader 입력/출력 구조체
            // ------------------------------------
            struct Attributes
            {
                float4 positionOS : POSITION; // 오브젝트 공간 위치
                float2 uv         : TEXCOORD0; // UV 좌표
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION; // 클립 공간 위치
                float2 uv         : TEXCOORD0; // UV 좌표 (원본 화면 샘플링용)
            };

            // ------------------------------------
            // Vertex Shader
            // ------------------------------------
            Varyings Vert(Attributes input)
            {
                Varyings output;
                // Fullscreen Pass에 맞게 position 및 UV 설정 (일반적으로 사각형 쿼드)
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.uv; // 이 uv가 화면 픽셀의 UV 좌표가 됩니다.
                return output;
            }

            // ------------------------------------
            // Fragment Shader
            // ------------------------------------
            half4 Frag(Varyings input) : SV_Target
            {
                // 1. 노이즈 UV 좌표 계산 (시간에 따른 스크롤)
                float2 noiseUV = TRANSFORM_TEX(input.uv, _NoiseTex);
                noiseUV += _ScrollSpeed * _Time.y;

                // 2. 노이즈 샘플링 (R, G 채널을 X, Y 왜곡 방향으로 사용)
                // float4 noiseSample = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, noiseUV);
                // URP/HLSL에서는 간단하게 Texture2D.Sample()을 사용합니다.
                float4 noiseSample = tex2D(_NoiseTex, noiseUV);

                // 3. 왜곡 벡터 계산
                // RG 채널을 2D 벡터로 사용하고 강도를 곱합니다.
                // 노이즈 값은 0~1 사이이므로, -0.5를 빼서 -0.5 ~ 0.5 범위로 만들어 왜곡이 중앙에서 퍼지게 합니다.
                float2 distortion = (noiseSample.rg - 0.5) * _DistortionStrength;

                // 4. 새로운 UV 좌표 계산 (화면 왜곡)
                float2 newUV = input.uv + distortion;

                // 5. 원본 화면 텍스처에서 새로운 UV로 색상 샘플링
                float4 finalColor = tex2D(_MainTex, newUV);

                return finalColor;
            }
            ENDHLSL
        }
    }
}