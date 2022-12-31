using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Experimental.Rendering.RenderGraphModule;
using UnityEngine.Rendering;
namespace Rendering.Reuben
{
    public partial class ReubenRenderPipeline
    {
        private ShaderTagId _passName = new ShaderTagId("GBufferPass");
        
        public class GBufferPassData
        {
            public RendererListHandle _renderList_Qpaque;
            // public RendererListHandle _renderList_Transparent;
            public TextureHandle _MRT0;
            public TextureHandle _MRT1;
            public TextureHandle _MRT2;
            public TextureHandle _MRT3;
            public TextureHandle _Depth;
        }
        
        public GBufferPassData RenderGBufferPass(Camera camera, RenderGraph renderGraph, CullingResults cull)
        {
            using (var builder = renderGraph.AddRenderPass<GBufferPassData>("GBuffer Pass", out var passData, new ProfilingSampler("GBuffer Pass Profiler")))
            {
                //Create Texture
                TextureHandle _MRT0 = CreateColorTexture(renderGraph, camera, "_MRT0");
                passData._MRT0 = builder.UseColorBuffer(_MRT0, 0);   //使用 SV_Target0
                TextureHandle _MRT1 = CreateColorTexture(renderGraph, camera, "_MRT1");
                passData._MRT1 = builder.UseColorBuffer(_MRT1, 1);   //使用 SV_Target1
                TextureHandle _MRT2 = CreateColorTexture(renderGraph, camera, "_MRT2");
                passData._MRT2 = builder.UseColorBuffer(_MRT2, 2);   //使用 SV_Target2
                TextureHandle _MRT3 = CreateColorTexture(renderGraph, camera, "_MRT3");
                passData._MRT3 = builder.UseColorBuffer(_MRT3, 3);   //使用 SV_Target3
                TextureHandle Depth = CreateDepthTexture(renderGraph, camera);
                passData._Depth = builder.UseDepthBuffer(Depth, DepthAccess.Write);
                
                //Renderer
                UnityEngine.Rendering.RendererUtils.RendererListDesc OpaqueDesc = new UnityEngine.Rendering.RendererUtils.RendererListDesc(_passName, cull, camera);
                OpaqueDesc.sortingCriteria = SortingCriteria.CommonOpaque;
                OpaqueDesc.renderQueueRange = RenderQueueRange.opaque;
                RendererListHandle OpaqueListHandle = renderGraph.CreateRendererList(OpaqueDesc);
                passData._renderList_Qpaque = builder.UseRendererList(OpaqueListHandle);
                
                /*
                UnityEngine.Rendering.RendererUtils.RendererListDesc TransparentDesc = new UnityEngine.Rendering.RendererUtils.RendererListDesc(_passName, cull, camera);
                TransparentDesc.sortingCriteria = SortingCriteria.CommonTransparent;
                TransparentDesc.renderQueueRange = RenderQueueRange.transparent;
                RendererListHandle TransparentListHandle = renderGraph.CreateRendererList(TransparentDesc);
                passData._renderList_Transparent = builder.UseRendererList(TransparentListHandle);
                */
                
                builder.SetRenderFunc((GBufferPassData data, RenderGraphContext context) =>
                {
                    CoreUtils.DrawRendererList(context.renderContext, context.cmd, data._renderList_Qpaque);
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

