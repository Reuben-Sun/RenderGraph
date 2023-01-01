Shader "RenderGraph/ShadingPass"
{
    Properties
    {
    }
    SubShader
    {
        Pass
        {
            Name "ShadingPass"

            Cull Off ZWrite Off ZTest Always
	
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "../ShaderLibrary/Common.hlsl"
			#include "../ShaderLibrary/Light.hlsl"

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

			float4 frag (v2f i) : SV_Target
			{
				SurfaceData surface = GetSurfaceData(i.uv);
				Light sunLight = GetDirectionalLight();
				float4 pos = float4(i.uv.x * 2 -1 , i.uv.y * 2 -1, 1, surface.depth);
				float3 posWS = mul(MATRIX_I_VP, pos);
				float3 viewDirWS = SafeNormalize(_MainCameraPosWS - posWS);

				float3 L = normalize(sunLight.direction);
				half3 H = normalize(viewDirWS + L);     
                float VoH = max(0.001, saturate(dot(viewDirWS, H)));
                float NoV = max(0.001, saturate(dot(surface.normal, viewDirWS)));
                float NoL = max(0.001, saturate(dot(surface.normal, L)));
                float NoH = saturate(dot(surface.normal, H));
				half3 radiance = sunLight.color * NoL;	//TODO: *= mainLight.shadowAttenuation
				
				float3 diffuseColor = surface.albedo * radiance;
				half3 F0 = lerp(half3(0.04, 0.04, 0.04), surface.albedo, surface.metallic);
				float3 F_Schlick = F0 + (1-F0) * pow(1 - VoH, 5.0);
				float a = surface.roughness * surface.roughness;
                float a2 = a * a;
                float d = (NoH * a2 - NoH) * NoH + 1;
                float D_GGX = a2 / (PI * d * d);
				float k = (surface.roughness + 1) * (surface.roughness + 1) / 8;
                float GV = NoV / (NoV * (1-k) + k);
                float GL = NoL / (NoL * (1-k) + k);
                float G_GGX = GV * GL;
				float3 brdf = F_Schlick * D_GGX * G_GGX / (4 * NoV * NoL);
                float3 specularColor = brdf * radiance * PI;

				//TODO: Fog
				//TODO: GI
				float3 finalColor = diffuseColor + specularColor;
				// float3 finalColor = surface.metallic;
				return half4(finalColor, 1);	
			}
			ENDHLSL
        }
    }
}