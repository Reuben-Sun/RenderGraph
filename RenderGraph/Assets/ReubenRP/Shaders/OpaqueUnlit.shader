Shader "RenderGraph/OpaqueLit"
{
    Properties
    {
        _MainTex ("Albedo", 2D) = "white" {}
    	_BaseColor("BaseColor", Color) = (1, 1, 1, 1)
    	_RoughnessMap("RoughnessMap", 2D) = "white" {}
    	_MetallicMap("MetallicMap", 2D) = "white" {}
    	_NormalMap("NormalMap", 2D) = "white" {}
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
            	float4 tangent : TANGENT;
            	float4 normal : NORMAL;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD;
				float3 normalWS: TEXCOORD1;
				float4 tangentWS: TEXCOORD2;	//A: sign
			};
            
            CBUFFER_START(UnityPerMaterial)
            float4 _MainTex_ST;
            half4 _BaseColor;
            CBUFFER_END

            sampler2D _MainTex;
            sampler2D _RoughnessMap;
            sampler2D _MetallicMap;
            sampler2D _NormalMap;
            
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = TransformObjectToHClip(v.vertex.xyz);
				o.normalWS = TransformObjectToWorldNormal(v.normal.xyz);
				o.tangentWS.xyz = TransformObjectToWorldDir(v.tangent.xyz);
				o.tangentWS.a = real(v.tangent.w) * GetOddNegativeScale();
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

			GBuffer frag (v2f i) : SV_Target
			{
				float3 albedo = tex2D(_MainTex, i.uv).rgb * _BaseColor;
				float3 emission = 0;
				//normal
				float4 normalMap = tex2D(_NormalMap, i.uv);
				float3 normalTS = UnpackNormalScale(normalMap, 1);
				float sgn = i.tangentWS.w;
				float3 bitangent = sgn * cross(i.normalWS.xyz, i.tangentWS.xyz);
				half3x3 tangentToWorld = half3x3(i.tangentWS.xyz, bitangent.xyz, i.normalWS.xyz);   //TBN矩阵
                i.normalWS = TransformTangentToWorld(normalTS, tangentToWorld);
				//pbr
				float metallic = tex2D(_MetallicMap, i.uv).r;
				float roughness = tex2D(_RoughnessMap, i.uv).r;
				//gbuffer
				GBuffer gbuffer;
				gbuffer.MRT0 = half4(albedo, 0);
				gbuffer.MRT1 = half4(emission, 0);
				gbuffer.MRT2 = half4(i.normalWS, 0);
				gbuffer.MRT3 = half4(metallic, roughness, 0, 0);
				return gbuffer;
			}
			ENDHLSL
        }
    }
}
