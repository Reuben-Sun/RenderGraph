using UnityEngine;
using UnityEngine.Experimental.Rendering.RenderGraphModule;
using UnityEngine.Rendering;

namespace Rendering.Reuben
{
    public partial class ReubenRenderPipeline
    {
        public class FinalBlitPassData
        {
            public Material blitMaterial;
            public TextureHandle _Sourse;
        }

        public void RenderFinalBlitPass(Camera camera, RenderGraph renderGraph, TextureHandle source)
        {
            using (var builder = renderGraph.AddRenderPass<FinalBlitPassData>("Final Blit Pass", out var passData, new ProfilingSampler("Final Blit Pass Profiler")))
            {
                if (passData.blitMaterial == null)
                {
                    passData.blitMaterial = CoreUtils.CreateEngineMaterial(Shader.Find("RenderGraph/FinalBlitPass"));
                }

                passData._Sourse = builder.ReadTexture(source);
                
                builder.SetRenderFunc((FinalBlitPassData data, RenderGraphContext context) =>
                {
                    passData.blitMaterial.SetTexture("_Destination", data._Sourse);
                    context.cmd.Blit(null, BuiltinRenderTextureType.CameraTarget, passData.blitMaterial);
                });
            }
        }
    }
}