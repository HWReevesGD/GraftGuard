using GraftGuard.Map;
using Microsoft.Xna.Framework;

namespace GraftGuard.Graphics;
internal class Camera
{
    public Vector2 Position { get; set; } = Vector2.Zero;
    public Matrix ScreenToWorld => Matrix.CreateTranslation(new Vector3(Position, 0));
    public Matrix WorldToScreen => Matrix.CreateTranslation(new Vector3(-Position, 0));
    public void UpdateFreeMovement(GameTime time, InputManager input)
    {
        Vector2 movement = input.GetMovementDirection() * Player.Speed;
        Position += movement * (float)time.ElapsedGameTime.TotalSeconds;
    }
}
