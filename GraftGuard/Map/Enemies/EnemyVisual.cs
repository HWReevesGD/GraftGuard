using GraftGuard.Grafting;
using GraftGuard.Grafting.Registry;
using GraftGuard.Graphics;
using GraftGuard.Map.Enemies.Animation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace GraftGuard.Map.Enemies;
public class EnemyVisual
{
    public BaseDefinition Base { get; private set; }
    internal List<AttachedPart> AttachedParts = [];
    public Animator Animator { get; private set; }
    public float Scale { get; set; }
    public Vector2 SortingOffset { get; set; }

    private Random rng;

    private float speedMultiplier = 1f;

    //handle death
    private List<FallingPart> fallingParts = new();
    public bool AllPartsLanded => IsDead && fallingParts.Count == 0;
    public bool IsDead { get; private set; } = false;

    public EnemyVisual(BaseDefinition torsoBase, float scale, AnimationClip initialClip, Vector2 spawnPos, Vector2? sortingOffset = null)
    {
        rng = new Random();

        Base = torsoBase;
        Scale = scale;
        Animator = new Animator(initialClip);
        SortingOffset = sortingOffset ?? Vector2.Zero;

        Animator.Teleport(spawnPos);
        InitializeRandomParts();
    }

    protected void InitializeRandomParts()
    {
        // Iterate through all sockets, and assign an appropriate part
        foreach (string slotName in Base.AttachmentPoints.Keys)
        {
            if (slotName == "Head")
            {
                AttachedParts.Add(new AttachedPart(GraftLibrary.GetRandomHead(), slotName));
            }
            else
            {
                // Fill all other slots with limbs
                AttachedParts.Add(new AttachedPart(GraftLibrary.GetRandomLimb(), slotName));
            }
        }
    }

    public void VisualDeath(Vector2 deathPosition)
    {
        IsDead = true;

        Random rnd = new Random();

        // Explode Limbs and Head
        foreach (AttachedPart part in AttachedParts)
        {
            // Calculate a vector pointing away from the center
            Vector2 direction = Vector2.Normalize(new Vector2(rnd.Next(-100, 101), rnd.Next(-100, 101)));
            fallingParts.Add(new FallingPart(part.Definition, deathPosition, direction));
        }

        // Clear the equipped parts so they stop drawing in the normal Draw call
        AttachedParts.Clear();
    }

