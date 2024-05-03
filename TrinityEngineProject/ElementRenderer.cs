

namespace TrinityEngineProject
{
    internal class ElementRenderer : Component
    {
        public Material Material
        {
            get
            {
                return _material;
            }
            set
            {
                if (_material == value) return;
                _material.RemoveElementRenderer(this);
                _material = value;
                _material.AddElementRenderer(this);
            }
        }
        Material _material;

        public ElementRenderer(Material material)
        {
            _material = material;
        }

        public virtual void RenderElement()
        {

        }

        internal override void OnLoad()
        {
            base.OnLoad();
            _material.AddElementRenderer(this);
        }
        internal override void OnUnload()
        {
            base.OnUnload();
            _material.AddElementRenderer(this);
        }
    }
}
