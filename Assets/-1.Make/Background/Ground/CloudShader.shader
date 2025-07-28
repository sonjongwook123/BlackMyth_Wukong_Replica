// CloudShader.shader
Shader "Custom/CloudShader"
{
    Properties
    {
        _MainTex ("Cloud Color (2D)", 2D) = "white" {}
        _Speed ("Speed", Vector) = (0.5, 0.0, 0.5, 0)
        _CloudDensity ("Cloud Density", Range(0, 20)) = 5.0
        _CloudHeight ("Cloud Height", Range(0, 1)) = 0.5
        _CloudThickness ("Cloud Thickness", Range(0.1, 1)) = 0.2
        _CloudThreshold ("Cloud Threshold", Range(0, 1)) = 0.4
        _LightColor ("Light Color", Color) = (1, 1, 1, 1)
        _AmbientColor ("Ambient Color", Color) = (0.5, 0.5, 0.5, 1)
        _CloudBrightness ("Cloud Brightness", Range(0, 2)) = 1.0
        _TransparentThreshold ("Transparent Threshold", Range(0, 1)) = 0.1
    }
    SubShader
    {
        Tags { "RenderPipeline" = "UniversalRenderPipeline" "RenderType" = "Transparent" "Queue" = "Transparent" }
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };
            
            float snoise(float3 v)
            {
                const float2 C = float2(1.0/6.0, 1.0/3.0);
                const float4 D = float4(0.0, 0.5, 1.0, 2.0);

                float3 i = floor(v + dot(v, C.yyy));
                float3 x0 = v - i + dot(i, C.xxx);
                
                float3 g = step(x0.yzx, x0.xyz);
                float3 l = 1.0 - g;
                float3 i1 = min(g.xyz, l.zxy);
                float3 i2 = max(g.xyz, l.zxy);

                float3 x1 = x0 - i1 + C.xxx;
                float3 x2 = x0 - i2 + C.yyy;
                float3 x3 = x0 - D.yyy;

                i = fmod(i, 289.0);
                float4 p = float4(i.y + D.w, i.x + D.z, i.z + D.y, 0.0);
                p += float4(i1.x, i2.x, 1.0, 0.0);
                
                float4 j = fmod(p, 289.0);
                float4 k = fmod(j, 7.0);
                float4 a = floor((j - k) / 7.0);
                
                float4 h = 0.5 - abs(x0.xyzx - k);
                h = max(h, 0.0);
                float4 m = h * h;
                float4 w = (dot(x0, a.x), dot(x1, a.y), dot(x2, a.z), dot(x3, a.w));
                
                return dot(m, w) * 0.5 + 0.5;
            }

            TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex);
            
            CBUFFER_START(UnityPerMaterial)
                float4 _Speed;
                float _CloudDensity;
                float _CloudHeight;
                float _CloudThickness;
                float _CloudThreshold;
                float4 _LightColor;
                float4 _AmbientColor;
                float _CloudBrightness;
                float _TransparentThreshold;
            CBUFFER_END
            
            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.uv;
                return output;
            }
            
            half4 frag(Varyings input) : SV_Target
            {
                float3 cloudPos = float3(input.uv, _CloudHeight);
                
                // _Time.y 변수를 재정의 없이 바로 사용합니다.
                float3 scrolledNoisePos = cloudPos * 2.0 + _Time.y * _Speed.xyz * 10.0;
                float2 scrolledTexUV = input.uv + _Time.y * _Speed.xz * 0.1;

                float density = snoise(scrolledNoisePos);
                density = step(_CloudThreshold, density);

                float heightFade = saturate(1.0 - abs(cloudPos.z - _CloudHeight) / _CloudThickness);
                density *= heightFade;

                float totalDensity = density * _CloudDensity;
                
                float3 lightDir = _MainLightPosition.xyz;
                float lightIntensity = saturate(dot(normalize(cloudPos), lightDir) * 0.5 + 0.5);

                float3 baseColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, scrolledTexUV).rgb;
                float3 finalColor = baseColor * totalDensity * lightIntensity;
                finalColor = lerp(finalColor, baseColor * totalDensity, 0.5);
                finalColor *= _CloudBrightness;
                
                float cloudAlpha = saturate(totalDensity);
                cloudAlpha = step(_TransparentThreshold, cloudAlpha);

                return half4(finalColor, cloudAlpha);
            }
            ENDHLSL
        }
    }
}