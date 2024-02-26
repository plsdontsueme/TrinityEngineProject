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
            GL.ClearColor(0.04f, 0.0f, 0.07f, 1.0f);
            IsVisible = true;

            Time.Start();

            LoadResources();
        }
        void LoadResources()
        {
            Renderer.AddShader("...//..//..//..//..//Data//Shaders//standard");

            Material zaiki = new Material(Texture.Get("...//..//..//..//..//Data//Textures//zaiki-k.png"));
            Material nier_d = new Material(Texture.Get("...//..//..//..//..//Data//Textures//Nier2b//Diffuse.jpg"));
            Mesh cube = Mesh.Get("...//..//..//..//..//Data//Models//primitive_cube.tgd");
            GameObject zaikiCube = new GameObject(new MeshRenderer(cube, zaiki), new ZaikiCubeBehaviour());

            Rnd.SetSeed(175);
            for(int i=0; i<5000; i++)
            {
                float height = Rnd.RandomRange(0.9f, 1f);
                zaikiCube.Instantiate((Rnd.RandomRange(-40f, 40f), height*0.5f + Rnd.RandomRange(-40, 40), Rnd.RandomRange(-40f, 40f)), (1, height,1));
            }
            //GameObject.Instantiate((0, -0.5f, 0), (100, 100, 1), Quaternion.FromAxisAngle(Vector3.UnitX, MathHelper.PiOver2), new MeshRenderer(cube, nier_d));

            GameObject cam = GameObject.Instantiate(position: (0, 1.75f, 0), components: [new Camera(), new AudioSource("...//..//..//..//..//Data//Audio//freed.mp3")]);
            cam.transform.parent = GameObject.Instantiate(position: (0, 0, 5), components: new PlayerBehaviour(cam.transform)).transform;
            cam.GetComponent<AudioSource>().Play();

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
