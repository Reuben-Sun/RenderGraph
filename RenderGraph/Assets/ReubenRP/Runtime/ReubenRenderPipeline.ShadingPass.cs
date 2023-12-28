using UnityEngine;
using UnityEngine.Experimental.Rendering.RenderGraphModule;
using UnityEngine.Rendering;
namespace Rendering.Reuben
{
    public partial class ReubenRenderPipeline
    {
        public class ShadingPassData
        {
            public Material ShadingPassMaterial;
            //input
            public TextureHandle MRT0;
            public TextureHandle MRT1;
            public TextureHandle MRT2;
            public TextureHandle MRT3;
            public TextureHandle Depth;
            //output
            public TextureHandle Destination;
        }
    
        public ShadingPassData RenderShadingPass(Camera camera, RenderGraph renderGraph, GBufferPassData gBufferPassOutput)
        {
            using (var builder = renderGraph.AddRenderPass<ShadingPassData>("Shading Pass", out var passData, new ProfilingSampler("Shading Pass Profiler")))
            {
                if (passData.ShadingPassMaterial == null)
                {
                    passData.ShadingPassMaterial = CoreUtils.CreateEngineMaterial(Shader.Find("RenderGraph/ShadingPass"));
                }
                passData.MRT0 = builder.ReadTexture(gBufferPassOutput.MRT0);
                passData.MRT1 = builder.ReadTexture(gBufferPassOutput.MRT1);
                passData.MRT2 = builder.ReadTexture(gBufferPassOutput.MRT2);
                passData.MRT3 = builder.ReadTexture(gBufferPassOutput.MRT3);
                passData.Depth = builder.ReadTexture(gBufferPassOutput.Depth);
                TextureHandle _Destination = CreateColorTexture(renderGraph, camera, "_Destination");
                passData.Destination = builder.UseColorBuffer(_Destination, 0);

                builder.SetRenderFunc((ShadingPassData data, RenderGraphContext context) =>
                {
                    passData.ShadingPassMaterial.SetTexture("_MRT0",data.MRT0);
                    passData.ShadingPassMaterial.SetTexture("_MRT1",data.MRT1);
                    passData.ShadingPassMaterial.SetTexture("_MRT2",data.MRT2);
                    passData.ShadingPassMaterial.SetTexture("_MRT3",data.MRT3);
                    passData.ShadingPassMaterial.SetTexture("_Depth", data.Depth);
                    context.cmd.Blit(null, passData.Destination, passData.ShadingPassMaterial);
                    // CoreUtils.SetRenderTarget(context.cmd, passData._Destination, ClearFlag.None, 0);
                    // context.cmd.DrawProcedural(Matrix4x4.identity, passData.shadingPassMaterial, 0, MeshTopology.Triangles, 3);
                });
                return passData;
            }
        }
    }
}
