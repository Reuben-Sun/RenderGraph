using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.RenderGraphModule;
using UnityEngine.Rendering;

public partial class ReubenRenderPipeline : RenderPipeline
{
    private readonly string cmdName = "Reuben Render Graph";
    #region RenderGraph

    private RenderGraph _RenderGraph;

    void InitRenderGraph()
    {
        _RenderGraph = new RenderGraph("Reuben Render Graph");
    }

    void CleanupRenderGraph()
    {
        _RenderGraph.Cleanup();
        _RenderGraph = null;
    }

    #endregion
    
    
    protected override void Render(ScriptableRenderContext context, Camera[] cameras)
    {
        BeginFrameRendering(context, cameras);
        InitRenderGraph();
        RenderGamera(context, cameras[0]);
        _RenderGraph.EndFrame();
        EndFrameRendering(context, cameras);
    }

    protected override void Dispose(bool disposing)
    {
        CleanupRenderGraph();
    }

    void RenderGamera(ScriptableRenderContext context, Camera mainCamera)
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

        using (_RenderGraph.RecordAndExecute(rgParams))
        {
            GBufferPassData gBufferPassData = RenderBasePass(mainCamera, _RenderGraph, cull);
            RenderAddPass(_RenderGraph, new ShadingPassData()
            {
                _MRT0 = gBufferPassData._MRT0,
                _MRT1 = gBufferPassData._MRT1,
                _MRT2 = gBufferPassData._MRT2,
                _MRT3 = gBufferPassData._MRT3,
                _Depth = gBufferPassData._Depth
            });
        }
        
        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
        context.Submit();
        EndCameraRendering(context, mainCamera);
    }
}
