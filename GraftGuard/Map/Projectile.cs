using GraftGuard.Map.Enemies;
using GraftGuard.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using System.Security.Cryptography;
using GraftGuard.Map.Projectiles;

namespace GraftGuard.Map;

internal enum ProjectileTarget
{
    Player,
    Enemy,
}

internal class Projectile
{
    public Vector2 Position { get; set; }
    public Vector2 Velocity { get; set; }
    public float Scale { get; set; }
    public float Radius { get; set; }
    public Circle HitCircle => new Circle(Position, Radius);
    public ProjectileTarget Targeting { get; set; }
    public Texture2D Texture { get; set; }
    public Projectile(Vector2 position, float radius, Vector2 velocity, float scale, Texture2D texture, ProjectileTarget targeting)
    {
        Position = position;
        Velocity = velocity;
        Radius = radius;
        Texture = texture;
        Targeting = targeting;
        Scale = scale;
    }

    /// <summary>
    /// Returns true if the projectile is overlapping <see cref="Terrain"/>
    /// </summary>
    /// <param name="world"><see cref="World"/> to check <see cref="Terrain"/> in</param>
    /// <returns>Returns true if the projectile is overlapping <see cref="Terrain"/></returns>
    public bool IsCollidingTerrain(World world)
    {
        return world.Terrain.Overlaps(HitCircle);
    }

    /// <summary>
    /// Deals damage to the target specified in <see cref="Targeting"/> if any targets overlap <see cref="HitCircle"/>.
    /// Returns true if any target was overlapping
    /// </summary>
    /// <param name="world"><see cref="World"/> to use</param>
    /// <param name="amount">Amount of damage to deal</param>
    /// <returns>True if any target was overlapping <see cref="HitCircle"/>, false otherwise</returns>
    public bool DealDamage(World world, float amount)
    {
        switch (Targeting)
        {
            case ProjectileTarget.Player:
                bool overlapsPlayer = world.Player.Hitbox.Intersects(HitCircle);
                // TODO: Deal Damage to player?
                return overlapsPlayer;
            case ProjectileTarget.Enemy:
                bool overlapsEnemy = false;
                foreach (Enemy enemy in world.EnemyManager.Enemies)
                {
                    if (enemy.Hitbox.Intersects(HitCircle))
                    {
                        overlapsEnemy = true;
                        enemy.Health -= amount;
                    }
                }
                return overlapsEnemy;
        }
        return false;
    }

    public virtual void Update(ProjectileManager manager, GameTime time, World world, InputManager inputManager)
    {

    }

    public virtual void Draw(SpriteBatch batch, GameTime time, World world, InputManager inputManager, ProjectileManager manager)
    {
        batch.DrawCentered(Texture, Position, scale: Scale);
    }

    public static Texture2D TFire;
    public static void LoadContent(ContentManager content)
    {
        TFire = content.Load<Texture2D>("Projectile/fire");
    }
}
