
namespace TrinityEngineProject
{
    /*
    * Access for:
    *  Renderer - Render Call
    *  User - create
    */

    internal class MeshRenderer : ElementRenderer
    {
        public Mesh mesh;

        public MeshRenderer(Mesh mesh, Material material) : base(material)
        {
            this.mesh = mesh;
        }

        public override void RenderElement()
        {
            base.RenderElement();
            mesh.Draw();
        }
    }
}
