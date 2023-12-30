Shader "RenderGraph/OpaqueLit"
{
    Properties
    {
    	_BaseColor("BaseColor", Color) = (1, 1, 1, 1)
    	
    	[Main(Surface, _, off, off)] _group("PBR", float) = 0
    	
    	[SubToggle(Surface, _ALBEDOMAP)] _EnableAlbedoMap("Enable Albedo Map", Float) = 0.0
        [Tex(Surface_ALBEDOMAP)] [ShowIf(_EnableAlbedoMap, Equal, 1)] 
    	_MainTex ("Albedo", 2D) = "white" {}
        
    	[Sub(Surface)]_Roughness("Roughness", Range(0,1.0)) = 1.0
    	[SubToggle(Surface, _ROUGHNESSMAP)] _EnableRoughnessMap("Enable Roughness Map", Float) = 0.0
        [Tex(Surface_ROUGHNESSMAP)] [ShowIf(_EnableRoughnessMap, Equal, 1)] 
    	_RoughnessMap("RoughnessMap", 2D) = "white" {}
    	
    	[Sub(Surface)]_Metallic("Metallic", Range(0,1.0)) = 1.0
    	[SubToggle(Surface, _METALLICMAP)] _EnableMetallicMap("Enable Metallic Map", Float) = 0.0
        [Tex(Surface_METALLICMAP)] [ShowIf(_EnableMetallicMap, Equal, 1)] 
    	_MetallicMap("MetallicMap", 2D) = "white" {}
    	
        [SubToggle(Surface, _NORMALMAP)] _EnableNormalMap("Enable Normal Map", Float) = 0.0
    	[Tex(Surface_NORMALMAP)] [ShowIf(_EnableNormalMap, Equal, 1)] 
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
			
            #pragma shader_feature_local _ALBEDOMAP
            #pragma shader_feature_local _NORMALMAP
            #pragma shader_feature_local _ROUGHNESSMAP
            #pragma shader_feature_local _METALLICMAP
            
            
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
            float _Roughness;
            float _Metallic;
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
				float3 albedo = _BaseColor;
				#if _ALBEDOMAP
				albedo *= tex2D(_MainTex, i.uv).rgb;
				#endif
				
				float3 emission = 0;
				#if _NORMALMAP
				//normal
				float4 normalMap = tex2D(_NormalMap, i.uv);
				float3 normalTS = UnpackNormalScale(normalMap, 1);
				float sgn = i.tangentWS.w;
				float3 bitangent = sgn * cross(i.normalWS.xyz, i.tangentWS.xyz);
				half3x3 tangentToWorld = half3x3(i.tangentWS.xyz, bitangent.xyz, i.normalWS.xyz);   //TBN矩阵
                i.normalWS = TransformTangentToWorld(normalTS, tangentToWorld);
				#endif
				
				//pbr
				float metallic = _Metallic;
				#if _METALLICMAP
				metallic = tex2D(_MetallicMap, i.uv).r;
				#endif
				
				float roughness = _Roughness;
				#if _ROUGHNESSMAP
				roughness *= tex2D(_RoughnessMap, i.uv).r;
				#endif
				
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
    	
    	Pass
    	{
    		Tags { "LightMode"="DepthOnlyPass" }
    		
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
                float2 depth : TEXCOORD0;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex.xyz);
                o.depth = o.vertex.zw;
                return o;
            }

    		void frag (v2f i)
            {
            	
            }
    		
    		ENDHLSL
    	}
    }
	CustomEditor "LWGUI.LWGUI"
}
