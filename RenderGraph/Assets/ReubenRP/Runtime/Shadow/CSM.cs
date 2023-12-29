using System.Collections.Generic;
using UnityEngine;

namespace Rendering.Reuben.Shadow
{
    public class CSM
    {
        public List<float> FrustumSplits = new List<float>() { 0.07f, 0.13f, 0.25f, 0.55f };
        // main camera frustum
        private Vector3[] _farCorners = new Vector3[4];
        private Vector3[] _nearCorners = new Vector3[4];
        // splited frustum
        private Vector3[] _f0NearCorners = new Vector3[4];
        private Vector3[] _f0FarCorners = new Vector3[4];
        private Vector3[] _f1NearCorners = new Vector3[4];
        private Vector3[] _f1FarCorners = new Vector3[4];
        private Vector3[] _f2NearCorners = new Vector3[4];
        private Vector3[] _f2FarCorners = new Vector3[4];
        private Vector3[] _f3NearCorners = new Vector3[4];
        private Vector3[] _f3FarCorners = new Vector3[4];
        
        private Vector3[] _box0, _box1, _box2, _box3;

        Vector3 MatTransform(Matrix4x4 m, Vector3 v, float w = 1.0f)
        {
            Vector4 v4 = new Vector4(v.x, v.y, v.z, w);
            v4 = m * v4;
            return new Vector3(v4.x, v4.y, v4.z);
        }
        
        Vector3[] GetLightSpaceAABB(Vector3[] nearCorners, Vector3[] farCorners, Vector3 lightDir)
        {
            Matrix4x4 toShadowViewInv = Matrix4x4.LookAt(Vector3.zero, lightDir, Vector3.up);
            Matrix4x4 toShadowView = toShadowViewInv.inverse;
            
            for (int i = 0; i < 4; i++)
            {
                _nearCorners[i] = MatTransform(toShadowView, nearCorners[i]);
                _farCorners[i] = MatTransform(toShadowView, farCorners[i]);
            }

            float[] x = new float[8];
            float[] y = new float[8];
            float[] z = new float[8];
            for (int i = 0; i < 4; i++)
            {
                x[i] = _nearCorners[i].x;
                y[i] = _nearCorners[i].y;
                z[i] = _nearCorners[i].z;
                x[i + 4] = _farCorners[i].x;
                y[i + 4] = _farCorners[i].y;
                z[i + 4] = _farCorners[i].z;
            }
            float minX = Mathf.Min(x);
            float minY = Mathf.Min(y);
            float minZ = Mathf.Min(z);
            float maxX = Mathf.Max(x);
            float maxY = Mathf.Max(y);
            float maxZ = Mathf.Max(z);

            Vector3[] lightSpaceAABB =
            {
                new Vector3(minX, minY, minZ),
                new Vector3(minX, minY, maxZ),
                new Vector3(minX, maxY, minZ),
                new Vector3(minX, maxY, maxZ),
                new Vector3(maxX, minY, minZ),
                new Vector3(maxX, minY, maxZ),
                new Vector3(maxX, maxY, minZ),
                new Vector3(maxX, maxY, maxZ)
            };

            for (int i = 0; i < 8; i++)
            {
                lightSpaceAABB[i] = MatTransform(toShadowViewInv, lightSpaceAABB[i]);
            }

            for (int i = 0; i < 4; i++)
            {
                _farCorners[i] = MatTransform(toShadowViewInv, _farCorners[i]);
                _nearCorners[i] = MatTransform(toShadowViewInv, _nearCorners[i]);
            }

            return lightSpaceAABB;
        }

        public void Tick(Camera mainCamera, Vector3 lightDir)
        {
            mainCamera.CalculateFrustumCorners(new Rect(0, 0, 1, 1), mainCamera.farClipPlane, Camera.MonoOrStereoscopicEye.Mono, _farCorners);
            mainCamera.CalculateFrustumCorners(new Rect(0, 0, 1, 1), mainCamera.nearClipPlane, Camera.MonoOrStereoscopicEye.Mono, _nearCorners);
            
            for (int i = 0; i < 4; i++) 
            {
                _farCorners[i] = mainCamera.transform.TransformVector(_farCorners[i]) + mainCamera.transform.position;
                _nearCorners[i] = mainCamera.transform.TransformVector(_nearCorners[i]) + mainCamera.transform.position;
            }

            // 按照比例划分相机视锥体
            for(int i=0; i<4; i++)
            {
                Vector3 dir = _farCorners[i] - _nearCorners[i];

                _f0NearCorners[i] = _nearCorners[i];
                _f0FarCorners[i] = _f0NearCorners[i] + dir * FrustumSplits[0];

                _f1NearCorners[i] = _f0FarCorners[i];
                _f1FarCorners[i] = _f1NearCorners[i] + dir * FrustumSplits[1];

                _f2NearCorners[i] = _f1FarCorners[i];
                _f2FarCorners[i] = _f2NearCorners[i] + dir * FrustumSplits[2];

                _f3NearCorners[i] = _f2FarCorners[i];
                _f3FarCorners[i] = _f3NearCorners[i] + dir * FrustumSplits[3];
            }

            // 计算包围盒
            _box0 = GetLightSpaceAABB(_f0NearCorners, _f0FarCorners, lightDir);
            _box1 = GetLightSpaceAABB(_f1NearCorners, _f1FarCorners, lightDir);
            _box2 = GetLightSpaceAABB(_f2NearCorners, _f2FarCorners, lightDir);
            _box3 = GetLightSpaceAABB(_f3NearCorners, _f3FarCorners, lightDir);
        }

