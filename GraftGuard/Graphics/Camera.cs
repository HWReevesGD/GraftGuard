using Microsoft.Xna.Framework;

namespace GraftGuard.Graphics;
internal class Camera
{
    public Vector2 Position { get; set; } = Vector2.Zero;
    public Matrix ScreenToWorld => Matrix.CreateTranslation(new Vector3(Position, 0));
    public Matrix WorldToScreen => Matrix.CreateTranslation(new Vector3(-Position, 0));
}
