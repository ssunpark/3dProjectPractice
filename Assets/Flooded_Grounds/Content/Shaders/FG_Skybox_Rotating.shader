// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Flooded_Grounds/Skybox_Rotating"
{
	Properties {
		_Tint ("Tint Color", Color) = (.5, .5, .5, .5)
		[Gamma] _Exposure ("Exposure", Range(0, 8)) = 1.0
		_Rotation ("Rotation", Range(0, 360)) = 0
		_RotSpeed ("Rotation Speed", Range(0, 360)) = 0
		[NoScaleOffset] _Tex ("Cubemap   (HDR)", Cube) = "grey" {}
	}

	SubShader {
		Tags { "Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox" "RenderPipeline" = "UniversalPipeline" }
		Cull Off ZWrite Off

		Pass {
			Name "ForwardLit"
			Tags {"LightMode" = "UniversalForward"}

			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

			#define PI 3.14159265359

			TEXTURECUBE(_Tex);
			SAMPLER(sampler_Tex);
			half4 _Tex_HDR;
			half4 _Tint;
			half _Exposure;
			float _Rotation, _RotSpeed;

			float4 RotateAroundYInDegrees(float4 vertex, float degrees)
			{
				float alpha = degrees * PI / 180.0;
				float sina, cosa;
				sincos(alpha, sina, cosa);
				float2x2 m = float2x2(cosa, -sina, sina, cosa);
				return float4(mul(m, vertex.xz), vertex.yw).xzyw;
			}

			struct Attributes {
				float4 positionOS : POSITION;
			};

			struct Varyings {
				float4 positionCS : SV_POSITION;
				float3 texcoord : TEXCOORD0;
			};

			Varyings vert(Attributes input)
			{
				Varyings output;
				output.positionCS = TransformObjectToHClip(RotateAroundYInDegrees(input.positionOS, _Rotation + (_Time.y * _RotSpeed)).xyz);
				output.texcoord = input.positionOS.xyz;
				return output;
			}

			half3 DecodeHDR(half4 encoded, half4 decodeInstructions)
			{
				// Take into account texture alpha if decodeInstructions.w is true(the alpha value affects the RGB channels)
				half alpha = decodeInstructions.w * (encoded.a - 1.0) + 1.0;

				// If Linear mode is not supported we can skip exponent part
				#if defined(UNITY_COLORSPACE_GAMMA)
					return (decodeInstructions.x * alpha).rrr;
				#else
					#if defined(UNITY_USE_NATIVE_HDR)
						return encoded.rgb;
					#else
						return (decodeInstructions.x * pow(alpha, decodeInstructions.y)).rrr;
					#endif
				#endif
			}

			half4 frag(Varyings input) : SV_Target
			{
				half4 tex = SAMPLE_TEXTURECUBE(_Tex, sampler_Tex, input.texcoord);
				half3 c = DecodeHDR(tex, _Tex_HDR);
				c = c * _Tint.rgb;
				c *= _Exposure;
				return half4(c, 1);
			}
			ENDHLSL
		}
	}
	FallBack "Universal Render Pipeline/Skybox"
}
