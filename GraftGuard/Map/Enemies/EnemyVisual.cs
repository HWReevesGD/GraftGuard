using GraftGuard.Grafting;
using GraftGuard.Grafting.Registry;
using GraftGuard.Map.Enemies.Animation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace GraftGuard.Map.Enemies
{
    public class EnemyVisual
    {
        public BaseDefinition Base { get; private set; }
        internal List<AttachedPart> AttachedParts = [];
        public Animator Animator { get; private set; }
        public float Scale { get; set; }

        private Random rng;

        private float speedMultiplier = 1f;

        //handle death
        private List<FallingPart> fallingParts = new();
        public bool AllPartsLanded => IsDead && fallingParts.Count == 0;
        public bool IsDead { get; private set; } = false;

        public EnemyVisual(BaseDefinition torsoBase, float scale, AnimationClip initialClip, Vector2 spawnPos)
        {
            rng = new Random();

            Base = torsoBase;
            Scale = scale;
            Animator = new Animator(initialClip);

            Animator.Teleport(spawnPos);

            InitializeDefaultParts();

        }

        private void InitializeDefaultParts()
        {

            List<PartDefinition> headPool = GraftLibrary.AllParts
                .Where(p => p.Type == PartType.Head).ToList();


            List<PartDefinition> limbPool = GraftLibrary.AllParts
                .Where(p => p.Type == PartType.Limb).ToList();

            // Select a random head (if any exist)
            PartDefinition selectedHead = headPool.Count > 0
                ? headPool[rng.Next(0, headPool.Count)]
                : null;

            // Iterate through sockets and assign
            int limbIndex = 0;
            foreach (string slotName in Base.AttachmentPoints.Keys)
            {
                if (slotName == "Head")
                {
                    // Assign the pre-selected random head
                    if (selectedHead != null)
                    {
                        AttachedPart part = new AttachedPart(selectedHead, slotName);
                        AttachedParts.Add(part);
                    }
                }
                else
                {
                    // Fill all other slots with limbs
                    if (limbIndex < limbPool.Count)
                    {
                        AttachedPart part = new AttachedPart(limbPool[limbIndex], slotName);
                        AttachedParts.Add(part);
                        limbIndex++;
                    }
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

        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            if (!IsDead)
            {
                LimbDrawContext ctx = GetContext(spriteBatch, position);

                // Draw Torso
                spriteBatch.Draw(Base.Texture, ctx.BodyPos, null, Color.White, ctx.BodyRot,
                    new Vector2(Base.Texture.Width / 2, Base.Texture.Height / 2),
                    ctx.DynamicScale, SpriteEffects.None, 0f);

                // Draw Limbs
                int count = 0;
                foreach (AttachedPart part in AttachedParts)
                {
                    Vector2 slotPosition = Base.AttachmentPoints[part.SlotName];
                    //Convert normalized Pivot (0.0 to 1.0) into center-relative pixel offset
                    Vector2 pixelOffset = new Vector2(
                        (slotPosition.X - 0.5f) * Base.Texture.Width,
                        (slotPosition.Y - 0.5f) * Base.Texture.Height
                    );

                    DrawLimb(part.Definition.Name, pixelOffset, part.Definition, count++, ctx);
                }
            }
            else
            {
                foreach (var part in fallingParts)
                {
                    part.Draw(spriteBatch);
                }
            }
        }

        public LimbDrawContext GetContext(SpriteBatch spriteBatch, Vector2 enemyPosition)
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
                SpriteBatch = spriteBatch,
                BodyPos = animatedPos,
                BodyRot = bodyRotation,
                DynamicScale = dynamicScale,
                Timer = timer,
                Clip = clip
            };
        }

        public PartTransform GetPartTransform(PartDefinition part, Vector2 offset, LimbDrawContext context, int index)
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

        public struct LimbDrawContext
        {
            public SpriteBatch SpriteBatch;
            public Vector2 BodyPos;
            public float BodyRot;
            public Vector2 DynamicScale;
            public float Timer;
            public AnimationClip Clip;
        }

        private void DrawLimb(string slotName, Vector2 offset, PartDefinition part, int index, LimbDrawContext ctx)
        {
            PartTransform transform = GetPartTransform(part, offset, ctx, index);

            ctx.SpriteBatch.Draw(part.Texture, transform.Position, null, Color.White, transform.Rotation,
                transform.Origin,
                transform.Scale, SpriteEffects.None, 0f);
        }
    }
}