        #region DebugDraw

        void DrawAABB(Vector3[] points, Color color)
        {
            // 画线
            Debug.DrawLine(points[0], points[1], color);
            Debug.DrawLine(points[0], points[2], color);
            Debug.DrawLine(points[0], points[4], color);

            Debug.DrawLine(points[6], points[2], color);
            Debug.DrawLine(points[6], points[7], color);
            Debug.DrawLine(points[6], points[4], color);

            Debug.DrawLine(points[5], points[1], color);
            Debug.DrawLine(points[5], points[7], color);
            Debug.DrawLine(points[5], points[4], color);

            Debug.DrawLine(points[3], points[1], color);
            Debug.DrawLine(points[3], points[2], color);
            Debug.DrawLine(points[3], points[7], color);
        }
        
        void DrawFrustum(Vector3[] nearCorners, Vector3[] farCorners, Color color)
        {
            for (int i = 0; i < 4; i++)
                Debug.DrawLine(nearCorners[i], farCorners[i], color);

            Debug.DrawLine(farCorners[0], farCorners[1], color);
            Debug.DrawLine(farCorners[0], farCorners[3], color);
            Debug.DrawLine(farCorners[2], farCorners[1], color);
            Debug.DrawLine(farCorners[2], farCorners[3], color);
            Debug.DrawLine(nearCorners[0], nearCorners[1], color);
            Debug.DrawLine(nearCorners[0], nearCorners[3], color);
            Debug.DrawLine(nearCorners[2], nearCorners[1], color);
            Debug.DrawLine(nearCorners[2], nearCorners[3], color);
        }

        public void DebugDraw()
        {
            DrawFrustum(_nearCorners, _farCorners, Color.white);
            DrawAABB(_box0, Color.yellow);  
            DrawAABB(_box1, Color.magenta);
            DrawAABB(_box2, Color.green);
            DrawAABB(_box3, Color.cyan);
        }

        #endregion

        #region ConfigCamera

        struct MainCameraSettings
        {
            public Vector3 position;
            public Quaternion rotation;
            public float nearClipPlane;
            public float farClipPlane;
            public float aspect;
        }

        private MainCameraSettings backupSettings = new MainCameraSettings();

        private void SaveCurrentMainCameraSettings(ref Camera camera)
        {
            backupSettings.position = camera.transform.position;
            backupSettings.rotation = camera.transform.rotation;
            backupSettings.farClipPlane = camera.farClipPlane;
            backupSettings.nearClipPlane = camera.nearClipPlane;
            backupSettings.aspect = camera.aspect;
        }

        private void RevertMainCameraSettings(ref Camera camera)
        {
            camera.transform.position = backupSettings.position;
            camera.transform.rotation = backupSettings.rotation;
            camera.farClipPlane = backupSettings.farClipPlane;
            camera.nearClipPlane = backupSettings.nearClipPlane;
            camera.aspect = backupSettings.aspect;
        }

        public void BeginShadowCamera(ref Camera camera)
        {
            SaveCurrentMainCameraSettings(ref camera);
            camera.orthographic = true;
        }

        public void EndShadowCamera(ref Camera camera)
        {
            RevertMainCameraSettings(ref camera);
            camera.orthographic = false;
        }
         
        public void SetCameraToShadowSpace(ref Camera camera, Vector3 lightDir, int level, float distance)
        {
            var box = new Vector3[8];
            if(level==0) box = _box0; 
            if(level==1) box = _box1; 
            if(level==2) box = _box2; 
            if(level==3) box = _box3;

            // 计算 Box 中点, 宽高比
            Vector3 center = (box[3] + box[4]) / 2; 
            float w = Vector3.Magnitude(box[0] - box[4]);
            float h = Vector3.Magnitude(box[0] - box[2]);

            // 配置相机
            camera.transform.rotation = Quaternion.LookRotation(lightDir);
            camera.transform.position = center; 
            camera.nearClipPlane = -distance;
            camera.farClipPlane = distance;
            camera.aspect = w / h;
            camera.orthographicSize = h * 0.5f;
        }
        
        
        #endregion
        
    }
}