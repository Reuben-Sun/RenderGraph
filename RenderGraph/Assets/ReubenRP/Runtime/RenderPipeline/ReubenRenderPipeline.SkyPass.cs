using UnityEngine;
using UnityEngine.Experimental.Rendering.RenderGraphModule;
using UnityEngine.Rendering;

namespace Rendering.Reuben
{
    public partial class ReubenRenderPipeline
    {
        public class SkyPassData
        {
            public Material skyMaterial;
            public TextureHandle _Depth;
            public TextureHandle _Source;
            public TextureHandle _Destination;
        }

        public SkyPassData RenderSkyPass(Camera camera, RenderGraph renderGraph, TextureHandle source, TextureHandle depth)
        {
            using (var builder = renderGraph.AddRenderPass<SkyPassData>("Sky Pass", out var passData, new ProfilingSampler("Sky Pass Profiler")))
            {
                // if (passData.skyMaterial == null)
                // {
                //     passData.skyMaterial = CoreUtils.CreateEngineMaterial(Shader.Find("RenderGraph/SkyPass"));
                // }

                passData._Depth = builder.ReadTexture(depth);
                passData._Source = builder.ReadTexture(source);
                TextureHandle _Destination = CreateColorTexture(renderGraph, camera, "_Destination");
                passData._Destination = builder.UseColorBuffer(_Destination, 0);
                
                
                if (passData.skyMaterial == null)
                {
                    passData.skyMaterial = RenderSettings.skybox;
                }
                // passData.skyMaterial.SetMatrix("MATRIX_I_VP", inverseViewProjection);

                builder.SetRenderFunc((SkyPassData data, RenderGraphContext context) =>
                {
                    passData.skyMaterial.SetTexture("_Depth", data._Depth);
                    passData.skyMaterial.SetTexture("_Source", data._Source);
                    context.cmd.Blit(null, passData._Destination, passData.skyMaterial);
                    CoreUtils.DrawFullScreen(context.cmd, passData.skyMaterial);
                });
                return passData;
            }
        }
    }
}