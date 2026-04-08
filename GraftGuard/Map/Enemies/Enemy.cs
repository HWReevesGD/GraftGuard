using GraftGuard.Grafting.Registry;
using GraftGuard.Map.Enemies.Animation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Reflection.Metadata;
using GraftGuard.Utility;
using GraftGuard.Map.Pathing;
using GraftGuard.Grafting.Registry.Behaviors;
using GraftGuard.Grafting;
using static GraftGuard.Map.Enemies.EnemyVisual;

namespace GraftGuard.Map.Enemies;
internal class Enemy : GameObject
{
    double timer = 0;
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
        Visual = new EnemyVisual(torso, 4f, AnimationClips.Idle, position);
    }

    // Methods
    /// <summary>
    /// Moves the enemy object by having it navigate along a list of PathNodes
    /// </summary>
    /// <param name="route">the PathNode objects that it is moving along</param>
    public void Move(List<PathNode> route)
    {
        PathNode target = route[0]; // Temp

        // Get the unit vector of the direction from the enemy to the node
        Vector2 dirVec = target.WorldPosition - Position;
        dirUnitVec = dirVec / dirVec.Length();

        // Move the enemy after checking to ensure it won't move backwards (sufficiently large speed penalties should just freeze it)
        if (speed - speedMod >= 0)
            Position += dirUnitVec * (speed - speedMod);
    }

    public virtual void OnDeath()
    {
        if (IsDead) return;

        IsDead = true;
        Visual.VisualDeath(Position);
    }

    public override void Update(GameTime gameTime, InputManager inputManager)
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
            Position += new Vector2(1, 0);

            Player player = World.CurrentWorld.Player;

            if (Hitbox.Intersects(player.Hitbox))
            {
                // Trigger damage and knockback
                player.TakeDamage(Position, 10, 50f);
            }

        }

        // Update attached parts
        foreach (AttachedPart part in Visual.AttachedParts)
        {
            LimbDrawContext context = Visual.GetContext(null, Position);
        }

        Visual.Update(gameTime, Position);
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        Visual.Draw(spriteBatch, Position);
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
