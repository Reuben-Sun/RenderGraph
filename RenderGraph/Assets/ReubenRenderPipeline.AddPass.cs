using UnityEngine;
using UnityEngine.Experimental.Rendering.RenderGraphModule;
using UnityEngine.Rendering;

public partial class ReubenRenderPipeline
{
    private Material _material;

    public class AddPassData
    {
        public TextureHandle _Albedo;
        public TextureHandle _Emission;
    }

    public void RenderAddPass(RenderGraph renderGraph, TextureHandle albedo, TextureHandle emission)
    {
        SetupMaterial();
        using (var builder = renderGraph.AddRenderPass<AddPassData>("Add Pass", out var passData, new ProfilingSampler("Add Pass Profiler")))
        {
            passData._Albedo = builder.ReadTexture(albedo);
            passData._Emission = builder.ReadTexture(emission);
            
            builder.SetRenderFunc((AddPassData data, RenderGraphContext context) =>
            {
                _material.SetTexture("_CameraAlbedoTexture",data._Albedo);
                _material.SetTexture("_CameraEmissionTexture",data._Emission);
                context.cmd.Blit(null, BuiltinRenderTextureType.CameraTarget, _material);
            });
        }
    }

    private void SetupMaterial()
    {
        if (_material == null)
        {
            _material = CoreUtils.CreateEngineMaterial(Shader.Find("RenderGraph/FinalColor"));
        }
    }
}