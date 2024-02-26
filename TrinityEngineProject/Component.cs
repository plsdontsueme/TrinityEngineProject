
namespace TrinityEngineProject
{
    /*
     * Access for:
     *  GameObject - set gameObject, load, unload
     *  User - create and assign to GameObject, read gameObject
     */

    internal class Component
    {
        public GameObject gameObject { get; internal set; }
        public Transform transform => gameObject.transform;

        public bool loaded { get; private set; }
        internal virtual void OnLoad()
        {
            loaded = true;
        }
        internal virtual void OnUnload()
        {
            loaded = false;
        }

        public Component ShallowCopy()
        {
            return (Component)this.MemberwiseClone();
        }
    }
}
