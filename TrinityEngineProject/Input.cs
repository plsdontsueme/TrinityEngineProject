using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace TrinityEngineProject
{
    /*
     * Access for:
     *  TgMain - set Window
     *  User - read Input
    */

    public static class Input
    {
        internal static TgMain _window;
        public static bool GetKey() => _window.KeyboardState.IsAnyKeyDown;
        public static bool GetKey(Keys key) => _window.KeyboardState.IsKeyDown(key);
        public static bool GetKeyDown(Keys key) => _window.KeyboardState.IsKeyPressed(key);
        public static bool GetKeyUp(Keys key) => _window.KeyboardState.IsKeyReleased(key);

        public static bool GetMouse() => _window.MouseState.IsAnyButtonDown;
        public static bool GetMouse(MouseButton button) => _window.MouseState.IsButtonDown(button);
        public static bool GetMouseDown(MouseButton button) => _window.MouseState.IsButtonPressed(button);
        public static bool GetMouseUp(MouseButton button) => _window.MouseState.IsButtonReleased(button);


        public static event Action<KeyboardKeyEventArgs> KeyDown
        {
            add
            {
                _window.KeyDown += value;
            }
            remove
            {
                _window.KeyDown -= value;
            }
        }
        public static event Action<KeyboardKeyEventArgs> KeyUp
        {
            add
            {
                _window.KeyUp += value;
            }
            remove
            {
                _window.KeyUp -= value;
            }
        }

        public static event Action<MouseMoveEventArgs> MouseMove
        {
            add
            {
                _window.MouseMove += value;
            }
            remove
            {
                _window.MouseMove -= value;
            }
        }
        public static event Action<MouseButtonEventArgs> MouseDown
        {
            add
            {
                _window.MouseDown += value;
            }
            remove
            {
                _window.MouseDown -= value;
            }
        }
        public static event Action<MouseButtonEventArgs> MouseUp
        {
            add
            {
                _window.MouseUp += value;
            }
            remove
            {
                _window.MouseUp -= value;
            }
        }
        public static event Action<MouseWheelEventArgs> MouseWheel
        {
            add
            {
                _window.MouseWheel += value;
            }
            remove 
            {
                _window.MouseWheel -= value;
            }
        }
    }
}
