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
            
            
            RenderCamera(context, cameras[0], m_RenderGraph);
            m_RenderGraph.EndFrame();
            
            EndFrameRendering(context, cameras);
        }
    
        protected override void Dispose(bool disposing)
        {
            m_RenderGraph.Cleanup();
            m_RenderGraph = null;
        }
    
        void RenderCamera(ScriptableRenderContext context, Camera mainCamera, RenderGraph graph)
        {
            BeginCameraRendering(context, mainCamera);
    
            ScriptableCullingParameters cullingParams;
            if(!mainCamera.TryGetCullingParameters(out cullingParams)) return;
            CullingResults cull = context.Cull(ref cullingParams);
    
            context.SetupCameraProperties(mainCamera);
            
            CommandBuffer cmd = CommandBufferPool.Get(cmdName);
            RenderGraphParameters rgParams = new RenderGraphParameters()
            {
                executionName = "Reuben Render Graph Excute",
                commandBuffer = cmd,
                scriptableRenderContext = context,
                currentFrameIndex = Time.frameCount
            };
    
            using (m_RenderGraph.RecordAndExecute(rgParams))
            {
                GBufferPassData gBufferPassData = RenderGBufferPass(mainCamera, m_RenderGraph, cull);
                RenderAddPass(m_RenderGraph, new ShadingPassData()
                {
                    _MRT0 = gBufferPassData._MRT0,
                    _MRT1 = gBufferPassData._MRT1,
                    _MRT2 = gBufferPassData._MRT2,
                    _MRT3 = gBufferPassData._MRT3,
                    _Depth = gBufferPassData._Depth
                });
            }
            
            EndCameraRendering(context, mainCamera);
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
            context.Submit();
        }
    }

}
