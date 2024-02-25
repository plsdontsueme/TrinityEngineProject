
namespace TrinityEngineProject
{
    /*
     * Access for:
     *  GameObject - set gameObject
     *  User - create and assign to GameObject, read gameObject
     */

    internal class Component
    {
        public GameObject gameObject { get; internal set; }
        public Transform transform => gameObject.transform;

        public bool loadel { get; private set; }
        public virtual void OnLoad()
        {
            loadel = true;
        }
        public virtual void OnUnload()
        {
            loadel = false;
        }

        public Component ShallowCopy()
        {
            return (Component)this.MemberwiseClone();
        }
    }
}
