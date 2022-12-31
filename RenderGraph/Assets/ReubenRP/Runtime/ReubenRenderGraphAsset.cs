using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Rendering.Reuben
{
    [ExecuteInEditMode]
    public class ReubenRenderGraphAsset : RenderPipelineAsset
    {
    #if UNITY_EDITOR
        [UnityEditor.MenuItem("Assets/Create/Render Pipeline/ReubenRenderGraph", priority = 1)]
        static void CreateReubenRenderGraph()
        {
            var instance = ScriptableObject.CreateInstance<ReubenRenderGraphAsset>();
            UnityEditor.AssetDatabase.CreateAsset(instance, "Assets/ReubenRenderGraph.asset");
        }
    #endif

        protected override RenderPipeline CreatePipeline() => new ReubenRenderPipeline(this);
    }
}
