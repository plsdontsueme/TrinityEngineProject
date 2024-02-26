using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Common.Input;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Reflection;

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
            GL.ClearColor(0.04f, 0.0f, 0.07f, 1.0f);
            IsVisible = true;

            Time.Start();

            LoadResources();
        }
        void LoadResources()
        {
            Renderer.AddShader("...//..//..//..//..//Data//Shaders//standard");
            GameObject.Instantiate(new Camera(), new PlayerBehaviour()).transform.position = (0, 0, 20);
            Material material = new Material(Texture.Get("...//..//..//..//..//Data//Textures//Nier2b//Diffuse.jpg"));
            GameObject nierPrefab = new GameObject(new MeshRenderer(Mesh.Get("...//..//..//..//..//Data//Models//Nier2b.tgd"), material), new ObjectSpinner());
            Material material1 = new Material(Texture.Get("...//..//..//..//..//Data//Textures//Nier2b//HairD.jpg"));
            GameObject nierHairprefab = new GameObject(new MeshRenderer(Mesh.Get("...//..//..//..//..//Data//Models//Nier2b_1.tgd"), material1));
            nierHairprefab.transform.parent = nierPrefab.transform;

            nierPrefab.Instantiate();

            CursorState = CursorState.Grabbed;
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


            //render UI
            Renderer.RenderFrame();


            SwapBuffers();
        }
    }
}
