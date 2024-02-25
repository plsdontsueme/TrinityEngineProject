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
        

        public static void ClearRenderGroups()
        {
            foreach (var renderGroup in shaderGroups)
            {
                renderGroup.shader.Dispose();
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

        static float rot = 0f;
        public static void RenderFrame()
        {
            foreach (var renderGroup in shaderGroups)
            {
                Matrix4 projectionMatrix = Camera.main.GetProjectionMatrix();
                renderGroup.shader.SetProjectionMatrix(ref projectionMatrix);
                Matrix4 viewMatrix = Camera.main.GetViewMatrix();
                renderGroup.shader.SetViewMatrix(ref viewMatrix);

                renderGroup.shader.Use();
                foreach(var meshRenderer in renderGroup.meshRenderers)
                {
                    Matrix4 modelMatrix = meshRenderer.transform.GetMatrix();
                    renderGroup.shader.SetModelMatrix(ref modelMatrix);

                    meshRenderer.RenderMesh();
                }
            }
        }


        class Shader : IDisposable
        {
            public readonly int Handle;
            public readonly string vertexShaderPath;
            public readonly string fragmentShaderPath;

            readonly Dictionary<string, int> uniformLocations;
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
                viewMatrixLocation = uniformLocations["view"];
                modelMatrixLocation = uniformLocations["model"];
                projectionMatrixLocation = uniformLocations["projection"];
            }

            public void Use()
            {
                GL.UseProgram(Handle);
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
        struct ShaderGroup
        {
            public Shader shader;
            public List<MeshRenderer> meshRenderers = new List<MeshRenderer>();
            public ShaderGroup(Shader shader)
            {
                this.shader = shader;
            }
        }
    }
}
