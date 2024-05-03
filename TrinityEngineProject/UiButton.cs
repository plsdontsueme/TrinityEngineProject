
using OpenTK.Mathematics;

namespace TrinityEngineProject
{
    internal class UiButton : Component
    {
        public readonly MethodInvoker Subscribers = new MethodInvoker();

        public UiImage Image;

        public bool Pressed { get; private set; }

        internal override void OnLoad()
        {
            if (Image == null)
            {
                Image = gameObject.GetComponent<UiImage>();
                if (Image == null)
                {
                    TgMessage.ThrowWarning("UiButton has no assigned UiImage and couldnt detect a UiImage component");
                    return;
                }
            }

            base.OnLoad();
            Input.MouseDown += Input_MouseDown;
            Input.MouseUp += Input_MouseUp;
        }

        private void Input_MouseUp(OpenTK.Windowing.Common.MouseButtonEventArgs e)
        {
            if (!Pressed) return;

            Pressed = false;
            Image.Material.Color.Xyz *= 2f;

            Vector2 mouse = Input.UiSpaceMouse();
            float hw = 0.5f * Image.Material.Diffuse.Width;
            float hh = 0.5f * Image.Material.Diffuse.Height;

            if (transform.position.X - hw <= mouse.X &&
                mouse.X <= transform.position.X + hw &&
                transform.position.Y - hh <= mouse.Y &&
                mouse.Y <= transform.position.Y + hh)
            {
                Subscribers.InvokeAll();
            }
        }

        private void Input_MouseDown(OpenTK.Windowing.Common.MouseButtonEventArgs e)
        {
            Vector2 mouse = Input.UiSpaceMouse();
            float hw = 0.5f * Image.Material.Diffuse.Width;
            float hh = 0.5f * Image.Material.Diffuse.Height;

            if (transform.position.X - hw <= mouse.X &&
                mouse.X <= transform.position.X + hw &&
                transform.position.Y - hh <= mouse.Y &&
                mouse.Y <= transform.position.Y + hh)
            {
                Pressed = true;
                Image.Material.Color.Xyz *= 0.5f;
            }
        }

        internal override void OnUnload()
        {
            base.OnUnload();
            Input.MouseDown -= Input_MouseDown;
            Input.MouseUp -= Input_MouseUp;
        }
    }
}
