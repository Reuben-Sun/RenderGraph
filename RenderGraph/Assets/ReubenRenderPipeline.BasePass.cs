using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Experimental.Rendering.RenderGraphModule;
using UnityEngine.Rendering;

public partial class ReubenRenderPipeline
{
    private ShaderTagId _passName = new ShaderTagId("BasePass");
    
    public class BasePassData
    {
        public RendererListHandle _renderList_Qpaque;
        public RendererListHandle _renderList_Transparent;
        public TextureHandle _Albedo;
        public TextureHandle _Emission;
        public TextureHandle _Depth;
    }
    
    private TextureHandle CreateColorTexture(RenderGraph graph, Camera camera, string name)
    {
        bool colorRT_sRGB = (QualitySettings.activeColorSpace == ColorSpace.Linear);

        //Texture description
        TextureDesc colorRTDesc = new TextureDesc(camera.pixelWidth, camera.pixelHeight);
        colorRTDesc.colorFormat = GraphicsFormatUtility.GetGraphicsFormat(RenderTextureFormat.Default,colorRT_sRGB);
        colorRTDesc.depthBufferBits = 0;
        colorRTDesc.msaaSamples = MSAASamples.None;
        colorRTDesc.enableRandomWrite = false;
        colorRTDesc.clearBuffer = true;
        colorRTDesc.clearColor = Color.black;
        colorRTDesc.name = name;

        return graph.CreateTexture(colorRTDesc);
    }

    private TextureHandle CreateDepthTexture(RenderGraph graph, Camera camera)
    {
        bool colorRT_sRGB = (QualitySettings.activeColorSpace == ColorSpace.Linear);

        //Texture description
        TextureDesc colorRTDesc = new TextureDesc(camera.pixelWidth, camera.pixelHeight);
        colorRTDesc.colorFormat = GraphicsFormatUtility.GetGraphicsFormat(RenderTextureFormat.Depth,colorRT_sRGB);
        colorRTDesc.depthBufferBits = DepthBits.Depth24;
        colorRTDesc.msaaSamples = MSAASamples.None;
        colorRTDesc.enableRandomWrite = false;
        colorRTDesc.clearBuffer = true;
        colorRTDesc.clearColor = Color.black;
        colorRTDesc.name = "Depth";

        return graph.CreateTexture(colorRTDesc);
    }

    public BasePassData RenderBasePass(Camera camera, RenderGraph renderGraph, CullingResults cull)
    {
        using (var builder = renderGraph.AddRenderPass<BasePassData>("Base Pass", out var passData, new ProfilingSampler("Base Pass Profiler")))
        {
            //Create Texture
            TextureHandle Albedo = CreateColorTexture(renderGraph, camera, "Albedo");
            passData._Albedo = builder.UseColorBuffer(Albedo, 0);   //使用 SV_Target0
            TextureHandle Emission = CreateColorTexture(renderGraph, camera, "Emission");
            passData._Emission = builder.UseColorBuffer(Emission, 1);   //使用 SV_Target1
            TextureHandle Depth = CreateDepthTexture(renderGraph, camera);
            passData._Depth = builder.UseDepthBuffer(Depth, DepthAccess.Write);
            
            //Renderer
            UnityEngine.Rendering.RendererUtils.RendererListDesc OpaqueDesc = new UnityEngine.Rendering.RendererUtils.RendererListDesc(_passName, cull, camera);
            OpaqueDesc.sortingCriteria = SortingCriteria.CommonOpaque;
            OpaqueDesc.renderQueueRange = RenderQueueRange.opaque;
            RendererListHandle OpaqueListHandle = renderGraph.CreateRendererList(OpaqueDesc);
            passData._renderList_Qpaque = builder.UseRendererList(OpaqueListHandle);
            
            UnityEngine.Rendering.RendererUtils.RendererListDesc TransparentDesc = new UnityEngine.Rendering.RendererUtils.RendererListDesc(_passName, cull, camera);
            TransparentDesc.sortingCriteria = SortingCriteria.CommonTransparent;
            TransparentDesc.renderQueueRange = RenderQueueRange.transparent;
            RendererListHandle TransparentListHandle = renderGraph.CreateRendererList(TransparentDesc);
            passData._renderList_Transparent = builder.UseRendererList(TransparentListHandle);
            
            builder.SetRenderFunc((BasePassData data, RenderGraphContext context) =>
            {
                if (camera.clearFlags == CameraClearFlags.Skybox)
                {
                    context.renderContext.DrawSkybox(camera);
                }
                CoreUtils.DrawRendererList(context.renderContext, context.cmd, data._renderList_Qpaque);
                CoreUtils.DrawRendererList(context.renderContext, context.cmd, data._renderList_Transparent);
            });

            return passData;
        }
    }
}
