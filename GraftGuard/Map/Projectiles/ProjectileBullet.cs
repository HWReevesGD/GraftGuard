using GraftGuard.Utility;
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
    public IntervalTimer DamageTimer;
    public ProjectileBullet(Vector2 position, float direction, ProjectileTarget targeting, bool isBlueprint = false)
        : base(position, 12.0f, Vector2.Rotate(Vector2.UnitX, direction) * Speed, 1.0f, TBullet, targeting, isBlueprint)
    {
        DamageTimer = new IntervalTimer(0.1f);
    }

    public override void Draw(SpriteBatch batch, GameTime time, World world, InputManager inputManager, ProjectileManager manager)
    {
        batch.DrawCentered(Texture, Position, scale: Scale, rotation: Velocity.Angle() + MathF.PI / 2.0f);
    }

    public override void Update(ProjectileManager manager, GameTime time, World world, InputManager inputManager)
    {
        Position += Velocity * time.Delta();
        if (DamageTimer.Update(time))
        {
            DealDamage(world, 4.0f);
        }
    }
}
