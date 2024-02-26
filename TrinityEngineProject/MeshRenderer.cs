
namespace TrinityEngineProject
{
    /*
    * Access for:
    *  Renderer - Render Call
    *  User - create
    */

    internal class MeshRenderer : Component
    {
        public Mesh mesh;
        public Material material;
        public int shaderIndex
        {
            get
            {
                return _shaderIndex;
            }
            set 
            {
                if (_shaderIndex == value) return;

                _shaderIndex = value;
                Renderer.RemoveFromRenderQueue(this);
                shaderIndex = value;
                Renderer.AddToRenderQueue(this);
            }
        }
        int _shaderIndex;

        public MeshRenderer(Mesh mesh, Material material, int shaderIndex = 0)
        {
            this.mesh = mesh;
            this.material = material;
            _shaderIndex = shaderIndex;
        }

        public void RenderMesh()
        {
            material.Use();
            mesh.Draw();
        }

        internal override void OnLoad()
        {
            base.OnLoad();
            Renderer.AddToRenderQueue(this);
        }
        internal override void OnUnload()
        {
            base.OnUnload();
            Renderer.RemoveFromRenderQueue(this);
        }
    }
}
