using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Experimental.Rendering.RenderGraphModule;
using UnityEngine.Rendering;

namespace Rendering.Reuben
{
    public partial class ReubenRenderPipeline
    {
        private TextureHandle CreateColorTexture(RenderGraph graph, Camera camera, string name)
        {
            bool colorRT_sRGB = (QualitySettings.activeColorSpace == ColorSpace.Linear);
    
            //Texture description
            TextureDesc colorRTDesc = new TextureDesc(camera.pixelWidth, camera.pixelHeight);
            colorRTDesc.colorFormat = GraphicsFormatUtility.GetGraphicsFormat(RenderTextureFormat.BGRA32,colorRT_sRGB);
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
        
        private TextureHandle CreateShadowMap(RenderGraph graph, int width, int height)
        {
            bool colorRT_sRGB = (QualitySettings.activeColorSpace == ColorSpace.Linear);
    
            //Texture description
            TextureDesc colorRTDesc = new TextureDesc(width, height);
            colorRTDesc.colorFormat = GraphicsFormatUtility.GetGraphicsFormat(RenderTextureFormat.Depth,colorRT_sRGB);
            colorRTDesc.depthBufferBits = DepthBits.Depth24;
            colorRTDesc.msaaSamples = MSAASamples.None;
            colorRTDesc.enableRandomWrite = false;
            colorRTDesc.clearBuffer = true;
            colorRTDesc.clearColor = Color.black;
            colorRTDesc.name = "ShadowMap";
    
            return graph.CreateTexture(colorRTDesc);
        }

    }
}