using GraftGuard.Grafting.Registry;
using GraftGuard.Map.Enemies.Animation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using GraftGuard.Utility; 

namespace GraftGuard.Map.Enemies;
internal class Enemy : GameObject
{
    // Fields
    private Vector2 dirUnitVec;
    private float speed;

    public float Health { get; set; }

    public float Rotation { get; set; }
    public float Scale;
    public TorsoDefinition Base { get; private set; }

    public Dictionary<string, PartDefinition> EquippedParts = new();

    // For procedural animation
    public float AnimationTimer;
    private Vector2 lastPosition;
    private Vector2 currentPosition;

    private Animator animator;
    public Animator EnemyAnimator => animator;

    public AnimationClip CurrentClip { get; private set; }

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


    public Enemy(Vector2 position, float rotation, float scale, TorsoDefinition @base, Vector2 hitboxSize, Texture2D texture, float health, float speed, Texture2D headTex) : base(position, hitboxSize, texture, collisionLayers: CollisionLayer.Enemy)
    {
        this.Health = health;
        this.speed = speed;
        dirUnitVec = new Vector2();

        currentPosition = position;
        lastPosition = position;
        Rotation = rotation;
        Scale = scale;
        Base = @base;
        EquippedParts = new();
        animator = new Animator(Idle);
        AnimationTimer = 0;

        // Define where limbs attach relative to the center of the torso sprite (0,0)
        //TEMPORARY, will be handled in grafter eventually.
        @base.AttachmentPoints.Add("Head", new Vector2(-1, -10));  // Up from center
        @base.AttachmentPoints.Add("RightArm", new Vector2(5, -10)); // Right and slightly up
        @base.AttachmentPoints.Add("LeftArm", new Vector2(-6.75f, -10));// Left and slightly up
        @base.AttachmentPoints.Add("RightLeg", new Vector2(5.75f, 8.25f));  // Down-right
        @base.AttachmentPoints.Add("LeftLeg", new Vector2(-6.5f, 8.25f)); // Down-left


        List<PartDefinition> partList = new(PartRegistry.Parts);

        int partIndex = 0;
        foreach (string slotName in Base.AttachmentPoints.Keys)
        {
            if (partIndex < partList.Count)
            {

                // Plug the part into the socket
                EquippedParts[slotName] = partList[partIndex];


                partIndex++;
            }
        }

        Vector2 headPivot = new Vector2(0.5f, 0.5f);

        PartDefinition manualHead = new PartDefinition("Manual Head", headTex, headPivot);

        // Force-equip to the "Head" socket
        EquippedParts["Head"] = manualHead;
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
        animator.Update(gameTime, Position);
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        float timer = animator.AnimationTimer;
        AnimationClip clip = animator.CurrentClip;
        float bodyRotation = animator.GetRotation();

        // Calculate general offsets once
        float walkBob = (float)Math.Sin(timer * 2f) * clip.BobIntensity;
        float sideSway = (float)Math.Sin(timer) * clip.SwayIntensity;
        Vector2 animatedPos = new Vector2(Position.X + sideSway, Position.Y + walkBob);

        float squash = (float)Math.Sin(timer * 2f) * 0.05f;
        Vector2 dynamicScale = new Vector2(Scale + squash, Scale - squash);

        // Draw Torso
        spriteBatch.Draw(Base.Texture, animatedPos, null, Color.White, bodyRotation,
                         new Vector2(Base.Texture.Width / 2, Base.Texture.Height / 2),
                         dynamicScale, SpriteEffects.None, 0f);

        // Draw Limbs
        int count = 0;
        foreach (var slot in Base.AttachmentPoints)
        {
            if (EquippedParts.TryGetValue(slot.Key, out var part))
            {
                DrawLimb(spriteBatch, slot.Value, part, animatedPos, bodyRotation, dynamicScale, timer, clip, count);
                count++;
            }
        }
    }


    /// <summary>
    /// Calculates the procedural positioning and rotation for an individual limb and draws it to the screen.
    /// </summary>
    /// <param name="sb">The SpriteBatch used for rendering.</param>
    /// <param name="offset">The local attachment point relative to the torso's center.</param>
    /// <param name="part">The definition containing the limb's texture and pivot data.</param>
    /// <param name="bodyPos">The current animated world position of the torso.</param>
    /// <param name="bodyRot">The current rotation/lean of the torso.</param>
    /// <param name="scale">The dynamic squash and stretch scale to apply to the limb.</param>
    /// <param name="timer">The current value of the animator's clock.</param>
    /// <param name="clip">The active AnimationClip providing intensity and wobble parameters.</param>
    /// <param name="index">The index of the limb, used to apply a phase shift for organic, desynchronized movement.</param>
    private void DrawLimb(SpriteBatch sb, Vector2 offset, PartDefinition part, Vector2 bodyPos,
                      float bodyRot, Vector2 scale, float timer, AnimationClip clip, int index)
    {
        Vector2 rotatedOffset = Vector2.Transform(offset * Scale, Matrix.CreateRotationZ(bodyRot));
        Vector2 worldAttachPos = bodyPos + rotatedOffset;

        float phaseShift = index * 0.5f;
        float rawSine = (float)Math.Sin(timer + phaseShift);
        float snappyWobble = Math.Sign(rawSine) * (float)Math.Pow(Math.Abs(rawSine), 0.5f) * clip.LimbWobbleScale;

        sb.Draw(part.Texture, worldAttachPos, null, Color.White, bodyRot + snappyWobble,
                new Vector2(part.Texture.Width * part.PivotX, part.Texture.Height * part.PivotY),
                scale, SpriteEffects.None, 0f);
    }
}

//TEMPORARY
// Defines where things can be attached to a specific Torso
public class TorsoDefinition
{
    public string Name { get; set; }
    public Texture2D Texture { get; set; }

    // Key: Slot Name (e.g., "LeftArm", "Head")
    // Value: The Vector2 offset from the CENTER of the torso sprite
    public Dictionary<string, Vector2> AttachmentPoints { get; set; } = new();

    public TorsoDefinition(string name, Texture2D texture)
    {
        Name = name;
        Texture = texture;
    }


}