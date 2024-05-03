using NAudio.Wave;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace TrinityEngineProject
{
    internal class TgMain : GameWindow
    {
        public TgMain(int width, int height, string title)
            : base(GameWindowSettings.Default, new NativeWindowSettings()
            {
                ClientSize = (width, height),
                Title = title,
                StartVisible = false,
                NumberOfSamples = 4, //AntiAliasing
                WindowBorder = WindowBorder.Hidden
            })
        {
            Input._window = this;
        }

        public static float aspectRatio { get; private set; }
        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);  
            aspectRatio = (float)e.Width / e.Height;
            GL.Viewport(0, 0, e.Width, e.Height);
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            CenterWindow();
            GL.Enable(EnableCap.CullFace);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Multisample); //AntiAlising (neccessary?, see no difference)
            GL.Enable(EnableCap.Blend); // may disable for regular 3d rendering
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.ClearColor(0.04f, 0.0f, 0.07f, 1.0f);
            IsVisible = true;

            Time.Start();

            LoadResources();
        }
        void LoadResources()
        {
            Shader def = new Shader("...//..//..//..//..//Data//Shaders//standard");
            Shader ui = new Shader("...//..//..//..//..//Data//Shaders//ui", true, DepthFunction.Always, 1);

            GameObject nier = new GameObject(
                new MeshRenderer(Mesh.Get("...//..//..//..//..//Data//Models//Nier2b.tgd"), 
                    new Material(def, Texture.Get("...//..//..//..//..//Data//Textures//Nier2b//Diffuse.jpg"))
                ));
            new GameObject(
                new MeshRenderer(Mesh.Get("...//..//..//..//..//Data//Models//Nier2b_Hair.tgd"),
                    new Material(def,Texture.Get("...//..//..//..//..//Data//Textures//Nier2b//HairD.jpg"))
                )).transform.parent = nier.transform;

            nier.Instantiate();
           
            GameObject uiTextBox = GameObject.Instantiate(new UiTextBox(), new UiImage(new Material(ui, Texture.Get("...//..//..//..//..//Data//Textures//tanya.png"))));
            Font font = Font.Create(ui, "...//..//..//..//..//Data//Fonts//arial.ttf", 1000, 1000, 100, characterRanges: [Font.CharacterRange.BasicLatin, Font.CharacterRange.Latin1Supplement]);
            GameObject uiText = GameObject.Instantiate(new UiText(font, "double" + Environment.NewLine + "lines" + Environment.NewLine));
            uiText.transform.parent = uiTextBox.transform;
            uiTextBox.GetComponent<UiTextBox>().Text = uiText.GetComponent<UiText>();

            GameObject cam = GameObject.Instantiate(position: (0, 1.75f, 0), components: [new Camera()]);
            cam.transform.parent = GameObject.Instantiate(position: (0, 0, 10), components: new PlayerBehaviour(cam.transform)).transform;
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            Time.deltaTime = (float)e.Time;
            if (KeyboardState.IsKeyDown(Keys.Escape))
            {
                Close();
            }

            base.OnUpdateFrame(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);


            //render frame
            Shader.RenderAll();


            SwapBuffers();
        }
    }
}
