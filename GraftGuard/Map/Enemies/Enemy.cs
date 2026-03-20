using GraftGuard.Grafting.Registry;
using GraftGuard.Map.Enemies.Animation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace GraftGuard.Map.Enemies;
internal class Enemy : GameObject
{
    // Fields
    private Vector2 dirUnitVec;
    private float speed;

    public float Health { get; set; }

    public EnemyVisual Visual { get; private set; }

    public static AnimationClip Idle = new AnimationClip
    {
        ClipName = "Idle",
        IsStatic = true,
        StrideLength = 2.0f,
        BobIntensity = 1.0f,
        SwayIntensity = 0.5f,
        LimbWobbleScale = 0.05f,
        LeanFactor = 0
    };

    public static AnimationClip Walk = new AnimationClip
    {
        ClipName = "Walk",
        IsStatic = false,
        StrideLength = 0.1f,
        BobIntensity = 2.5f,
        SwayIntensity = 3.0f,
        LimbWobbleScale = 0.25f,
        LeanFactor = 0.15f
    };


    public Enemy(Vector2 position, BaseDefinition torso, Vector2 hitboxSize, float health, float speed)
        : base(position, hitboxSize, torso.Texture, collisionLayers: CollisionLayer.Enemy)
    {
        this.Health = health;
        this.speed = speed;
        dirUnitVec = new Vector2();

        // Attachment points logic could also be moved to a Factory or Registry later
        SetupDefaultAttachmentPoints(torso);


        // Initialize the visual component
        Visual = new EnemyVisual(torso, 1f, Idle);
    }

    private static void SetupDefaultAttachmentPoints(BaseDefinition torso)
    {
        if (torso.AttachmentPoints.Count > 0) return;
        torso.AttachmentPoints.Add("Head", new Vector2(-1, -10));
        torso.AttachmentPoints.Add("RightArm", new Vector2(5, -10));
        torso.AttachmentPoints.Add("LeftArm", new Vector2(-6.75f, -10));
        torso.AttachmentPoints.Add("RightLeg", new Vector2(5.75f, 8.25f));
        torso.AttachmentPoints.Add("LeftLeg", new Vector2(-6.5f, 8.25f));
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
        Vector2 dirVec = target.Position - Position;
        dirUnitVec = dirVec / dirVec.Length();

        // Move the enemy
        Position += dirUnitVec * speed;
    }

    public virtual void OnDeath()
    {

    }

    public override void Update(GameTime gameTime, InputManager inputManager)
    {
        Visual.Update(gameTime, Position);
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        Visual.Draw(spriteBatch, Position);
    }
}
