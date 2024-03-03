using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.Reflection.Metadata;

namespace TrinityEngineProject
{
    /*
     * Access for:
     *  GlMain - Render call
     *  MeshRenderer - Add self to render list
     *  ? - Clear for new scene
     */

    internal static class Renderer
    {
        static List<ShaderGroup> shaderGroups = new List<ShaderGroup>();
        public static void ClearShaderGroups()
        {
            foreach (var shaderGroup in shaderGroups)
            {
                shaderGroup.shader.Dispose();
            }
            shaderGroups.Clear();
        }
        public static void AddShader(string shaderDirectory)
        {
            Shader shader = new Shader(Path.Combine(shaderDirectory, "vertex.glsl"), Path.Combine(shaderDirectory, "fragment.glsl"));
            shaderGroups.Add(new ShaderGroup(shader));
        }

        public static void AddToRenderQueue(MeshRenderer meshRenderer)
        {
            shaderGroups[meshRenderer.shaderIndex].meshRenderers.Add(meshRenderer);
        }
        public static void RemoveFromRenderQueue(MeshRenderer meshRenderer)
        {
            shaderGroups[meshRenderer.shaderIndex].meshRenderers.Remove(meshRenderer);
        }


        static List<UiShaderGroup> uiShaderGroups = new List<UiShaderGroup>();
        public static void ClearUiRenderGroups()
        {
            foreach (var shaderGroup in uiShaderGroups)
            {
                shaderGroup.shader.Dispose();
            }
            uiShaderGroups.Clear();
        }
        public static void AddUiShader(string shaderDirectory)
        {
            Shader shader = new Shader(Path.Combine(shaderDirectory, "vertex.glsl"), Path.Combine(shaderDirectory, "fragment.glsl"));
            uiShaderGroups.Add(new UiShaderGroup(shader));
        }

        public static void AddToUiRenderQueue(UiRenderer uiRenderer)
        {
            uiShaderGroups[uiRenderer.shaderIndex].uiRenderers.Add(uiRenderer);
        }
        public static void RemoveFromUiRenderQueue(UiRenderer uiRenderer)
        {
            uiShaderGroups[uiRenderer.shaderIndex].uiRenderers.Remove(uiRenderer);
        }



        public static void RenderFrame()
        {
            GL.DepthFunc(DepthFunction.Less);
            Matrix4 projectionMatrix = Camera.main.GetProjectionMatrix();
            Matrix4 viewMatrix = Camera.main.GetViewMatrix();
            foreach (var renderGroup in shaderGroups)
            {
                renderGroup.shader.Use();
                renderGroup.shader.SetProjectionMatrix(ref projectionMatrix);
                renderGroup.shader.SetViewMatrix(ref viewMatrix);
                foreach (var meshRenderer in renderGroup.meshRenderers)
                {
                    Matrix4 modelMatrix = meshRenderer.transform.GetMatrix();
                    renderGroup.shader.SetModelMatrix(ref modelMatrix);

                    meshRenderer.RenderMesh();
                }
            }

            //rendered last because of transparency
            GL.DepthFunc(DepthFunction.Always);
            projectionMatrix = Matrix4.CreateOrthographic(1000f * TgMain.aspectRatio, 1000f, 0, 0.01f);
            foreach (var renderGroup in uiShaderGroups)
            {
                renderGroup.shader.Use();
                renderGroup.shader.SetProjectionMatrix(ref projectionMatrix);
                foreach (var uiRenderer in renderGroup.uiRenderers)
                {
                    renderGroup.shader.setDimensionVector(uiRenderer.Width, uiRenderer.Height);
                    Matrix4 modelMatrix = uiRenderer.transform.GetMatrix();
                    renderGroup.shader.SetModelMatrix(ref modelMatrix);

                    uiRenderer.RenderUi();
                }
            }
        }

        struct UiShaderGroup
        {
            public Shader shader;
            public List<UiRenderer> uiRenderers = new List<UiRenderer>();
            public UiShaderGroup(Shader shader)
            {
                this.shader = shader;
            }
        }
        struct ShaderGroup
        {
            public Shader shader;
            public List<MeshRenderer> meshRenderers = new List<MeshRenderer>();
            public ShaderGroup(Shader shader)
            {
                this.shader = shader;
            }
        }

