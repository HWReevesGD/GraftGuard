using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GraftGuard.Map;
internal class Projectile : GameObject
{
    public Projectile(Vector2 position, Vector2 size, Texture2D texture) : base(position, size, texture)
    {
    }
}