    public void Update(GameTime gameTime, Vector2 position, float speedMultiplier = 1)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (!IsDead)
        {
            Animator.Update(gameTime, position, speedMultiplier);
        }
        else
        {
            for (int i = fallingParts.Count - 1; i >= 0; i--)
            {
                fallingParts[i].Update(dt);

                if (fallingParts[i].HasLanded)
                {
                    World.ScatterPart(fallingParts[i].GetLandingPosition(), fallingParts[i].Definition);

                    fallingParts.RemoveAt(i);
                }
            }
        }
    }
    internal void Draw(DrawManager drawing, Vector2 position)
    {
        if (!IsDead)
        {
            LimbDrawContext ctx = GetContext(drawing, position);
            Vector2 sortingPosition = position + SortingOffset;

            // Draw Torso
            drawing.Draw(
                texture: Base.Texture,
                position: ctx.BodyPos,
                rotation: ctx.BodyRot,
                origin: new Vector2(Base.Texture.Width / 2, Base.Texture.Height / 2),
                scale: ctx.DynamicScale,
                sortingOriginOffset: SortingOffset - new Vector2(Base.Texture.Width / 2, Base.Texture.Height / 2));
            // Draw Limbs
            int count = 0;
            foreach (AttachedPart part in AttachedParts)
            {
                //legs and head in front of that
                DrawLayer(drawing, ctx, p => !p.SlotName.Contains("Arm", StringComparison.OrdinalIgnoreCase));

                //arms above that
                DrawLayer(drawing, ctx, p => p.SlotName.Contains("Arm", StringComparison.OrdinalIgnoreCase));
            }
        }
        else
        {
            foreach (var part in fallingParts)
            {
                part.Draw(drawing);
            }
        }
    }
    /// <summary>
    /// Helper to draw a specific subset of parts based on a filter
    /// </summary>
    private void DrawLayer(DrawManager spriteBatch, LimbDrawContext ctx, Func<AttachedPart, bool> filter)
    {
        int count = 0;
        foreach (AttachedPart part in AttachedParts)
        {
            if (!filter(part)) continue;

            Vector2 slotPosition = Base.AttachmentPoints[part.SlotName];
            Vector2 pixelOffset = new Vector2(
                (slotPosition.X - 0.5f) * Base.Texture.Width,
                (slotPosition.Y - 0.5f) * Base.Texture.Height
            );

            DrawLimb(part.Definition.Name, pixelOffset, part.Definition, count++, ctx, SortingOffset, part.Definition.FlipHorizonal ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
        }
    }

    internal LimbDrawContext GetContext(DrawManager drawing, Vector2 enemyPosition)
    {
        float timer = Animator.AnimationTimer;
        AnimationClip clip = Animator.CurrentClip;
        float bodyRotation = Animator.GetRotation();

        float walkBob = (float)Math.Sin(timer * 2f) * clip.BobIntensity;
        float sideSway = (float)Math.Sin(timer) * clip.SwayIntensity;
        Vector2 animatedPos = new Vector2(enemyPosition.X + sideSway, enemyPosition.Y + walkBob);

        float squash = (float)Math.Sin(timer * 2f) * 0.05f;
        Vector2 dynamicScale = new Vector2(Scale + squash, Scale - squash);

        return new LimbDrawContext
        {
            Drawing = drawing,
            BodyPos = animatedPos,
            BodyRot = bodyRotation,
            DynamicScale = dynamicScale,
            Timer = timer,
            Clip = clip
        };
    }

    internal PartTransform GetPartTransform(PartDefinition part, Vector2 offset, LimbDrawContext context, int index)
    {
        // Use ctx.Timer, ctx.BodyPos, etc.
        Vector2 rotatedOffset = Vector2.Transform(offset * Scale, Matrix.CreateRotationZ(context.BodyRot));
        Vector2 worldAttachPos = context.BodyPos + rotatedOffset;

        float phaseShift = index * 0.5f;
        float rawSine = (float)Math.Sin(context.Timer + phaseShift);
        float snappyWobble = Math.Sign(rawSine) * (float)Math.Pow(Math.Abs(rawSine), 0.5f) * context.Clip.LimbWobbleScale;

        return new PartTransform()
        {
            Position = worldAttachPos,
            Rotation = context.BodyRot + snappyWobble,
            Origin = new Vector2(part.Texture.Width * part.PivotX, part.Texture.Height * part.PivotY),
            Scale = context.DynamicScale
        };
    }

    internal PartTransform GetPartTransform(AttachedPart part, Vector2 enemyPosition, int index, bool physical = false)
    {
        Vector2 slotPosition = Base.AttachmentPoints[part.SlotName];
        //Convert normalized Pivot (0.0 to 1.0) into center-relative pixel offset
        Vector2 pixelOffset = new Vector2(
            (slotPosition.X - 0.5f) * Base.Texture.Width,
            (slotPosition.Y - 0.5f) * Base.Texture.Height
        );

        LimbDrawContext context = GetContext(null, enemyPosition);
        PartTransform transform = GetPartTransform(part.Definition, pixelOffset, context, index);
        if (physical)
        {
            transform.Rotation += MathF.PI / 2.0f;
        }
        return transform;
    }

    internal struct LimbDrawContext
    {
        public DrawManager Drawing;
        public Vector2 BodyPos;
        public float BodyRot;
        public Vector2 DynamicScale;
        public float Timer;
        public AnimationClip Clip;
    }

    internal void DrawLimb(string slotName, Vector2 offset, PartDefinition part, int index, LimbDrawContext ctx, Vector2 sortingOffset, SpriteEffects effects)
    {
        PartTransform transform = GetPartTransform(part, offset, ctx, index);

        ctx.Drawing.Draw(
            texture: part.Texture,
            position: transform.Position,
            rotation: transform.Rotation,
            origin: transform.Origin,
            scale: transform.Scale,
            sortingOriginOffset: sortingOffset - transform.Origin,
            effects: effects);
    }
}
