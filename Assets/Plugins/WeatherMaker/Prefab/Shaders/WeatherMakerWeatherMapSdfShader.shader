//
// Weather Maker for Unity
// (c) 2016 Digital Ruby, LLC
// Source code may be used for personal or commercial projects.
// Source code may NOT be redistributed or sold.
// 
// *** A NOTE ABOUT PIRACY ***
// 
// If you got this asset from a pirate site, please consider buying it from the Unity asset store at https://www.assetstore.unity3d.com/en/#!/content/60955?aid=1011lGnL. This asset is only legally available from the Unity Asset Store.
// 
// I'm a single indie dev supporting my family by spending hundreds and thousands of hours on this and other assets. It's very offensive, rude and just plain evil to steal when I (and many others) put so much hard work into the software.
// 
// Thank you.
//
// *** END NOTE ABOUT PIRACY ***
//

Shader "WeatherMaker/WeatherMakerWeatherMapSdfShader"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags { }
		LOD 100
		Blend One Zero
		Fog { Mode Off }
		ZWrite On
		ZTest Always

		CGINCLUDE

		#include "WeatherMakerCoreShaderInclude.cginc"

		#pragma target 3.5
		#pragma exclude_renderers gles
		#pragma exclude_renderers d3d9

		uniform UNITY_DECLARE_TEX2D(_MainTex);
		uniform UNITY_DECLARE_TEX2D(_PrevSdfTex);
		uniform UNITY_DECLARE_TEX2D(_CurSdfTex);

		struct appdata
		{
			float4 vertex : POSITION;
			float2 uv : TEXCOORD0;
		};

		struct v2f
		{
			float2 uv : TEXCOORD0;
			float4 vertex : SV_POSITION;
		};

		v2f vert(appdata v)
		{
			v2f o;
			o.vertex = UnityObjectToClipPos(v.vertex);
			o.uv = v.uv;
			return o;
		}

		float4 _CurSdfTex_TexelSize;
		float4 _PrevSdfTex_TexelSize;

		uniform float4 _SdfPixelSize;
		
		static const float3 offsetsCur[8] =
		{
			float3(-_CurSdfTex_TexelSize.x, 0.0, 0.0),
			float3(_CurSdfTex_TexelSize.x, 0.0, 0.0),
			float3(0.0, -_CurSdfTex_TexelSize.y, 0.0),
			float3(0.0, _CurSdfTex_TexelSize.y, 0.0),
			float3(-_CurSdfTex_TexelSize.x, -_CurSdfTex_TexelSize.y, 1.0),
			float3(_CurSdfTex_TexelSize.x, -_CurSdfTex_TexelSize.y, 1.0),
			float3(-_CurSdfTex_TexelSize.x, _CurSdfTex_TexelSize.y, 1.0),
			float3(_CurSdfTex_TexelSize.x, _CurSdfTex_TexelSize.y, 1.0)
		};

		ENDCG

		// sdf calculator
		Pass
		{
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			fixed4 frag (v2f input) : SV_Target
			{
				// prev sdf tex has alpha that is the sdf value
				fixed4 prevSdfCol = UNITY_SAMPLE_TEX2D_SAMPLER_LOD(_PrevSdfTex, _point_clamp_sampler, float4(input.uv, 0.0, 0.0));
				fixed4 currentSdfCol = UNITY_SAMPLE_TEX2D_SAMPLER_LOD(_CurSdfTex, _point_clamp_sampler, float4(input.uv, 0.0, 0.0));

				// current sdf tex has an alpha that needs to be computed based off of the red channel of the texture
				UNITY_LOOP
				for (uint i = 0; i < 8; i++)
				{
					float3 offsets = offsetsCur[i];
					float4 coord = float4(input.uv + offsets.xy, 0.0, 0.0);
					fixed4 col = UNITY_SAMPLE_TEX2D_SAMPLER_LOD(_CurSdfTex, _point_clamp_sampler, coord);

					// if we have an opaque pixel, use it and we are done - we check the horizontal and vertical pixels first
					// and then check the diagonals, so we can break out upon first found pixel
					UNITY_BRANCH
					if (col.r > _SdfPixelSize.z)
					{
						// currentSdfCol.a = min(_SdfPixelSize[offsets.z], currentSdfCol.a);
						currentSdfCol.a = min(_SdfPixelSize.x, currentSdfCol.a);
						break;
					}
				}

				currentSdfCol.a = min(prevSdfCol.a, currentSdfCol.a);

				return currentSdfCol;
			}

			ENDCG
		}

		// point sample blit
		Pass
		{
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			fixed4 frag (v2f input) : SV_Target
			{
				return UNITY_SAMPLE_TEX2D_SAMPLER_LOD(_MainTex, _point_clamp_sampler, float4(input.uv, 0.0, 0.0));
			}

			ENDCG
		}
	}

	Fallback Off
}
