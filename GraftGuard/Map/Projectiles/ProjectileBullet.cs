using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraftGuard.Map.Projectiles;
internal class ProjectileBullet : Projectile
{
    public const float Speed = 480.0f;
    public ProjectileBullet(Vector2 position, float direction, ProjectileTarget targeting, bool isBlueprint = false)
        : base(position, 12.0f, Vector2.Rotate(Vector2.UnitX, direction) * Speed, 1.0f, TBullet, targeting, isBlueprint)
    {

    }
}
