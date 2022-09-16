Shader "RenderGraph/FinalColor"
{
    Properties
    {
    }
    SubShader
    {
        Pass
        {
            Name "FinalColor"

            Cull Off ZWrite Off ZTest Always
	
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

			CBUFFER_START(UnityPerMaterial)
			sampler2D _MRT0;
			sampler2D _MRT1;
			sampler2D _MRT2;
			sampler2D _MRT3;
			sampler2D _Depth;
			CBUFFER_END

			float4 frag (v2f i) : SV_Target
			{
				float4 albedo = tex2D(_MRT0, i.uv);
				float4 emission = tex2D(_MRT1, i.uv);
				float depth = tex2D(_Depth, i.uv).r;
				return depth;
			}
			ENDHLSL
        }
    }
}