
namespace TrinityEngineProject
{
    internal class UiImage : UiRenderer
    {
        public Texture texture;
        static Mesh imageMesh = Mesh.Get("...//..//..//..//..//Data//Models//quad.tgd");

        public UiImage(Texture texture, int shaderIndex = 0) : base(shaderIndex, texture.Width, texture.Height)
        {
            this.texture = texture;
        }

        public override void RenderUi()
        {
            texture.Use();
            imageMesh.Draw();
        }
    }
}
