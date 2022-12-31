using UnityEngine;
using UnityEngine.Rendering;

namespace Rendering.Reuben
{
    public partial class ReubenRenderPipeline
    {
        private static int _DirectionalLightColorShaderId = Shader.PropertyToID("_DirectionalLightColor");
        private static int _DirectionalLightDirectionShaderId = Shader.PropertyToID("_DirectionalLightDirection");
        
        void SetupDirectionalLight(CommandBuffer cmd)
        {
            Light sunLight = RenderSettings.sun;
            cmd.SetGlobalVector(_DirectionalLightColorShaderId, sunLight.color);
            cmd.SetGlobalVector(_DirectionalLightDirectionShaderId, -sunLight.transform.forward);
        }
    }
}