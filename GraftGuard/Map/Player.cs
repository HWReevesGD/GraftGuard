using GraftGuard.Data;
using GraftGuard.Grafting;
using GraftGuard.Grafting.Registry;
using GraftGuard.Graphics;
using GraftGuard.Graphics.Particles;
using GraftGuard.Map.Enemies;
using GraftGuard.Map.Enemies.Animation;
using GraftGuard.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace GraftGuard.Map;
internal class Player : GameObject
{
    public const int MaxHeldParts = 8;
    public const float PickupRadius = 32;
    public const float Speed = 600.0f;

    public static readonly Vector2 CenterOffset = new Vector2(25, 50) * 0.5f;
    private static readonly float invincibilityFrameTime = 0.5f; // in seconds
    public Vector2 VisualCenter => Position + new Vector2(HitboxSize.X * 0.5f, -16f);
    public Vector2 SortingOffset { get; set; } = new Vector2(0, 150);

    private static Texture2D playerTorso;
    private static Texture2D playerHead;
    private static Texture2D playerLeg;
    private static Texture2D playerArm;
    private static Texture2D playerHair;

    private Circle _collectionCircle;
    private PlayerVisual playerVisual;
    private float invincibilityTimer;

    private List<FallingPart> fallingParts = new();

    public List<PartDefinition> HeldParts { get; private set; }
    public bool InventoryFull => HeldParts.Count >= MaxHeldParts;
    public event Action OnDamaged;

    public static void LoadContent(ContentManager content)
    {
        playerTorso = content.Load<Texture2D>("Entities/Player/player_torso");
        playerHead = content.Load<Texture2D>("Entities/Player/player_face");
        playerLeg = content.Load<Texture2D>("Entities/Player/player_leg");
        playerArm = content.Load<Texture2D>("Entities/Player/player_arm");
        playerHair = content.Load<Texture2D>("Entities/Player/player_ponytail");

    }

    public Player(Vector2 position) : base(position, new Vector2(18, 18), playerTorso, collisionLayers: CollisionLayer.Player, collisionMasks: CollisionLayer.Solid | CollisionLayer.Terrain)
    {
        _collectionCircle = new Circle(Center, PickupRadius);
        HeldParts = [];

        SetupVisual();
    }

    /// <summary>
    /// Sets up the <see cref="Player"/> for a new Session
    /// </summary>
    public void Setup()
    {
        Position = Vector2.Zero;
        ClearHeldParts();
    }

    public void SetupVisual()
    {
        var playerSockets = new List<AttachPoint>
        {
            new AttachPoint { Name = "Arm_R", PivotX = 0.6859326f, PivotY = 0.28445747f },
            new AttachPoint { Name = "Arm_L", PivotX = 0.31524926f, PivotY = 0.28445747f },
            new AttachPoint { Name = "Leg_L", PivotX = 0.29765397f, PivotY = 0.7683284f },
            new AttachPoint { Name = "Leg_R", PivotX = 0.66129035f, PivotY = 0.7859238f },
            new AttachPoint { Name = "Head",  PivotX = 0.47947213f, PivotY = 0.199648f },
            new AttachPoint { Name = "Ponytail", PivotX = 0.15f, PivotY = -0.15f },
        };

        playerVisual = new PlayerVisual(playerTorso, playerSockets, 1, AnimationClips.Idle, Center, SortingOffset);

        playerVisual.CreatePart("Head", "PlayerHead", playerHead, 0.53f, 0.77f, PartType.Head, false);
        playerVisual.CreatePart("Arm_R", "PlayerArmR", playerArm, 0.48f, 0.24f, PartType.Limb, false);
        playerVisual.CreatePart("Arm_L", "PlayerArmL", playerArm, 0.48f, 0.24f, PartType.Limb, true);
        playerVisual.CreatePart("Leg_R", "PlayerLegR", playerLeg, 0.48f, 0.26f, PartType.Limb, false);
        playerVisual.CreatePart("Leg_L", "PlayerLegL", playerLeg, 0.48f, 0.26f, PartType.Limb, true);
        playerVisual.CreatePart("Ponytail", "Ponytail", playerHair, 0.42f, 0.25f, PartType.Limb, true);
    }

    public override void Update(GameTime gameTime, InputManager inputManager)
    {
        // Delta Time
        float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

        // Get Movement from Input
        Vector2 moveVector = inputManager.GetMovementDirection();

        // Move with Collision
        MoveAndCollide(moveVector * Speed * delta, World.CurrentWorld);

        HandlePartPickups(World.CurrentWorld);

        playerVisual.Update(gameTime, VisualCenter, .1f);

        if (invincibilityTimer > 0)
            invincibilityTimer -= delta;

        for (int i = fallingParts.Count - 1; i >= 0; i--)
        {
            fallingParts[i].Update(delta);

            if (fallingParts[i].HasLanded)
            {
                World.ScatterPart(fallingParts[i].GetLandingPosition(), fallingParts[i].Definition);

                fallingParts.RemoveAt(i);
            }
        }

        if (inputManager.WasKeyPressStarted(Keys.Q))
        {
            DropHeldParts();
        }
    }

    public override void Draw(GameTime gameTime, DrawManager drawing)
    {
        

        if (invincibilityTimer > 0)
        {
            int phase = (int)(invincibilityTimer / 0.025f);
            if (phase % 2 == 0) // skip drawing for this frame to make the flashing effect
                return;
        }

        playerVisual.Draw(drawing, VisualCenter);
        //base.Draw(gameTime, new Rectangle(Position.ToPoint(), new Point(25, 50)), batch);

        // Draw Held Parts
        for (int index = 0; index < HeldParts.Count; index++)
        {
            Texture2D part = HeldParts[index].Texture;
            Vector2 offset = Vector2.UnitY * (index - 2) * 8 * 10;
            Vector2 position = VisualCenter - Vector2.UnitY * (index - 2) * 8;
            drawing.Draw(part, position, rotation: -MathF.PI / 2.0f, sortingOriginOffset: offset);
        }

        foreach (var part in fallingParts)
        {
            part.Draw(drawing);
        }

        //drawing.Draw(Placeholders.TexturePixel, Hitbox, color: Color.Red);
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
        List<ScatteredPart> scatteredParts = world.ScatteredParts.Where((part) => Vector2.DistanceSquared(Position, part.Position) <= _collectionCircle.Radius * _collectionCircle.Radius).ToList();

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

        //updated game log parts collected
        PlayerData.CurrentGame.GameLog.PartsCollected++;

        // Remove ScatteredPart from the World
        world.ScatteredParts.Remove(scatteredPart);
    }

    public void DropHeldParts()
    {

        Random rnd = new Random();

        // Explode Limbs and Head
        foreach (PartDefinition part in HeldParts)
        {

            // Calculate a vector pointing away from the center
            Vector2 direction = Vector2.Normalize(new Vector2(rnd.Next(-100, 101), rnd.Next(-100, 101)));
            fallingParts.Add(new FallingPart(part, Position, direction));

        }

        // Clear the equipped parts so they stop drawing in the normal Draw call
        ClearHeldParts();

    }



    public void ClearHeldParts() => HeldParts.Clear();
}
