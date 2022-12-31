using UnityEngine;
using UnityEngine.Experimental.Rendering.RenderGraphModule;
using UnityEngine.Rendering;
namespace Rendering.Reuben
{
    public partial class ReubenRenderPipeline
    {
        public class ShadingPassData
        {
            public Material shadingPassMaterial;
            //input
            public TextureHandle _MRT0;
            public TextureHandle _MRT1;
            public TextureHandle _MRT2;
            public TextureHandle _MRT3;
            public TextureHandle _Depth;
            //output
            public TextureHandle _Destination;
        }
    
        public ShadingPassData RenderShadingPass(Camera camera, RenderGraph renderGraph, GBufferPassData gBufferPassOutput)
        {
            using (var builder = renderGraph.AddRenderPass<ShadingPassData>("Shading Pass", out var passData, new ProfilingSampler("Shading Pass Profiler")))
            {
                if (passData.shadingPassMaterial == null)
                {
                    passData.shadingPassMaterial = CoreUtils.CreateEngineMaterial(Shader.Find("RenderGraph/ShadingPass"));
                }
                passData._MRT0 = builder.ReadTexture(gBufferPassOutput._MRT0);
                passData._MRT1 = builder.ReadTexture(gBufferPassOutput._MRT1);
                passData._MRT2 = builder.ReadTexture(gBufferPassOutput._MRT2);
                passData._MRT3 = builder.ReadTexture(gBufferPassOutput._MRT3);
                passData._Depth = builder.ReadTexture(gBufferPassOutput._Depth);
                TextureHandle _Destination = CreateColorTexture(renderGraph, camera, "_Destination");
                passData._Destination = builder.UseColorBuffer(_Destination, 0);

                builder.SetRenderFunc((ShadingPassData data, RenderGraphContext context) =>
                {
                    passData.shadingPassMaterial.SetTexture("_MRT0",data._MRT0);
                    passData.shadingPassMaterial.SetTexture("_MRT1",data._MRT1);
                    passData.shadingPassMaterial.SetTexture("_MRT2",data._MRT2);
                    passData.shadingPassMaterial.SetTexture("_MRT3",data._MRT3);
                    passData.shadingPassMaterial.SetTexture("_Depth", data._Depth);
                    context.cmd.Blit(null, passData._Destination, passData.shadingPassMaterial);
                    // CoreUtils.SetRenderTarget(context.cmd, passData._Destination, ClearFlag.None, 0);
                    // context.cmd.DrawProcedural(Matrix4x4.identity, passData.shadingPassMaterial, 0, MeshTopology.Triangles, 3);
                });
                return passData;
            }
        }
    }
}
