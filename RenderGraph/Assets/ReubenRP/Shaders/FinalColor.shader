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
			#include "../ShaderLibrary/Common.hlsl"

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
				return half4(surface.albedo, 1);	
			}
			ENDHLSL
        }
    }
}