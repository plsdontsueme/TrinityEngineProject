using OpenTK.Mathematics;
using System.ComponentModel;

namespace TrinityEngineProject
{
    /*
     * Access for:
     *  GameObject - set gameObject
     *  Renderer - GetMatrix for model
     *  User - create and assign to GameObject, read gameObject
     */

    internal class Transform
    {
        public GameObject? gameObject {  get; internal set; }

        public Vector3 position;
        public Vector3 scale;
        public Quaternion rotation;

        public Transform Copy()
        {
            return new Transform(position, scale, rotation);
        }
        public Transform(Vector3? position = null, Vector3? scale = null, Quaternion? rotation = null)
        {
            this.position = position ?? Vector3.Zero;
            this.scale = scale ?? Vector3.One;
            this.rotation = rotation ?? Quaternion.Identity;
        }

        public Matrix4 GetMatrix()
        {
            return Matrix4.CreateScale(scale) * Matrix4.CreateFromQuaternion(rotation) * Matrix4.CreateTranslation(position);
        }

        public virtual void OnLoad() { }
        public virtual void OnUnload() { }
    }
}
