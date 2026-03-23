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

    // Drag and Drop Properties
    public bool IsDragging { get; private set; }
    private Vector2 dragStartPosition;
    private const float DragThreshold = 5f;

    public InputManager() { }

    public void Update(Camera camera = null)
    {
        if (camera is not null)
        {
            _currentScreenToWorld = camera.ScreenToWorld;
        }

        prevKeyState = currentKeyState;
        prevMouseState = currentMouseState;

        currentKeyState = Keyboard.GetState();
        currentMouseState = Mouse.GetState();

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
