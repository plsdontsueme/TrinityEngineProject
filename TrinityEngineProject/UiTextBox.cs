using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace TrinityEngineProject
{
    internal class UiTextBox : Component
    {
        public UiImage Image;
        public UiText Text;

        public bool Selected { get; private set; }

        internal override void OnLoad()
        {
            //double check when onload is called
            //remove when interface is ready
            if (Image == null)
            {
                Image = gameObject.GetComponent<UiImage>();
                if (Image == null)
                {
                    TgMessage.ThrowWarning("UiTextBox has no assigned UiImage and couldnt detect a UiImage-Component");
                    return;
                }
            }

            base.OnLoad();
            Input.MouseDown += Input_MouseDown;
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
                if (!Selected)
                {
                    Selected = true;
                    Image.Material.Color.Xyz *= 0.5f;
                    Input.TextInput += Input_TextInput;
                    Input.KeyDown += Input_KeyDown;
                }
            }
            else
            {
                if (Selected)
                {
                    Selected = false;
                    Image.Material.Color.Xyz *= 2f;
                    Input.TextInput -= Input_TextInput;
                    Input.KeyDown -= Input_KeyDown;
                }
            }
        }

        private void Input_TextInput(OpenTK.Windowing.Common.TextInputEventArgs e)
        {
            Text.Text += e.AsString;
        }

        private void Input_KeyDown(OpenTK.Windowing.Common.KeyboardKeyEventArgs e)
        {
            switch(e.Key)
            {
                case Keys.Backspace:
                    if (Text.Text.EndsWith(Environment.NewLine)) Text.Text = Text.Text.Substring(0, Text.Text.Length - Environment.NewLine.Length);
                    else Text.Text = Text.Text.Substring(0, Text.Text.Length - 1);
                    break;
                case Keys.Enter:
                    Text.Text += Environment.NewLine;
                    break;
            }
        }

        internal override void OnUnload()
        {
            base.OnUnload();
            Input.MouseDown -= Input_MouseDown;
        }
    }
}
