using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Experimental.Rendering.RenderGraphModule;
using UnityEngine.Rendering;
namespace Rendering.Reuben
{
    public partial class ReubenRenderPipeline
    {
        private ShaderTagId gbufferPassName = new ShaderTagId("GBufferPass");
        
        public class GBufferPassData
        {
            public RendererListHandle RenderListOpaque;
            // public RendererListHandle _renderList_Transparent;
            public TextureHandle MRT0;
            public TextureHandle MRT1;
            public TextureHandle MRT2;
            public TextureHandle MRT3;
            public TextureHandle Depth;
        }
        
        public GBufferPassData RenderGBufferPass(Camera camera, RenderGraph renderGraph, CullingResults cull)
        {
            using (var builder = renderGraph.AddRenderPass<GBufferPassData>("GBuffer Pass", out var passData, new ProfilingSampler("GBuffer Pass Profiler")))
            {
                //Create Texture
                TextureHandle _MRT0 = CreateColorTexture(renderGraph, camera, "_MRT0");
                passData.MRT0 = builder.UseColorBuffer(_MRT0, 0);   //使用 SV_Target0
                TextureHandle _MRT1 = CreateColorTexture(renderGraph, camera, "_MRT1");
                passData.MRT1 = builder.UseColorBuffer(_MRT1, 1);   //使用 SV_Target1
                TextureHandle _MRT2 = CreateColorTexture(renderGraph, camera, "_MRT2");
                passData.MRT2 = builder.UseColorBuffer(_MRT2, 2);   //使用 SV_Target2
                TextureHandle _MRT3 = CreateColorTexture(renderGraph, camera, "_MRT3");
                passData.MRT3 = builder.UseColorBuffer(_MRT3, 3);   //使用 SV_Target3
                TextureHandle Depth = CreateDepthTexture(renderGraph, camera);
                passData.Depth = builder.UseDepthBuffer(Depth, DepthAccess.Write);
                
                //Renderer
                UnityEngine.Rendering.RendererUtils.RendererListDesc OpaqueDesc = new UnityEngine.Rendering.RendererUtils.RendererListDesc(gbufferPassName, cull, camera);
                OpaqueDesc.sortingCriteria = SortingCriteria.CommonOpaque;
                OpaqueDesc.renderQueueRange = RenderQueueRange.opaque;
                RendererListHandle OpaqueListHandle = renderGraph.CreateRendererList(OpaqueDesc);
                passData.RenderListOpaque = builder.UseRendererList(OpaqueListHandle);
                
                /*
                UnityEngine.Rendering.RendererUtils.RendererListDesc TransparentDesc = new UnityEngine.Rendering.RendererUtils.RendererListDesc(_passName, cull, camera);
                TransparentDesc.sortingCriteria = SortingCriteria.CommonTransparent;
                TransparentDesc.renderQueueRange = RenderQueueRange.transparent;
                RendererListHandle TransparentListHandle = renderGraph.CreateRendererList(TransparentDesc);
                passData._renderList_Transparent = builder.UseRendererList(TransparentListHandle);
                */
                
                builder.SetRenderFunc((GBufferPassData data, RenderGraphContext context) =>
                {
                    CoreUtils.DrawRendererList(context.renderContext, context.cmd, data.RenderListOpaque);
                    /*
                    if (camera.clearFlags == CameraClearFlags.Skybox)
                    {
                        context.renderContext.DrawSkybox(camera);
                    }
                    CoreUtils.DrawRendererList(context.renderContext, context.cmd, data._renderList_Transparent);
                    */
                });
    
                return passData;
            }
        }
    }
}

