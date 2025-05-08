// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Flooded_Grounds/Triplanar_BumpSpec"
{
    Properties
    {
        _TexScale ("Tex Scale", Range (0.1, 10.0))= 1.0
        _BlendPlateau ("BlendPlateau",     Range (0.0, 1.0)) = 0.2       
        _MainTex ("Base 1 (RGB) Gloss(A)", 2D) = "white" {}
        _BumpMap1 ("NormalMap 1 (_Y_X)", 2D)  = "bump" {}   
        
        _Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
   
    SubShader
    {
        Tags { "Queue"="Geometry" "IgnoreProjector"="True" "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline" }
        LOD 400

        Pass {
            Name "ForwardLit"
            Tags {"LightMode" = "UniversalForward"}

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            TEXTURE2D(_MainTex);
            TEXTURE2D(_BumpMap1);
            SAMPLER(sampler_MainTex);
            SAMPLER(sampler_BumpMap1);

            CBUFFER_START(UnityPerMaterial)
                float _TexScale;
                float _BlendPlateau;
                float4 _MainTex_ST;
                float4 _BumpMap1_ST;
                float _Cutoff;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float4 color : COLOR;
            };

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.positionWS = TransformObjectToWorld(input.positionOS.xyz);
                output.normalWS = TransformObjectToWorldNormal(input.normalOS);
                output.color = input.color;
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                // Determine the blend weights for the 3 planar projections
                half3 blend_weights = abs(input.normalWS.xyz);
                blend_weights = (blend_weights - _BlendPlateau);
                blend_weights = max(blend_weights, 0);
                blend_weights /= (blend_weights.x + blend_weights.y + blend_weights.z).xxx;

                // Compute the UV coords for each of the 3 planar projections
                half2 coord1 = input.positionWS.yz * _TexScale;
                half2 coord2 = input.positionWS.zx * _TexScale;
                half2 coord3 = input.positionWS.xy * _TexScale;

                // Sample color maps for each projection
                half4 col1 = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, coord1);
                half4 col2 = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, coord2);
                half4 col3 = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, coord3);

                // Sample bump maps and generate bump vectors
                half2 bumpVec1 = SAMPLE_TEXTURE2D(_BumpMap1, sampler_BumpMap1, coord1).wy * 2 - 1;
                half2 bumpVec2 = SAMPLE_TEXTURE2D(_BumpMap1, sampler_BumpMap1, coord2).wy * 2 - 1;
                half2 bumpVec3 = SAMPLE_TEXTURE2D(_BumpMap1, sampler_BumpMap1, coord3).wy * 2 - 1;

                half3 bump1 = half3(0, bumpVec1.x, bumpVec1.y);
                half3 bump2 = half3(bumpVec2.y, 0, bumpVec2.x);
                half3 bump3 = half3(bumpVec3.x, bumpVec3.y, 0);

                // Blend the results of the 3 planar projections
                half4 blended_color = col1.xyzw * blend_weights.xxxx +
                                    col2.xyzw * blend_weights.yyyy +
                                    col3.xyzw * blend_weights.zzzz;

                half3 blended_bumpvec = bump1.xyz * blend_weights.xxx +
                                      bump2.xyz * blend_weights.yyy +
                                      bump3.xyz * blend_weights.zzz;

                // Setup surface data
                SurfaceData surfaceData = (SurfaceData)0;
                surfaceData.albedo = blended_color.rgb * input.color.rgb;
                surfaceData.normalTS = normalize(half3(0,0,1) + blended_bumpvec.xyz);
                surfaceData.alpha = blended_color.a;

                // Setup input data
                InputData inputData = (InputData)0;
                inputData.positionWS = input.positionWS;
                inputData.normalWS = TransformTangentToWorld(surfaceData.normalTS, float3x3(0,0,0,0,0,0,0,0,0)); // Simplified tangent space
                inputData.viewDirectionWS = GetWorldSpaceViewDir(input.positionWS);
                inputData.shadowCoord = TransformWorldToShadowCoord(input.positionWS);

                // Calculate lighting
                half4 finalColor = UniversalFragmentPBR(inputData, surfaceData);
                clip(finalColor.a - _Cutoff);
                return finalColor;
            }
            ENDHLSL
        }
    }
    FallBack "Universal Render Pipeline/Lit"
}
 