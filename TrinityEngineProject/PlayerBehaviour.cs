using OpenTK.Windowing.Common;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace TrinityEngineProject
{
    internal class PlayerBehaviour : ComponentUpdated
    {
        public PlayerBehaviour()
        {
            Input.MouseMove += Input_MouseMove;
        }

        Vector2d sensitivity = new(4f, 4f);
        Vector2d virtualMouse;
        private void Input_MouseMove(MouseMoveEventArgs e)
        {
            virtualMouse.X -= (double)e.DeltaX * sensitivity.X * Time.deltaTime;
            virtualMouse.Y = MathHelper.Clamp(virtualMouse.Y - (double)e.DeltaY * Time.deltaTime * sensitivity.Y, -1.569d, 1.569d);
            transform.rotation = Quaternion.FromAxisAngle(Vector3.UnitY, (float)virtualMouse.X) * Quaternion.FromAxisAngle(Vector3.UnitX, (float)virtualMouse.Y);
        }

        protected override void Update(FrameEventArgs e)
        {
            Vector3 movement = Vector3.Zero;
            if (Input.GetKey(Keys.W)) movement.Z -= 1;
            if (Input.GetKey(Keys.S)) movement.Z += 1;
            if (Input.GetKey(Keys.A)) movement.X -= 1;
            if (Input.GetKey(Keys.D)) movement.X += 1;
            if (movement.Length == 0) return;
            movement.Normalize();
            transform.position += movement * Time.deltaTime * 3;
        }
    }
}
