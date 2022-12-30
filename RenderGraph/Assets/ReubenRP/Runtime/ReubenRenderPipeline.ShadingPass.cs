using UnityEngine;
using UnityEngine.Experimental.Rendering.RenderGraphModule;
using UnityEngine.Rendering;
namespace Rendering.Reuben
{
    public partial class ReubenRenderPipeline
    {
        private Material _material;
    
        public class ShadingPassData
        {
            public TextureHandle _MRT0;
            public TextureHandle _MRT1;
            public TextureHandle _MRT2;
            public TextureHandle _MRT3;
            public TextureHandle _Depth;
        }
    
        public void RenderAddPass(RenderGraph renderGraph, ShadingPassData shadingPassInput)
        {
            SetupMaterial();
            using (var builder = renderGraph.AddRenderPass<ShadingPassData>("Shading Pass", out var passData, new ProfilingSampler("Shading Pass Profiler")))
            {
                passData._MRT0 = builder.ReadTexture(shadingPassInput._MRT0);
                passData._MRT1 = builder.ReadTexture(shadingPassInput._MRT1);
                passData._MRT2 = builder.ReadTexture(shadingPassInput._MRT2);
                passData._MRT3 = builder.ReadTexture(shadingPassInput._MRT3);
                passData._Depth = builder.ReadTexture(shadingPassInput._Depth);
                
                builder.SetRenderFunc((ShadingPassData data, RenderGraphContext context) =>
                {
                    _material.SetTexture("_MRT0",data._MRT0);
                    _material.SetTexture("_MRT1",data._MRT1);
                    _material.SetTexture("_MRT2",data._MRT2);
                    _material.SetTexture("_MRT3",data._MRT3);
                    _material.SetTexture("_Depth", data._Depth);
                    
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
}
