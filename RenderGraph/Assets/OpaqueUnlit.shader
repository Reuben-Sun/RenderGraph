Shader "RenderGraph/OpaqueUnlit"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
        	Tags{"LightMode" = "BasePass"}
        	
            HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = TransformObjectToHClip(v.vertex.xyz);
				o.uv = v.uv;
				return o;
			}

            struct MRT
            {
	            float4 Albedo : SV_Target0;
				float4 Emission : SV_Target1;
            };
			CBUFFER_START(UnityPerMaterial)

			CBUFFER_END

			MRT frag (v2f i) : SV_Target
			{
				MRT mrt;
				mrt.Albedo = half4(0.6, 0, 0.1, 1);
				mrt.Emission = half4(0.2, 0.2, 0.2, 1);
				return mrt;
			}
			ENDHLSL
        }
    }
}
