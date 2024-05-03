using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;

namespace TrinityEngineProject
{
    class Shader : IDisposable
    {
        readonly int Handle;
        DepthFunction depthFunc = DepthFunction.Less;

        readonly string shaderPath;

        readonly Dictionary<string, int> uniformLocations;
        readonly int dimensionVectorLocation;
        public readonly bool HasDimensionVector;
        readonly int? viewMatrixLocation;
        readonly int modelMatrixLocation;
        readonly int projectionMatrixLocation;

        readonly int colorLocation;
        public readonly bool HasColorModifyer;

        readonly List<Material> materials = new List<Material>();
        public void AddMaterial(Material material)
        {
            if (materials.Contains(material))
            {
                TgMessage.ThrowWarning("Attempt to double assign material to one shader");
                return;
            }
            materials.Add(material);
        }
        public void RemoveMaterial(Material material)
        {
            if (!materials.Contains(material))
            {
                TgMessage.ThrowWarning("Attempt to remove material from shader it was not assigned to");
            }
            materials.Remove(material);
        }

        static readonly List<Shader> shaders = new List<Shader>();
        public int? RenderLayer { get; private set; } = null;
        public bool UiCamera { get; private set; }
        //public bool Is2D { get; private set; } = false;

        public Shader(string shaderPath, bool useUiCamera = false,DepthFunction depthFunc = DepthFunction.Less,int renderLayer = 0)
        {
            this.shaderPath = shaderPath;
            #region load
            string VertexShaderSource = File.ReadAllText(Path.Combine(shaderPath, "vertex.glsl"));
            string FragmentShaderSource = File.ReadAllText(Path.Combine(shaderPath, "fragment.glsl"));

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
            if (uniformLocations.ContainsKey("dimensions"))
            {
                HasDimensionVector = true;
                dimensionVectorLocation = uniformLocations["dimensions"];
            }
            viewMatrixLocation = uniformLocations.ContainsKey("view") ? uniformLocations["view"] : null;
            modelMatrixLocation = uniformLocations["model"];
            projectionMatrixLocation = uniformLocations["projection"];

            if (uniformLocations.ContainsKey("color"))
            {
                HasColorModifyer = true;
                colorLocation = uniformLocations["color"];
            }

            /*
            if (dimensionVectorLocation.HasValue || viewMatrixLocation.HasValue)
            {
                if (dimensionVectorLocation.HasValue && viewMatrixLocation.HasValue)
                    Is2D = true;
                else
                    TgMessage.ThrowWarning("Shader layout not supported: hasDimensions and hasView must be equal");
            }*/
            this.depthFunc = depthFunc;
            UiCamera = useUiCamera;
            SetRenderLayer(renderLayer);
        }

        public void SetRenderLayer(int? layer)
        {
            if (RenderLayer.HasValue) shaders.Remove(this);
            if (!layer.HasValue) return;

            RenderLayer = layer;
            int lastIndex = shaders.Count - 1;
            if (lastIndex < 0 || shaders.ElementAt(lastIndex).RenderLayer <= layer) shaders.Add(this);

            for (int i = lastIndex-1; i >= 0; i--)
            {
                if (shaders.ElementAt(i).RenderLayer <= layer) shaders.Insert(i + 1, this);
            }
        }

        public void Use()
        {
            GL.UseProgram(Handle);
        }
        public void Render(ref Matrix4 projectionMatrix, ref Matrix4 viewMatrix)
        {
            GL.DepthFunc(depthFunc);
            Use();
            SetProjectionMatrix(ref projectionMatrix);
            if (viewMatrixLocation.HasValue) SetViewMatrix(ref viewMatrix);

            foreach (var material in materials)
                material.RenderAll();
        }

        public static void RenderAll()
        {
            Matrix4 projectionMatrix = Camera.main.GetProjectionMatrix();
            Matrix4 viewMatrix = Camera.main.GetViewMatrix();
            Matrix4 projectionMatrixUI = Camera.GerUiProjectionMatrix();
            foreach (var shader in shaders)
            {
                if (shader.UiCamera) shader.Render(ref projectionMatrixUI, ref viewMatrix);
                else shader.Render(ref projectionMatrix, ref viewMatrix);
            }
        }


        public void SetDimensionVector(float width, float height)
        {
            GL.Uniform2(dimensionVectorLocation, width, height);
        }
        public void SetModelMatrix(ref Matrix4 matrix)
        {
            GL.UniformMatrix4(modelMatrixLocation, true, ref matrix);
        }
        void SetViewMatrix(ref Matrix4 matrix)
        {
            GL.UniformMatrix4(viewMatrixLocation.Value, true, ref matrix);
        }
        void SetProjectionMatrix(ref Matrix4 matrix)
        {
            GL.UniformMatrix4(projectionMatrixLocation, true, ref matrix);
        }

        public void SetColorModifyer(Vector4 color)
        {
            GL.Uniform4(colorLocation, color);
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
