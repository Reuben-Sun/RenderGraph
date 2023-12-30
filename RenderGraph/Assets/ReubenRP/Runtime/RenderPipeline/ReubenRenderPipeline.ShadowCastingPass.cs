using UnityEngine;
using UnityEngine.Experimental.Rendering.RenderGraphModule;
using UnityEngine.Rendering;

namespace Rendering.Reuben
{
    public partial class ReubenRenderPipeline
    {
        private ShaderTagId depthOnlyPassName = new ShaderTagId("DepthOnlyPass");

        public class ShadowCastingPassData
        {
            public RendererListHandle RenderListOpaque;
            public TextureHandle Shadowmap;
        }

        public ShadowCastingPassData RenderShadowCastingPass(Camera camera, RenderGraph renderGraph,
            CullingResults cull)
        {
            using (var builder = renderGraph.AddRenderPass<ShadowCastingPassData>("Shadow Casting Pass", out var passData, new ProfilingSampler("Shadow Casting Pass Profiler")))
            {
                TextureHandle shadowMap = CreateShadowMap(renderGraph, 1024, 1024);
                passData.Shadowmap = builder.UseColorBuffer(shadowMap, 0);
                
                //Renderer
                Light sunLight = RenderSettings.sun;
                Vector3 lightDir = sunLight.transform.rotation * Vector3.forward;
                    
                UnityEngine.Rendering.RendererUtils.RendererListDesc OpaqueDesc = new UnityEngine.Rendering.RendererUtils.RendererListDesc(depthOnlyPassName, cull, camera);
                OpaqueDesc.sortingCriteria = SortingCriteria.CommonOpaque;
                OpaqueDesc.renderQueueRange = RenderQueueRange.opaque;
                RendererListHandle OpaqueListHandle = renderGraph.CreateRendererList(OpaqueDesc);
                passData.RenderListOpaque = builder.UseRendererList(OpaqueListHandle);
                
                
                builder.SetRenderFunc((ShadowCastingPassData data, RenderGraphContext context) =>
                {
                    CoreUtils.DrawRendererList(context.renderContext, context.cmd, data.RenderListOpaque);
                    
                    // Camera shadowCamera = new Camera();
                    // shadowCamera.transform.rotation = Quaternion.LookRotation(lightDir);
                    // shadowCamera.transform.position = Vector3.zero; 
                    // shadowCamera.nearClipPlane = -1;
                    // shadowCamera.farClipPlane = 100;
                    // shadowCamera.aspect = 1.0f;
                    // shadowCamera.orthographicSize = 200;
                    // cull.ComputeDirectionalShadowMatricesAndCullingPrimitives(0, 0, 1, Vector3.zero, 1024,
                    //     camera.nearClipPlane, out Matrix4x4 viewMatrix, out Matrix4x4 projMatrix, out ShadowSplitData splitData);
                    // var settings = new ShadowDrawingSettings(cull, 0, BatchCullingProjectionType.Orthographic);
                    // context.renderContext.DrawShadows(ref settings);
                    // TODO: pass vp matrix to shader
                    // SortingSettings sortingSettings = new SortingSettings(camera);
                    // DrawingSettings drawingSettings = new DrawingSettings(depthOnlyPassName, sortingSettings);
                    // FilteringSettings filteringSettings = FilteringSettings.defaultValue;
                    // context.renderContext.DrawRenderers(cull, ref drawingSettings, ref filteringSettings);
                });
                
                return passData;
            }
        }
        
    }
}