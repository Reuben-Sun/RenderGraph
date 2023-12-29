using UnityEngine;
using UnityEngine.Rendering;

namespace Rendering.Reuben
{
    public partial class ReubenRenderPipeline
    {
        void SetupCamera(CommandBuffer cmd, Camera camera)
        {
            Matrix4x4 inverseViewMatrix = Matrix4x4.Inverse(camera.worldToCameraMatrix);
            Matrix4x4 inverseProjectionMatrix = Matrix4x4.Inverse(camera.projectionMatrix);
            Matrix4x4 inverseViewProjection = inverseViewMatrix * inverseProjectionMatrix;
            cmd.SetGlobalMatrix("MATRIX_I_VP", inverseViewProjection);
            cmd.SetGlobalVector("_MainCameraPosWS", camera.transform.position);
        }
    }
}