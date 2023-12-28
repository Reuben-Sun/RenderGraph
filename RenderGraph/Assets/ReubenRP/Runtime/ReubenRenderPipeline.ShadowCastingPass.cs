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
            public TextureHandle Depth;
        }

        public ShadowCastingPassData RenderShadowCastingPass(Camera camera, RenderGraph renderGraph,
            CullingResults cull)
        {
            using (var builder = renderGraph.AddRenderPass<ShadowCastingPassData>("Shadow Casting Pass", out var passData, new ProfilingSampler("Shadow Casting Pass Profiler")))
            {
                // TextureHandle Depth = CreateDepthTexture(renderGraph, camera);
                // passData.Depth = builder.UseDepthBuffer(Depth, DepthAccess.Write);
                
                //Renderer

                builder.SetRenderFunc((ShadowCastingPassData data, RenderGraphContext context) =>
                {
                    // Light sunLight = RenderSettings.sun;
                    // Vector3 lightDir = sunLight.transform.rotation * Vector3.forward;
                    // cull.ComputeDirectionalShadowMatricesAndCullingPrimitives(sunLight, 0, 1, Vector3.zero, 1024,
                    //     camera.nearClipPlane, out Matrix4x4 shadowMatrix, out _, out _);
                    //
                    // Matrix4x4 viewMatrix = camera.worldToCameraMatrix;
                    // Matrix4x4 projMatrix = GL.GetGPUProjectionMatrix(camera.projectionMatrix, false);
                    // // TODO: pass vp matrix to shader
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