        class Shader : IDisposable
        {
            public readonly int Handle;
            public readonly string vertexShaderPath;
            public readonly string fragmentShaderPath;

            readonly Dictionary<string, int> uniformLocations;
            readonly int dimensionVectorLocation;
            readonly int viewMatrixLocation;
            readonly int modelMatrixLocation;
            readonly int projectionMatrixLocation;

            public Shader(string vertexShaderPath, string fragmentShaderPath)
            {
                this.vertexShaderPath = vertexShaderPath;
                this.fragmentShaderPath = fragmentShaderPath;
                #region load
                string VertexShaderSource = File.ReadAllText(vertexShaderPath);
                string FragmentShaderSource = File.ReadAllText(fragmentShaderPath);

                int VertexShader = GL.CreateShader(ShaderType.VertexShader);
                GL.ShaderSource(VertexShader, VertexShaderSource);

                int FragmentShader = GL.CreateShader(ShaderType.FragmentShader);
                GL.ShaderSource(FragmentShader, FragmentShaderSource);

                GL.CompileShader(VertexShader);

                GL.GetShader(VertexShader, ShaderParameter.CompileStatus, out int successV);
                if (successV == 0)
                {
                    string infoLog = GL.GetShaderInfoLog(VertexShader);
                    TgMessage.ThrowWarning($"Error compiling Vertex Shader: {Environment.NewLine}{infoLog}");
                }

                GL.CompileShader(FragmentShader);

                GL.GetShader(FragmentShader, ShaderParameter.CompileStatus, out int successF);
                if (successF == 0)
                {
                    string infoLog = GL.GetShaderInfoLog(FragmentShader);
                    TgMessage.ThrowWarning($"Error compiling Fragment Shader: {Environment.NewLine}{infoLog}");
                }

                Handle = GL.CreateProgram();

                GL.AttachShader(Handle, VertexShader);
                GL.AttachShader(Handle, FragmentShader);

                GL.LinkProgram(Handle);

                GL.GetProgram(Handle, GetProgramParameterName.LinkStatus, out int success);
                if (success == 0)
                {
                    string infoLog = GL.GetProgramInfoLog(Handle);
                    TgMessage.ThrowWarning($"Error compiling Shader Program: {Environment.NewLine}{infoLog}");
                }

                //cleanup
                GL.DetachShader(Handle, VertexShader);
                GL.DetachShader(Handle, FragmentShader);
                GL.DeleteShader(FragmentShader);
                GL.DeleteShader(VertexShader);
                #endregion

                uniformLocations = new Dictionary<string, int>();
                GL.GetProgram(Handle, GetProgramParameterName.ActiveUniforms, out int uniformCount);
                for (int i = 0; i < uniformCount; i++)
                {
                    string key = GL.GetActiveUniform(Handle, i, out _, out _);
                    int location = GL.GetUniformLocation(Handle, key);
                    uniformLocations.Add(key, location);
                }
                //no view if ui Shader
                if (uniformLocations.ContainsKey("dimensions")) dimensionVectorLocation = uniformLocations["dimensions"];
                if (uniformLocations.ContainsKey("view")) viewMatrixLocation = uniformLocations["view"];
                modelMatrixLocation = uniformLocations["model"];
                projectionMatrixLocation = uniformLocations["projection"];
            }

            public void Use()
            {
                GL.UseProgram(Handle);
            }
            
            public void setDimensionVector(float width, float height)
            {
                GL.Uniform2(dimensionVectorLocation, width, height);
            }
            public void SetViewMatrix(ref Matrix4 matrix)
            {
                GL.UniformMatrix4(viewMatrixLocation, true, ref matrix);
            }
            public void SetModelMatrix(ref Matrix4 matrix)
            {
                GL.UniformMatrix4(modelMatrixLocation, true, ref matrix);
            }
            public void SetProjectionMatrix(ref Matrix4 matrix)
            {
                GL.UniformMatrix4(projectionMatrixLocation, true, ref matrix);
            }

            #region dispose
            bool disposedValue;
            protected virtual void Dispose(bool disposing)
            {
                if (!disposedValue)
                {
                    GL.DeleteProgram(Handle);

                    disposedValue = true;
                }
            }
            ~Shader()
            {
                if (disposedValue == false)
                {
                    TgMessage.ThrowWarning("GPU Resource leak - Dispose wasnt called 0o0");
                }
            }
            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }
            #endregion
        }
    }
}
