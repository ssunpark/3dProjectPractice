Shader "Flooded_Grounds/PBR_TopBlend"
{
    Properties {
        _MainTex ("Base Albedo (RGB)", 2D) = "white" {}
        _Spc("Base Metalness(R) Smoothness(A)", 2D) = "black" {}
        _BumpMap ("Base Normal", 2D) = "bump" {} 
        _AO("Base AO", 2D)= "white" {}
        _layer1Tex ("Layer1 Albedo (RGB) Smoothness (A)", 2D) = "white" {}
        _layer1Metal ("Layer1 Metalness", Range(0,1)) = 0
        _layer1Norm("Layer 1 Normal", 2D) = "bump" {}
        _layer1Breakup ("Layer1 Breakup (R)", 2D) = "white" {}
        _layer1BreakupAmnt ("Layer1 Breakup Amount", Range(0,1)) = 0.5
        _layer1Tiling("Layer1 Tiling", float) = 10
        _Power ("Layer1 Blend Amount", float ) = 1
        _Shift("Layer1 Blend Height", float) = 1           
        _DetailBump ("Detail Normal", 2D) = "bump" {}  
        _DetailInt ("DetailNormal Intensity", Range(0,1)) = 0.4
        _DetailTiling("DetailNormal Tiling", float) = 2  
    }

    SubShader {
        Tags { "RenderType"="Opaque" "Queue" = "Geometry" "RenderPipeline" = "UniversalPipeline" }
        LOD 500

        Pass {
            Name "ForwardLit"
            Tags {"LightMode" = "UniversalForward"}

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                float2 uv : TEXCOORD0;
            };

            struct Varyings {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float3 positionWS : TEXCOORD2;
                float3 viewDirWS : TEXCOORD3;
            };

            TEXTURE2D(_MainTex);
            TEXTURE2D(_Spc);
            TEXTURE2D(_BumpMap);
            TEXTURE2D(_AO);
            TEXTURE2D(_DetailBump);
            TEXTURE2D(_layer1Tex);
            TEXTURE2D(_layer1Norm);
            TEXTURE2D(_layer1Breakup);

            SAMPLER(sampler_MainTex);
            SAMPLER(sampler_Spc);
            SAMPLER(sampler_BumpMap);
            SAMPLER(sampler_AO);
            SAMPLER(sampler_DetailBump);
            SAMPLER(sampler_layer1Tex);
            SAMPLER(sampler_layer1Norm);
            SAMPLER(sampler_layer1Breakup);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _BumpMap_ST;
                float4 _layer1Tex_ST;
                float4 _layer1Norm_ST;
                float4 _layer1Breakup_ST;
                float4 _DetailBump_ST;
                float _layer1Metal;
                float _layer1BreakupAmnt;
                float _layer1Tiling;
                float _Power;
                float _Shift;
                float _DetailInt;
                float _DetailTiling;
            CBUFFER_END

            Varyings vert(Attributes input) {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.positionWS = TransformObjectToWorld(input.positionOS.xyz);
                output.normalWS = TransformObjectToWorldNormal(input.normalOS);
                output.viewDirWS = GetWorldSpaceViewDir(output.positionWS);
                output.uv = input.uv;
                return output;
            }

            half4 frag(Varyings input) : SV_Target {
                half3 layer1direction = half3(0,1,0);

                // Texture Inputs
                half3 main = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv).rgb;
                half3 norm = UnpackNormal(SAMPLE_TEXTURE2D(_BumpMap, sampler_BumpMap, input.uv));
                half4 spec = SAMPLE_TEXTURE2D(_Spc, sampler_Spc, input.uv);
                half3 ao = SAMPLE_TEXTURE2D(_AO, sampler_AO, input.uv).rgb;
                half4 layer1 = SAMPLE_TEXTURE2D(_layer1Tex, sampler_layer1Tex, input.uv * _layer1Tiling);
                half3 layer1norm = UnpackNormal(SAMPLE_TEXTURE2D(_layer1Norm, sampler_layer1Norm, input.uv * _layer1Tiling));
                half layer1Breakup = SAMPLE_TEXTURE2D(_layer1Breakup, sampler_layer1Breakup, input.uv * _layer1Tiling).r;
                half3 detnorm = UnpackNormal(SAMPLE_TEXTURE2D(_DetailBump, sampler_DetailBump, input.uv * _DetailTiling));

                half3 modNormal = norm + half3(layer1norm.r * 0.6, layer1norm.g * 0.6, 0);

                // Prepare Blend Masks
                half blend = dot(input.normalWS, layer1direction);
                half blend2 = (blend * _Power + _Shift) * lerp(1, layer1Breakup, _layer1BreakupAmnt);
                blend2 = saturate(pow(blend2, 3));

                // Combine Normals
                half3 blendedNormal = lerp(norm, layer1norm, blend2);
                blendedNormal = blendedNormal + (detnorm * half3(_DetailInt,_DetailInt,0));

                // Combine Diffuse layers
                half3 blendedColor = lerp(main, layer1.rgb, blend2);

                // Setup surface data
                SurfaceData surfaceData = (SurfaceData)0;
                surfaceData.albedo = blendedColor;
                surfaceData.normalTS = blendedNormal;
                surfaceData.occlusion = ao;
                surfaceData.smoothness = lerp(spec.a, layer1.a, blend2);
                surfaceData.metallic = lerp(spec.r, _layer1Metal, blend2);

                // Setup input data
                InputData inputData = (InputData)0;
                inputData.positionWS = input.positionWS;
                inputData.normalWS = TransformTangentToWorld(blendedNormal, float3x3(0,0,0,0,0,0,0,0,0)); // Simplified tangent space
                inputData.viewDirectionWS = normalize(input.viewDirWS);
                inputData.shadowCoord = TransformWorldToShadowCoord(input.positionWS);

                // Calculate lighting
                half4 finalColor = UniversalFragmentPBR(inputData, surfaceData);
                return finalColor;
            }
            ENDHLSL
        }
    }
    FallBack "Universal Render Pipeline/Lit"
}