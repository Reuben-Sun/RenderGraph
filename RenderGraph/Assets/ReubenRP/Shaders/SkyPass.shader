Shader "RenderGraph/SkyPass"
{
    Properties
    {
    	_Cubemap("Cubemap", CUBE) = ""{}
    	_Tint("Tint", Color) = (0.7, 0.7, 0.7, 1)
    }
    SubShader
    {
        Pass
        {
            Name "SkyPass"

            Cull Off ZWrite Off ZTest Always
	
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "../ShaderLibrary/Common.hlsl"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD;
			};
			
			samplerCUBE _Cubemap;
			float4 _Tint;
			
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
				float depth = tex2D(_Depth, i.uv).r;
				float4 pos = float4(i.uv.x * 2 -1 , i.uv.y * 2 -1, 1, 1);
				float3 posWS = mul(MATRIX_I_VP, pos);
				float3 viewDirWS = normalize(posWS);
				
				float3 finalColor = 0;
				
				if(depth > 0)
				{
					finalColor = tex2D(_Source, i.uv).rgb;
				}
				else
				{
					finalColor = texCUBE(_Cubemap, viewDirWS) * _Tint.xyz;
				}
				return half4(finalColor, 1);	
			}
			ENDHLSL
        }
    }
}