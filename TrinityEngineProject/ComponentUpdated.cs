using OpenTK.Windowing.Common;

namespace TrinityEngineProject
{
    /*
     * Access for:
     *  GameObject - set gameObject
     *  User - create and assign to GameObject, read gameObject
     */

    class ComponentUpdated : Component
    {
        internal override void OnLoad()
        {
            base.OnLoad();
            Input._window.UpdateFrame += Update;
        }
        internal override void OnUnload()
        {
            base.OnUnload();
            Input._window.UpdateFrame -= Update;
        }

        protected virtual void Update(FrameEventArgs e)
        {
            
        }
    }
}
