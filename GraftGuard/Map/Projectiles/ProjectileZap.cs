using GraftGuard.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GraftGuard.Map.Projectiles;
internal class ProjectileZap : Projectile
{
    public List<ProjectileZap> Next;
    public float Lifetime = 0.0f;
    public const float MaxLifetime = 0.5f;
    public ProjectileZap(Vector2 position, ProjectileTarget targeting, bool isBlueprint = false)
        : base(position,
               radius: 32.0f,
               velocity: Vector2.Zero,
               scale: 1.0f,
               texture: TZap,
               targeting: targeting, isBlueprint: isBlueprint)
    {
        Next = [];
    }

    public override void Draw(SpriteBatch batch, GameTime time, World world, InputManager inputManager, ProjectileManager manager)
    {
        base.Draw(batch, time, world, inputManager, manager);
        
        foreach (ProjectileZap zap in Next)
        {
            batch.Draw(
                TLightning,
                Position,
                sourceRectangle: null,
                new Color(Color.White, (MaxLifetime - Lifetime) / MaxLifetime),
                rotation: (zap.Position - Position).Angle(),
                origin: new Vector2(0, 16),
                scale: 1.0f,
                SpriteEffects.None,
                layerDepth: 1.0f);
        }
    }

    public override void Update(ProjectileManager manager, GameTime time, World world, InputManager inputManager)
    {
        Lifetime += time.Delta();

        if (Lifetime > MaxLifetime)
        {
            manager.Remove(this);
        }
    }

}
