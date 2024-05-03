using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrinityEngineProject
{
    /*
     * Access for:
     *  MeshRenderer - Use call
     *  User - create and assign to mesh renderers
     */

    internal class Material
    {
        readonly List<ElementRenderer> elementRenderers = new List<ElementRenderer>();
        public void AddElementRenderer(ElementRenderer elementRenderer)
        {
            if (elementRenderers.Contains(elementRenderer))
            {
                TgMessage.ThrowWarning("Attempt to double assign element renderer to one material");
                return;
            }
            elementRenderers.Add(elementRenderer);
        }
        public void RemoveElementRenderer(ElementRenderer elementRenderer)
        {
            if (!elementRenderers.Contains(elementRenderer))
            {
                TgMessage.ThrowWarning("Attempt to remove element renderer from material it was not assigned to");
                return;
            }
            elementRenderers.Remove(elementRenderer);
        }

        public Texture Diffuse;
        public Vector4 Color = Vector4.One;
        public Shader Shader
        {
            get
            {
                return _shader;
            }
            set
            {
                if (_shader == value) return;
                _shader.RemoveMaterial(this);
                _shader = value;
                _shader.AddMaterial(this);
            }
        }
        Shader _shader;

        public Material(Shader shader, Texture diffuse)
        {
            Diffuse = diffuse;
            _shader = shader;
            shader.AddMaterial(this);
        }

        protected bool isFont;
        public void RenderAll()
        {
            if(Shader.HasColorModifyer)
            {
                Shader.SetColorModifyer(Color);
            }

            Diffuse.Use();
            if (Shader.HasDimensionVector)
            {
                if (isFont) Shader.SetDimensionVector(1, 1);
                else Shader.SetDimensionVector(Diffuse.Width, Diffuse.Height);
            }

            Matrix4 modelMatrix;
            foreach (var elementRenderer in elementRenderers)
            {
                modelMatrix = elementRenderer.gameObject.transform.GetMatrix();
                Shader.SetModelMatrix(ref modelMatrix);
                elementRenderer.RenderElement();
            }
        }
    }
}
