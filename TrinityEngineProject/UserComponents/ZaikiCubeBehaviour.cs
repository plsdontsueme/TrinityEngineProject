using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrinityEngineProject
{
    internal class ZaikiCubeBehaviour : ComponentUpdated
    {
        Vector3 lastTarget;
        float speed = 0.8f;

        internal override void OnLoad()
        {
            base.OnLoad();
        }

        protected override void Update(FrameEventArgs e)
        {
            base.Update(e);
            Vector3 target = Camera.main.transform.globalPosition;
            lastTarget.Y = MathHelper.Lerp(lastTarget.Y, target.Y, speed * Time.deltaTime);
            lastTarget.X = MathHelper.Lerp(lastTarget.X, target.X, speed * Time.deltaTime);
            lastTarget.Z = MathHelper.Lerp(lastTarget.Z, target.Z, speed * Time.deltaTime);

            transform.rotation = LookAt(lastTarget, transform.position);

            Quaternion LookAt(Vector3 targetPosition, Vector3 currentPosition)
            {
                // Calculate the direction from the current position to the target position
                Vector3 direction = targetPosition - currentPosition;

                // Create a view matrix that looks from the current position to the target position
                Matrix4 viewMatrix = Matrix4.LookAt(currentPosition, targetPosition, Vector3.UnitY);

                // Extract the rotation part (upper-left 3x3 part) from the view matrix
                Matrix3 rotationMatrix = new Matrix3(viewMatrix);

                // Convert the rotation matrix to a quaternion
                Quaternion rotation = Quaternion.FromMatrix(rotationMatrix);

                return rotation;
            }
        }
    }
}
