
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;

namespace TrinityEngineProject
{
    internal class UiRenderer : Component
    {
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
                Renderer.RemoveFromUiRenderQueue(this);
                shaderIndex = value;
                Renderer.AddToUiRenderQueue(this);
            }
        }
        int _shaderIndex;

        public readonly float Width, Height;

        public UiRenderer(int shaderIndex = 0, float width = 1, float height = 1)
        {
            _shaderIndex = shaderIndex;
            Width = width;
            Height = height;
        }

        public virtual void RenderUi()
        {
        }

        internal override void OnLoad()
        {
            base.OnLoad();
            Renderer.AddToUiRenderQueue(this);
        }

        internal override void OnUnload()
        {
            base.OnUnload();
            Renderer.RemoveFromUiRenderQueue(this);
        }
    }
}
