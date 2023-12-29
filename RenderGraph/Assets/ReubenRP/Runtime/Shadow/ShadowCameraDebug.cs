using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rendering.Reuben.Shadow
{
    [ExecuteAlways]
    public class ShadowCameraDebug: MonoBehaviour
    {
        private CSM _csm;

        private void Update()
        {
            Camera camera = Camera.main;
            Light light = RenderSettings.sun;
            Vector3 lightDir = light.transform.rotation * Vector3.forward;
            if(_csm==null) _csm = new CSM();
            _csm.Tick(camera, lightDir);
        }

        private void OnDrawGizmosSelected()
        {
            _csm.DebugDraw();
        }
    }
}