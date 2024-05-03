
namespace TrinityEngineProject
{
    internal class UiImage : ElementRenderer
    {
        static Mesh imageMesh = Mesh.Get("...//..//..//..//..//Data//Models//quad.tgd");

        public UiImage(Material material) : base(material) { }

        public override void RenderElement()
        {
            base.RenderElement();
            imageMesh.Draw();
        }
    }
}
