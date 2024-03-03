

using OpenTK.Graphics.OpenGL4;

namespace TrinityEngineProject
{
    /*
     * Access for:
     *  MeshRenderer - Draw call
     *  User - create and assign to mesh renderers
     *  ? - Clear meshes for new scene
     */

    internal class Mesh : IDisposable
    {
        modelInformation data;

        int VertexBufferObject;
        int VertexArrayObject;
        int ElementBufferObject;

        private static Dictionary<string, Mesh> meshes = new Dictionary<string, Mesh>();
        public static void ClearMeshes()
        {
            foreach (var mesh in meshes.Values)
                mesh.Dispose();

            meshes.Clear();
        }
        public static Mesh Get(string filename)
        {
            if (meshes.ContainsKey(filename))
            {
                return meshes[filename];
            }
            else
            {
                Mesh mesh = new Mesh(filename);
                meshes.Add(filename, mesh);
                return mesh;
            }
        }
        private Mesh(string filename)
        {
            data = ImportTgd(filename);

            #region load
            VertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, data.vertexData.Length * sizeof(float), data.vertexData, BufferUsageHint.StaticDraw);

            VertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(VertexArrayObject);

            ElementBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ElementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, data.indices.Length * sizeof(uint), data.indices, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);
            #endregion
        }

        public void Draw()
        {
            GL.BindVertexArray(VertexArrayObject);
            GL.DrawElements(PrimitiveType.Triangles, data.indices.Length, DrawElementsType.UnsignedInt, 0);
        }

        static modelInformation ImportTgd(string filename)
        {
            byte[] data = File.ReadAllBytes(filename);
            float[] vert = new float[BitConverter.ToInt32(data, 0)];
            uint[] ind = new uint[BitConverter.ToInt32(data, 4)];

            int vertByteLength = vert.Length * 4;
            System.Buffer.BlockCopy(data, 8, vert, 0, vertByteLength);
            System.Buffer.BlockCopy(data, 8 + vertByteLength, ind, 0, ind.Length * 4);

            return new modelInformation() { vertexData = vert, indices = ind };
        }
        struct modelInformation
        {
            public float[] vertexData;
            public uint[] indices;
        }


        #region dispose 
        private bool disposedValue;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                #region unload
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
                GL.DeleteBuffer(VertexBufferObject);
                GL.BindVertexArray(0);
                GL.DeleteVertexArray(VertexArrayObject);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
                GL.DeleteBuffer(ElementBufferObject);
                #endregion

                data.indices = null;
                data.vertexData = null;
                disposedValue = true;
            }
        }
        ~Mesh()
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
