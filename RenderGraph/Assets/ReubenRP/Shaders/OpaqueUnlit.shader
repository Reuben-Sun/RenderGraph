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
        	Tags{"LightMode" = "GBufferPass"}
        	
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

            struct GBuffer
            {
	            float4 MRT0 : SV_Target0;		//RGB: Albedo, A: shadow
				float4 MRT1 : SV_Target1;		//RGB: Emission + fog.xyz
				float4 MRT2 : SV_Target2;		//RGB: EncodeNormal.xyz, A: ShadingMode
				float4 MRT3 : SV_Target3;		//R: Metallic, G: Roughness, B:Occlusion, A: fog.w
            };
			CBUFFER_START(UnityPerMaterial)

			CBUFFER_END

			GBuffer frag (v2f i) : SV_Target
			{
				GBuffer gbuffer;
				gbuffer.MRT0 = half4(1,0,0, 1);
				gbuffer.MRT1 = half4(0.2, 0.2, 0.2, 1);
				gbuffer.MRT2 = half4(0.1, 0.1, 0.1, 1);
				gbuffer.MRT3 = half4(0.1, 0.1, 0.1, 1);
				return gbuffer;
			}
			ENDHLSL
        }
    }
}
