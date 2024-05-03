using OpenTK.Mathematics;
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

        public static Vector2 MousePosition => _window.MousePosition;

        public static bool CursorGrabbed { get; private set; }
        public static void GrabCursor(bool state = true)
        {
            if (state) _window.CursorState = CursorState.Grabbed;
            else _window.CursorState = CursorState.Normal;

            CursorGrabbed = state;
        }

        public static event Action<TextInputEventArgs> TextInput
        {
            add
            {
                _window.TextInput += value;
            }
            remove
            {
                _window.TextInput -= value;
            }
        }

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


        public static Vector2 UiSpaceMouse()
        {
            return ScreenMouseToUi(MousePosition);
        }
        public static Vector2 ScreenMouseToUi(Vector2 screenMouse)
        {
            double xunitsize = TgMain.aspectRatio * 0.5d * Camera.UiGridDivisions;
            double x = (double)screenMouse.X / _window.Size.X * Camera.UiGridDivisions * TgMain.aspectRatio - xunitsize;
            double y = (double)screenMouse.Y / _window.Size.Y * Camera.UiGridDivisions - 0.5d * Camera.UiGridDivisions;
            return new Vector2((float)x, -(float)y);
        }
    }
}
