using OpenTK.Windowing.Common;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace TrinityEngineProject
{
    internal class PlayerBehaviour : ComponentUpdated
    {
        Transform camTransform;
        public PlayerBehaviour(Transform camTransform)
        {
            this.camTransform = camTransform;
            Input.MouseMove += Input_MouseMove;
            Input.GrabCursor();
        }

        Vector2d sensitivity = new(0.001f, 0.001f);
        Vector2d virtualMouse;
        Vector2 lastMouse;
        bool first = true;
        private void Input_MouseMove(MouseMoveEventArgs e)
        {
            if (!Input.CursorGrabbed) return;

            if (first)
            {
                lastMouse = e.Position;
                first = false;
            }
            virtualMouse.X -= ((double)e.Position.X-lastMouse.X) * sensitivity.X;
            virtualMouse.Y = MathHelper.Clamp(virtualMouse.Y - ((double)e.Position.Y - lastMouse.Y) * sensitivity.Y, -1.569d, 1.569d);
            camTransform.rotation = Quaternion.FromAxisAngle(Vector3.UnitX, (float)virtualMouse.Y);
            transform.rotation = Quaternion.FromAxisAngle(Vector3.UnitY, (float)virtualMouse.X);
            lastMouse = e.Position;
        }

        protected override void Update(FrameEventArgs e)
        {
            if (Input.GetKeyDown(Keys.Tab)) { Input.GrabCursor(!Input.CursorGrabbed); first = true; }
            if (!Input.CursorGrabbed) return;

            Vector3 movement = Vector3.Zero;
            if (Input.GetKey(Keys.W)) movement.Z -= 1;
            if (Input.GetKey(Keys.S)) movement.Z += 1;
            if (Input.GetKey(Keys.A)) movement.X -= 1;
            if (Input.GetKey(Keys.D)) movement.X += 1;
            if (movement.Length == 0) return;
            movement.Normalize();
            transform.position += transform.rotation * movement * Time.deltaTime * 3;
        }
    }
}
