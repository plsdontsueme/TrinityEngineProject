
namespace TrinityEngineProject
{
    internal class UiButton : Component
    {
        public readonly MethodInvoker Subscribers = new MethodInvoker();

        internal override void OnLoad()
        {
            base.OnLoad();
            Input.MouseDown += Input_MouseDown;
        }

        internal override void OnUnload()
        {
            base.OnUnload();
            Input.MouseDown -= Input_MouseDown;
        }

        private void Input_MouseDown(OpenTK.Windowing.Common.MouseButtonEventArgs e)
        {
            //check for cick in hitbox
            Subscribers.InvokeAll();
        }
    }
}
