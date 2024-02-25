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
        public Texture diffuse;

        public Material(Texture diffuse)
        {
            this.diffuse = diffuse;
        }

        public void Use()
        {
            diffuse.Use();
        }
    }
}
