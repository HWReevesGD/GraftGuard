using GraftGuard.Graphics;
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

    public override void Draw(DrawManager drawing, GameTime time, World world, InputManager inputManager, ProjectileManager manager)
    {
        float lifeFactor = 1.0f - Lifetime / MaxLifetime;
        drawing.DrawCentered(Texture, Position, scale: Vector2.One * Scale * 0.5f * lifeFactor, rotation: (time.Total() % MathF.Tau) * 3.0f);

        foreach (ProjectileZap zap in Next)
        {
            drawing.Draw(
                TLightning,
                destinationRectangle: new Rectangle(Position.ToPoint() - new Point(0, (int)(16 * lifeFactor)), new Point((int)Vector2.Distance(Position, zap.Position) + 8, (int)(32 * lifeFactor))),
                sourceRectangle: null,
                new Color(Color.White, (MaxLifetime - Lifetime) / MaxLifetime),
                rotation: (zap.Position - Position).Angle(),
                origin: new Vector2(0, 16),
                SpriteEffects.None,
                layerDepth: 0.0f);
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
