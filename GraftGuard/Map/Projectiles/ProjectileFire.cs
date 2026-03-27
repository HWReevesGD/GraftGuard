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

    private const float FullLifetime = 0.5f;
    private float _lifetime = 0.0f;
    private float _speedModifier;
    private float _lifetimeModifier;

    public ProjectileFire(Vector2 position, float direction, ProjectileTarget targeting, float speedModifier = 1.0f, float lifetimeModifier = 1.0f)
        : base(position, FireRadius, new Vector2(MathF.Cos(direction), MathF.Sin(direction)) * FireSpeed * speedModifier, TFire, targeting)
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

        Position += Velocity * time.Delta();
    }

    public override void Draw(SpriteBatch batch, GameTime time, World world, InputManager inputManager, ProjectileManager manager)
    {
        batch.DrawCentered(Texture, Position, scale: MathF.Log(_lifetime + 1.0f), rotation: Velocity.Angle(),
            color: new Color(Color.White, MathF.Max(0.0f, 0.2f - _lifetime / (FullLifetime * _lifetimeModifier))));
    }
}
