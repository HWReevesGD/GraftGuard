using GraftGuard.Grafting;
using GraftGuard.Grafting.Registry;
using GraftGuard.Grafting.Registry.Behaviors;
using GraftGuard.Map.Enemies.Animation;
using GraftGuard.Map.Pathing;
using GraftGuard.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using static GraftGuard.Map.Enemies.EnemyVisual;
using static GraftGuard.Map.Pathing.PathManager;

namespace GraftGuard.Map.Enemies;
internal abstract class Enemy : GameObject
{
    double timer = 0;
    public static readonly float NearDistanceSquared = MathF.Pow(32.0f, 2.0f);

    // Fields
    private Vector2 dirUnitVec;
    private float speed;

    // Fields for speed modification
    private float speedMod;
    private int speedModDuration;

    // Fields for Damage over time
    private float damageOverTime;
    private int damageOverTimeDuration;

    public float Health { get; set; }
    public bool IsDead { get; private set; } = false;
    public EnemyVisual Visual { get; private set; }

    public List<PathNode> Path { get; set; } = [];
    public IntervalTimer PathTimer { get; set; }
    public Vector2 Velocity { get; set; }
    /// <summary>
    /// Mass Affects how fast an Enemy can change direction
    /// </summary>
    public float Mass { get; set; } = 2.0f;

    public Enemy(Vector2 position, BaseDefinition torso, Vector2 hitboxSize, float health, float speed)
        : base(position, hitboxSize, torso.Texture, collisionLayers: CollisionLayer.Enemy)
    {
        this.Health = health;
        this.speed = speed;
        dirUnitVec = new Vector2();

        speedMod = 0;
        speedModDuration = 0;
        damageOverTime = 0;
        damageOverTimeDuration = 0;

        // Attachment points logic could also be moved to a Factory or Registry later
        //SetupDefaultAttachmentPoints(torso);


        // Initialize the visual component
        Visual = new EnemyVisual(torso, 1f, AnimationClips.Idle, position);
    }

    public virtual void OnDeath()
    {
        if (IsDead) return;

        IsDead = true;
        Visual.VisualDeath(Position);
    }

    public void TakeDamage(float amount)
    {
        System.Diagnostics.Debug.WriteLine("Damaged for!: " + amount);
        Health -= amount;
    }

    public virtual void Update(GameTime gameTime, InputManager inputManager, World world, PathManager pathManager)
    {
        // code for processing DoT and speed modifier status effects
        // increment timer
        timer += gameTime.ElapsedGameTime.TotalSeconds;

        // If a second passed
        if (timer >= 1)
        {
            // process speed modifier duration
            if (speedModDuration > 0)
                speedMod--;

            // process DoT
            if (damageOverTimeDuration > 0)
            {
                Health -= damageOverTime;
                damageOverTimeDuration--;
            }

            // reset timer
            timer = 0;
        }
        

        if (!IsDead)
        {

            Player player = world.Player;

            if (Hitbox.Intersects(player.Hitbox))
            {
                // Trigger damage and knockback
                player.TakeDamage(Position, 1, 50f);
            }

        }

        // Update attached parts
        int index = 0;
        foreach (AttachedPart part in Visual.AttachedParts)
        {
            PartTransform transform = Visual.GetPartTransform(part, Position, index++);
            part.UpdateBehavior(
                settings: PartSettings.DefaultEnemy,
                transform: transform,
                time: gameTime,
                inputManager: inputManager,
                state: Data.TimeState.Night,
                world: world,
                projectileManager: world.ProjectileManager);
        }

        Visual.Update(gameTime, Position);

        if (!IsDead)
        {
            UpdatePathing(gameTime, inputManager, world, pathManager);
        }
    }

    public abstract void UpdatePathing(GameTime gameTime, InputManager inputManager, World world, PathManager pathManager);

    /// <summary>
    /// Returns a steering velocity from basic pathing logic
    /// </summary>
    /// <param name="gameTime"></param>
    /// <param name="world"></param>
    /// <param name="pathManager"></param>
    /// <returns></returns>
    public Vector2 BasicGaragePathing(GameTime gameTime, World world, PathManager pathManager)
    {
        bool recalculate = PathTimer.Update(gameTime);
        if (Path.Count == 0 || recalculate)
        {
            Path = pathManager.FindPath(world, Position,
                new PathSettings()
                {
                    Goal = PathGoal.Garage,
                });
        }

        IEnumerable<Enemy> nearbyEnemies = world.EnemyManager.Enemies.Where((enemy) =>
        {
            return !ReferenceEquals(enemy, this) && Vector2.DistanceSquared(enemy.Position, Position) < NearDistanceSquared;
        });

        if (Path.Count == 0)
        {
            return Vector2.Zero;
        }

        PathNode goal = Path[0];
        Vector2 pathingVelocity = Position.MovedTowards(goal.WorldPosition, gameTime.Delta() * speed) - Position;

        Vector2 steering = pathingVelocity - Velocity;
        steering /= Mass;

        foreach (Enemy near in nearbyEnemies)
        {
            Vector2 avoidVelocity = ((Position - near.Position).Normalized() * speed / MathF.Max((Position - near.Position).Length(), 0.01f)).Truncated(speed);
            Vector2 avoidSteering = (avoidVelocity - Velocity) / Mass;
            steering += avoidSteering;
        }

        if (Vector2.Distance(Position, goal.WorldPosition) <= 8.0f)
        {
            Path.RemoveAt(0);
        }

        return steering.Truncated(speed);
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch, InputManager inputManager, World world)
    {
        Visual.Draw(spriteBatch, Position);

        // Draw attached part behaviors
        int index = 0;
        foreach (AttachedPart part in Visual.AttachedParts)
        {
            PartTransform transform = Visual.GetPartTransform(part, Position, index++, physical: true);
            part.DrawBehavior(
                batch: spriteBatch,
                settings: PartSettings.DefaultEnemy,
                transform: transform,
                time: gameTime,
                inputManager: inputManager,
                state: Data.TimeState.Night,
                world: world);
        }
    }

    /// <summary>
    /// Sets a speed modifier for a duration when called, meant to be called by tower attacks that hit the enemy
    /// </summary>
    /// <param name="modifier">the amount subtracted from move speed (larger number slows by more)</param>
    /// <param name="duration">duration of the affect in seconds</param>
    public void setSpeedModifier(float modifier, int duration)
    {
        this.speedMod = modifier;
        this.speedModDuration = duration;
    }

    /// <summary>
    /// Sets damage over time for a duration when called, meant to be called by tower attacks that hit the enemy
    /// </summary>
    /// <param name="damage">the damage taken per tick</param>
    /// <param name="duration">the duration of the effect in seconds</param>
    public void setDamageOverTime(float damage, int duration)
    {
        this.damageOverTime = damage;
        this.damageOverTimeDuration = duration;
    }
}
