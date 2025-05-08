Shader "Flooded_Grounds/PBR_Water"
{
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_Emis("Self-Ilumination", Range(0,1)) = 0.1
		_Smth("Smoothness", Range(0,1)) = 0.9
		_Parallax ("Height", Range (0.005, 0.08)) = 0.02
		_MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
		_BumpMap ("Normalmap", 2D) = "bump" {}
		_BumpMap2 ("Normalmap2", 2D) = "bump" {}
		_BumpLerp("Normalmap2 Blend", Range(0,1)) = 0.5
		_ParallaxMap ("Heightmap", 2D) = "black" {}
		_ScrollSpeed("Scroll Speed", float) = 0.2
		_WaveFreq("Wave Frequency", float) = 20
		_WaveHeight("Wave Height", float) = 0.1
	}

	SubShader {
		Tags { "RenderType"="Opaque" "Queue"="Geometry" "RenderPipeline" = "UniversalPipeline" }
		LOD 200

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
			float2 uv_BumpMap : TEXCOORD4;
			float2 uv_BumpMap2 : TEXCOORD5;
			float2 uv_ParallaxMap : TEXCOORD6;
		};

		TEXTURE2D(_MainTex);
		TEXTURE2D(_BumpMap);
		TEXTURE2D(_BumpMap2);
		TEXTURE2D(_ParallaxMap);

		SAMPLER(sampler_MainTex);
		SAMPLER(sampler_BumpMap);
		SAMPLER(sampler_BumpMap2);
		SAMPLER(sampler_ParallaxMap);

		CBUFFER_START(UnityPerMaterial)
			float4 _Color;
			float4 _MainTex_ST;
			float4 _BumpMap_ST;
			float4 _BumpMap2_ST;
			float4 _ParallaxMap_ST;
			float _Emis;
			float _Smth;
			float _Parallax;
			float _ScrollSpeed;
			float _WaveFreq;
			float _WaveHeight;
			float _BumpLerp;
		CBUFFER_END

		// Parallax offset function implementation
		float2 ParallaxOffset(float height, float strength, float3 viewDir)
		{
			float2 offset = viewDir.xy * (height * strength);
			return offset;
		}

		Varyings vert(Attributes input) {
			Varyings output;
			
			// Wave animation
			float phase = _Time.y * _WaveFreq;
			float offset = (input.positionOS.x + (input.positionOS.z * 2)) * 8;
			input.positionOS.y += sin(phase + offset) * _WaveHeight;

			output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
			output.positionWS = TransformObjectToWorld(input.positionOS.xyz);
			output.normalWS = TransformObjectToWorldNormal(input.normalOS);
			output.viewDirWS = GetWorldSpaceViewDir(output.positionWS);
			
			output.uv = TRANSFORM_TEX(input.uv, _MainTex);
			output.uv_BumpMap = TRANSFORM_TEX(input.uv, _BumpMap);
			output.uv_BumpMap2 = TRANSFORM_TEX(input.uv, _BumpMap2);
			output.uv_ParallaxMap = TRANSFORM_TEX(input.uv, _ParallaxMap);
			
			return output;
		}

		half4 frag(Varyings input) : SV_Target {
			half scrollX = _ScrollSpeed * _Time.y;
			half scrollY = (_ScrollSpeed * _Time.y) * 0.5;
			
			half scrollX2 = (1 - _ScrollSpeed) * _Time.y;
			half scrollY2 = (1 - _ScrollSpeed * _Time.y) * 0.5;

			input.uv_ParallaxMap += half2(scrollX * 0.2, scrollY * 0.2);
			
			half h = SAMPLE_TEXTURE2D(_ParallaxMap, sampler_ParallaxMap, input.uv_ParallaxMap).r;
			half2 offset = ParallaxOffset(h, _Parallax, input.viewDirWS);

			input.uv += offset + half2(scrollX, scrollY);
			
			half2 uv1 = input.uv_BumpMap + offset + half2(scrollX, scrollY);
			half2 uv2 = input.uv_BumpMap + offset + half2(scrollX2, scrollY2);

			half3 nrml = UnpackNormal(SAMPLE_TEXTURE2D(_BumpMap, sampler_BumpMap, uv1));
			half3 nrml2 = UnpackNormal(SAMPLE_TEXTURE2D(_BumpMap, sampler_BumpMap, uv2));
			half3 nrml3 = UnpackNormal(SAMPLE_TEXTURE2D(_BumpMap2, sampler_BumpMap2, input.uv_BumpMap2));

			half4 tex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
			half3 finalnormal = lerp(nrml.rgb + (nrml2.rgb * half3(1,1,0)), nrml3, _BumpLerp);

			// Setup surface data
			SurfaceData surfaceData = (SurfaceData)0;
			surfaceData.albedo = tex.rgb * _Color.rgb;
			surfaceData.normalTS = finalnormal;
			surfaceData.smoothness = _Smth;
			surfaceData.emission = tex.rgb * _Color.rgb * _Emis;

			// Setup input data
			InputData inputData = (InputData)0;
			inputData.positionWS = input.positionWS;
			inputData.normalWS = TransformTangentToWorld(finalnormal, float3x3(0,0,0,0,0,0,0,0,0)); // Simplified tangent space
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
