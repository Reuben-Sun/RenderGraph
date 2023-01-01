using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.RenderGraphModule;
using UnityEngine.Rendering;

namespace Rendering.Reuben
{
    public partial class ReubenRenderPipeline : RenderPipeline
    {
        private readonly string cmdName = "Reuben Render Graph";

        private RenderGraph m_RenderGraph = new RenderGraph("ReubenRP");
        

        public ReubenRenderPipeline(ReubenRenderGraphAsset asset)
        {
            
        }
        
        protected override void Render(ScriptableRenderContext context, Camera[] cameras)
        {
            BeginFrameRendering(context, cameras);
            
            RenderCamera(context, cameras[0]);
            m_RenderGraph.EndFrame();
            
            EndFrameRendering(context, cameras);
        }
    
        protected override void Dispose(bool disposing)
        {
            m_RenderGraph.Cleanup();
            m_RenderGraph = null;
        }
    
        void RenderCamera(ScriptableRenderContext context, Camera mainCamera)
        {
            BeginCameraRendering(context, mainCamera);
    
            ScriptableCullingParameters cullingParams;
            if(!mainCamera.TryGetCullingParameters(out cullingParams)) return;
            CullingResults cull = context.Cull(ref cullingParams);
    
            context.SetupCameraProperties(mainCamera);
            
            CommandBuffer cmd = CommandBufferPool.Get(cmdName);
            RenderGraphParameters rgParams = new RenderGraphParameters()
            {
                executionName = "Reuben Render Graph Execute",
                commandBuffer = cmd,
                scriptableRenderContext = context,
                currentFrameIndex = Time.frameCount
            };
    
            using (m_RenderGraph.RecordAndExecute(rgParams))
            {
                SetupDirectionalLight(cmd);
                SetupCamera(cmd, mainCamera);
                GBufferPassData gBufferPassData = RenderGBufferPass(mainCamera, m_RenderGraph, cull);
                ShadingPassData shadingPassData = RenderShadingPass(mainCamera, m_RenderGraph, gBufferPassData);
                SkyPassData skyPassData = RenderSkyPass(mainCamera, m_RenderGraph, shadingPassData._Destination, gBufferPassData._Depth);
                RenderFinalBlitPass(mainCamera, m_RenderGraph, skyPassData._Destination);
            }
            
            EndCameraRendering(context, mainCamera);
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
            context.Submit();
        }
    }

}
