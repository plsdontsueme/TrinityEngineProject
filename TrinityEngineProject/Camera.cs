﻿using OpenTK.Mathematics;

namespace TrinityEngineProject
{
    /*
     * Access for:
     *  Renderer - apply view, projection matrix
     *  User - create and assign to mesh renderers
    */

    internal class Camera : Component
    {
        static Vector3 up = Vector3.UnitY;
        static Vector3 forward = -Vector3.UnitZ;

        static List<Camera> cameras = new List<Camera>();
        public static Camera main { get; private set; }

        float fov = float.Pi * 0.4f;
        float near = 0.01f;
        float far = 100;

        public Matrix4 GetViewMatrix()
        {
            Vector3 cameraDirection = transform.globalRotation * forward;
            Vector3 cameraRight = Vector3.Normalize(Vector3.Cross(up, cameraDirection));
            Vector3 cameraUp = Vector3.Cross(cameraDirection, cameraRight);

            Matrix4 mat0 = new Matrix4(new Vector4(-cameraRight, 0), new Vector4(cameraUp, 0), new Vector4(-cameraDirection, 0), new Vector4(Vector3.Zero, 1));
            Matrix4 mat1 = Matrix4.Identity;
            mat1.Column3 = new Vector4(-transform.globalPosition, 1);

            Matrix4 final = mat0 * mat1;
            final.Transpose();

            return final;
        }

        public Matrix4 GetProjectionMatrix()
        {
            return Matrix4.CreatePerspectiveFieldOfView(fov, TgMain.aspectRatio, near, far);
        }

        internal override void OnLoad()
        {
            base.OnLoad();
            cameras.Add(main);
            if (main == null)
            {
                main = this;
            }

        }
        internal override void OnUnload()
        {
            base.OnUnload();
            cameras.Remove(this);
            if(main == this)
            {
                if(cameras.Count > 0) main = cameras[0];
            }
        }

        public static readonly float UiGridDivisions = 1000f;
        public static Matrix4 GerUiProjectionMatrix()
        {
            return Matrix4.CreateOrthographic(UiGridDivisions * TgMain.aspectRatio, UiGridDivisions, 0, 0.01f);
        }
    }
}
