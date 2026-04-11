using GraftGuard.Data;
using GraftGuard.Grafting;
using GraftGuard.Grafting.Registry;
using GraftGuard.Graphics.Particles;
using GraftGuard.Map.Enemies;
using GraftGuard.Map.Enemies.Animation;
using GraftGuard.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace GraftGuard.Map;
internal class Player : GameObject
{
    public const int MaxHeldParts = 8;
    public const float PickupRadius = 32;
    public const float Speed = 600.0f;

    public static readonly Vector2 CenterOffset = new Vector2(25, 50) * 0.5f;
    private static readonly float invincibilityFrameTime = 0.5f; // in seconds

    private static Texture2D texture;
    private Circle _collectionCircle;
    private EnemyVisual playerVisual;
    private float invincibilityTimer;

    public List<PartDefinition> HeldParts { get; private set; }
    public bool InventoryFull => HeldParts.Count >= MaxHeldParts;
    public event Action OnDamaged;

    public static void LoadContent(ContentManager content)
    {
        
    }

    public Player(Vector2 position) : base(position, new Vector2(24, 48), texture, collisionLayers: CollisionLayer.Player, collisionMasks: CollisionLayer.Solid | CollisionLayer.Terrain)
    {
        _collectionCircle = new Circle(Center, PickupRadius);
        HeldParts = [];
        playerVisual = new EnemyVisual(GraftLibrary.GetBaseByName("Default"), 1, AnimationClips.Idle, Center);
    }

    /// <summary>
    /// Sets up the <see cref="Player"/> for a new Session
    /// </summary>
    public void Setup()
    {
        Position = Vector2.Zero;
        ClearHeldParts();
    }

    public override void Update(GameTime gameTime, InputManager inputManager)
    {
        // Delta Time
        float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

        // Get Movement from Input
        Vector2 moveVector = inputManager.GetMovementDirection();

        // Move with Collision
        MoveAndCollide(moveVector * Speed * delta, World.CurrentWorld);

        // Set the Camera's position
        World.CurrentWorld.Camera.Position = Position;

        HandlePartPickups(World.CurrentWorld);

        playerVisual.Update(gameTime, Center, .1f);

        if (invincibilityTimer > 0)
            invincibilityTimer -= delta;
    }

    public override void Draw(GameTime gameTime, SpriteBatch batch)
    {
        if (invincibilityTimer > 0)
        {
            int phase = (int)(invincibilityTimer / 0.025f);
            if (phase % 2 == 0) // skip drawing for this frame to make the flashing effect
                return;
        }

        playerVisual.Draw(batch, Center);
        //base.Draw(gameTime, new Rectangle(Position.ToPoint(), new Point(25, 50)), batch);

        // Draw Held Parts
        for (int index = 0; index < HeldParts.Count; index++)
        {
            Texture2D part = HeldParts[index].Texture;
            batch.Draw(part, Position - Vector2.UnitY * (index - 2) * 8, null, Color.White, -MathF.PI / 2.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);
        }

        batch.Draw(Placeholders.TexturePixel, Hitbox, Color.Red);

        Debug.WriteLine("COUNT!: " + CollidedDebug.Count);
        foreach (var box in CollidedDebug)
        {
            batch.Draw(Placeholders.TexturePixel, box, null, Color.Magenta, 0.0f, Vector2.Zero, SpriteEffects.None, 1.0f);
        }
        CollidedDebug = [];
    }

    
    /// <summary>
    /// Player takes damage from an enemy and gets knocked back to avoid double damage
    /// </summary>
    /// <param name="sourcePosition">Position of contact to move away from</param>
    /// <param name="damageAmount">Amount of damage to deal. Likely only ever 1</param>
    /// <param name="knockbackForce">Amount of force to be knocked back by. Perhaps bigger for bigger foes</param>
    public void TakeDamage(Vector2 sourcePosition, int damageAmount, float knockbackForce)
    {
        if (invincibilityTimer > 0)
            return;

        // Deduct Health
        if (PlayerData.CurrentGame != null)
        {
            PlayerData.CurrentGame.Health -= damageAmount;
        }

        // Calculate Knockback Direction
        Vector2 pushDirection = Position - sourcePosition;
        if (pushDirection == Vector2.Zero) pushDirection = -Vector2.UnitY; 
        pushDirection.Normalize();

        // Apply Knockback
        MoveAndCollide(pushDirection * knockbackForce, World.CurrentWorld);

        OnDamaged.Invoke();
        invincibilityTimer = invincibilityFrameTime;
    }

    /// <summary>
    /// Handles picking up <see cref="ScatteredPart"/>s in the provided<see cref="World"/>. Should be called from <see cref="Update"/>
    /// </summary>
    /// <param name="world">World to use</param>
    private void HandlePartPickups(World world)
    {
        // Get all overlapping ScatteredParts
        List<ScatteredPart> scatteredParts = GetIntersects(
            _collectionCircle.Translated(Position),
            layers: CollisionLayer.ScatteredPart,
            World.ScatteredParts
            );

        // Pickup overlapping parts
        foreach (ScatteredPart part in scatteredParts)
        {
            PickupPart(part, world);
            // Early break if can't hold more parts
            if (HeldParts.Count >= MaxHeldParts)
            {
                break;
            }
        }
    }

    /// <summary>
    /// Removes a <see cref="ScatteredPart"/> from the world and adds it's <see cref="PartDefinition"/> to <see cref="HeldParts"/>. This method does nothing if <see cref="HeldParts"/> is full
    /// </summary>
    /// <param name="scatteredPart">Scattered Part to pickup</param>
    /// <param name="world"><see cref="World"/> to use</param>
    public void PickupPart(ScatteredPart scatteredPart, World world)
    {
        // Don't do anything if the inventory is full
        if (InventoryFull)
        {
            return;
        }

        // Add part definition to _heldParts
        HeldParts.Add(scatteredPart.Definition);

        // Remove ScatteredPart from the World
        World.ScatteredParts.Remove(scatteredPart);
    }

    public void ClearHeldParts() => HeldParts.Clear();
}
