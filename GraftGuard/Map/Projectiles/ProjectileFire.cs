using GraftGuard.Graphics;
using GraftGuard.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraftGuard.Map.Projectiles;

internal class ProjectileFire : Projectile
{
    public const float FireSpeed = 256.0f;
    public const float FireRadius = 16.0f;
    public ProjectileDamage Damage = new ProjectileDamage(1.0f, 0, 0, 0, 0);

    private const float FullLifetime = 0.5f;
    private float _lifetime = 0.0f;
    private float _speedModifier;
    private float _lifetimeModifier;
    private IntervalTimer DamageInterval = new IntervalTimer(0.2f);

    public ProjectileFire(Vector2 position, float scale, float direction, ProjectileTarget targeting, float speedModifier = 1.0f, float lifetimeModifier = 1.0f, bool isBlueprint = false)
        : base(position, FireRadius, new Vector2(MathF.Cos(direction), MathF.Sin(direction)) * FireSpeed * speedModifier, scale, TFire, targeting, isBlueprint)
    {
        _speedModifier = speedModifier;
        _lifetimeModifier = lifetimeModifier;
    }

    public override void Update(ProjectileManager manager, GameTime time, World world, InputManager inputManager)
    {
        _lifetime += (float)time.ElapsedGameTime.TotalSeconds;

        if (_lifetime >= FullLifetime * _lifetimeModifier)
        {
            manager.Remove(this);
        }

        Position += Velocity * time.Delta() * Scale;

        bool dealDamage = DamageInterval.Update(time);
        if (dealDamage)
        {
            DealDamage(world, Damage);
        }
    }

    public override void Draw(DrawManager drawing, GameTime time, World world, InputManager inputManager, ProjectileManager manager)
    {
        float lifetimeFactor = _lifetime / (FullLifetime * _lifetimeModifier);
        drawing.DrawCentered(Texture, Position, scale: Vector2.One * (MathF.Log(_lifetime + 1.0f) * Scale), rotation: Velocity.Angle(),
            color: new Color(Color.White, (1.0f - lifetimeFactor) * 4.0f));
    }
}
