using GraftGuard.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

namespace GraftGuard;
internal class InputManager
{
    // States
    private KeyboardState currentKeyState;
    private KeyboardState prevKeyState;
    private MouseState currentMouseState;
    private MouseState prevMouseState;
    private Matrix _currentScreenToWorld = Matrix.Identity;

    public MouseState CurrentMouse => currentMouseState;

    public Matrix ResolutionScaleMatrix { get; set; } = Matrix.Identity;

    // Drag and Drop Properties
    public bool IsDragging { get; private set; }
    private Vector2 dragStartPosition;
    private const float DragThreshold = 5f;

    // Zoom settings
    private const float ZoomSensitivity = 0.001f;

    public InputManager() { }

    public void Update(Camera camera = null)
    {

        prevKeyState = currentKeyState;
        prevMouseState = currentMouseState;

        currentKeyState = Keyboard.GetState();

        MouseState rawMouseState = Mouse.GetState();

        // transform the raw position into your virtual coordinate space for scaling
        Vector2 virtualPos = Vector2.Transform(
            rawMouseState.Position.ToVector2(),
            Matrix.Invert(ResolutionScaleMatrix)
        );

        // overwrite currentMouseState with the virtual version
        currentMouseState = new MouseState(
            (int)virtualPos.X,
            (int)virtualPos.Y,
            rawMouseState.ScrollWheelValue,
            rawMouseState.LeftButton,
            rawMouseState.MiddleButton,
            rawMouseState.RightButton,
            rawMouseState.XButton1,
            rawMouseState.XButton2
        );

        if (camera is not null)
        {
            _currentScreenToWorld = camera.ScreenToWorld;

            int scrollDelta = GetScrollDelta();
            if (scrollDelta != 0)
            {
                camera.AdjustZoom(scrollDelta * ZoomSensitivity);
            }
        }

        UpdateDragLogic();
    }

    #region Keyboard Helpers

    public bool IsKeyDown(Keys key) => currentKeyState.IsKeyDown(key);

    public bool WasKeyPressStarted(Keys key) => currentKeyState.IsKeyDown(key) && !prevKeyState.IsKeyDown(key);

    public Vector2 GetMovementDirection()
    {
        Vector2 direction = Vector2.Zero;
        if (currentKeyState.IsKeyDown(Keys.W)) direction.Y -= 1;
        if (currentKeyState.IsKeyDown(Keys.S)) direction.Y += 1;
        if (currentKeyState.IsKeyDown(Keys.A)) direction.X -= 1;
        if (currentKeyState.IsKeyDown(Keys.D)) direction.X += 1;

        if (direction != Vector2.Zero) direction.Normalize();
        
        return direction;
    }

    #endregion

    #region Mouse Helpers

    public int GetScrollDelta() => currentMouseState.ScrollWheelValue - prevMouseState.ScrollWheelValue;

    public Point MouseScreenPosition => currentMouseState.Position;
    public Point MouseWorldPosition => Vector2.Transform(MouseScreenPosition.ToVector2(), _currentScreenToWorld).ToPoint();

    public bool LeftMouseClicked() =>
        currentMouseState.LeftButton == ButtonState.Pressed &&
        prevMouseState.LeftButton == ButtonState.Released;

    public bool LeftMouseHeld() => currentMouseState.LeftButton == ButtonState.Pressed;

    private void UpdateDragLogic()
    {
        if (LeftMouseClicked())
        {
            dragStartPosition = currentMouseState.Position.ToVector2();
        }

        if (LeftMouseHeld())
        {
            float distance = Vector2.Distance(dragStartPosition, currentMouseState.Position.ToVector2());
            if (distance > DragThreshold)
            {
                IsDragging = true;
            }
        }
        else
        {
            IsDragging = false;
        }
    }

    #endregion
}
