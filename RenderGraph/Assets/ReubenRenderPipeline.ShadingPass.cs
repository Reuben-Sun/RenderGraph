using UnityEngine;
using UnityEngine.Experimental.Rendering.RenderGraphModule;
using UnityEngine.Rendering;

public partial class ReubenRenderPipeline
{
    private Material _material;

    public class ShadingPassData
    {
        public TextureHandle _MRT0;
        public TextureHandle _MRT1;
    }

    public void RenderAddPass(RenderGraph renderGraph, ShadingPassData shadingPassInput)
    {
        SetupMaterial();
        using (var builder = renderGraph.AddRenderPass<ShadingPassData>("Shading Pass", out var passData, new ProfilingSampler("Shading Pass Profiler")))
        {
            passData._MRT0 = builder.ReadTexture(shadingPassInput._MRT0);
            passData._MRT1 = builder.ReadTexture(shadingPassInput._MRT1);
            
            builder.SetRenderFunc((ShadingPassData data, RenderGraphContext context) =>
            {
                _material.SetTexture("_MRT0",data._MRT0);
                _material.SetTexture("_MRT1",data._MRT1); ;
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