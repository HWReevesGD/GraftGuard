using Microsoft.Xna.Framework;

namespace GraftGuard.Map;
public struct PartTransform
{
    public PartTransform() { }
    public Vector2 Position = Vector2.Zero;
    public float Rotation = 0.0f;
    public Vector2 Origin = Vector2.Zero;
    public Vector2 Scale = Vector2.One;
}
