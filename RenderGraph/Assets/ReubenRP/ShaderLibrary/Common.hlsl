#ifndef COMMON_INCLUDE
#define COMMON_INCLUDE

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

CBUFFER_START(UnityPerMaterial)
    sampler2D _MRT0;
    sampler2D _MRT1;
    sampler2D _MRT2;
    sampler2D _MRT3;
    sampler2D _Depth;
    sampler2D _Destination;
CBUFFER_END

struct SurfaceData
{
    float3 albedo;
    float shadow;
    float3 emission;
    float3 normal;
    float shadingMode;
    float metallic;
    float roughness;
    float occlusion;
    float fog;
    float depth;
};

SurfaceData GetSurfaceData(float2 uv)
{
    SurfaceData surface = (SurfaceData)0;
    half4 mrt0 = tex2D(_MRT0, uv);
    half4 mrt1 = tex2D(_MRT1, uv);
    half4 mrt2 = tex2D(_MRT2, uv);
    half4 mrt3 = tex2D(_MRT3, uv);
    half depth = tex2D(_Depth, uv);

    surface.albedo = mrt0.rgb;
    surface.shadow = mrt0.a;
    surface.emission = mrt1.rgb;
    surface.normal = mrt2.rgb;
    surface.shadingMode = mrt2.a;
    surface.metallic = mrt3.r;
    surface.roughness = mrt3.g;
    surface.occlusion = mrt3.b;
    surface.fog = mrt3.a;
    surface.depth = depth;
    
    return surface;
}
#endif
