using OpenTK.Graphics.OpenGL4;
using StbImageSharp;

namespace TrinityEngineProject
{
    /*
     * Access for:
     *  Material - Use call
     *  User - create and assign to material
     *  ? - Clear textures for new scene
     */

    internal class Texture : IDisposable
    {
        readonly int Handle;
        public readonly int Width, Height;
        public readonly string filename;

        readonly static Dictionary<string, Texture> textures = new Dictionary<string, Texture>();
        public static void ClearTextures()
        {
            foreach (var texture in textures.Values)
                texture.Dispose();
            textures.Clear();
        }
        public static Texture Get(string filename)
        {
            if (textures.ContainsKey(filename))
            {
                return textures[filename];
            }
            else
            {
                return new Texture(filename);
            }
        }
        private Texture(string filename)
        {
            this.filename = filename;

            #region load
            Handle = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, Handle);

            StbImage.stbi_set_flip_vertically_on_load(1); //flip (stb loads from top-left pixel, OpenGL loads from  bottom-left)
            ImageResult image = ImageResult.FromStream(File.OpenRead(this.filename), ColorComponents.RedGreenBlueAlpha);
            Width = image.Width;
            Height = image.Height;

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            #endregion
        }
        public Texture(byte[] rgbaDataRaw, int width, int height, bool generateMipmaps = true)
        {
            Width = width; 
            Height = height;

            Handle = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, Handle);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, rgbaDataRaw);
            if (generateMipmaps) GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
        }

        public void Use()
        {
            GL.BindTexture(TextureTarget.Texture2D, Handle);
        }


        #region dispose
        private bool disposedValue;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                #region unload
                GL.DeleteTexture(Handle);
                #endregion

                disposedValue = true;
            }
        }
        ~Texture()
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
