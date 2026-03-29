using GraftGuard.Grafting;
using GraftGuard.Grafting.Registry;
using GraftGuard.Map.Enemies.Animation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace GraftGuard.Map.Enemies
{
    public class EnemyVisual
    {
        public BaseDefinition Base { get; private set; }
        public Dictionary<string, PartDefinition> EquippedParts { get; private set; } = new();
        public Animator Animator { get; private set; }
        public float Scale { get; set; }

        private Random rng;

        private float speedMultiplier = 1f;

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
                        EquippedParts[slotName] = selectedHead;
                    }
                }
                else
                {
                    // Fill all other slots with limbs
                    if (limbIndex < limbPool.Count)
                    {
                        EquippedParts[slotName] = limbPool[limbIndex];
                        limbIndex++;
                    }
                }
            }

        }

        public void Update(GameTime gameTime, Vector2 position, float speedMultiplier = 1)
        {
            Animator.Update(gameTime, position, speedMultiplier);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            float timer = Animator.AnimationTimer;
            AnimationClip clip = Animator.CurrentClip;
            float bodyRotation = Animator.GetRotation();

            float walkBob = (float)Math.Sin(timer * 2f) * clip.BobIntensity;
            float sideSway = (float)Math.Sin(timer) * clip.SwayIntensity;
            Vector2 animatedPos = new Vector2(position.X + sideSway, position.Y + walkBob);

            float squash = (float)Math.Sin(timer * 2f) * 0.05f;
            Vector2 dynamicScale = new Vector2(Scale + squash, Scale - squash);

            // Draw Torso
            spriteBatch.Draw(Base.Texture, animatedPos, null, Color.White, bodyRotation,
                new Vector2(Base.Texture.Width / 2, Base.Texture.Height / 2),
                dynamicScale, SpriteEffects.None, 0f);

            var ctx = new LimbDrawContext
            {
                SpriteBatch = spriteBatch,
                BodyPos = animatedPos,
                BodyRot = bodyRotation,
                DynamicScale = dynamicScale,
                Timer = timer,
                Clip = clip
            };



            // Draw Limbs
            int count = 0;
            foreach (var slot in Base.AttachmentPoints)
            {
                if (EquippedParts.TryGetValue(slot.Key, out var part))
                {
                    //Convert normalized Pivot (0.0 to 1.0) into center-relative pixel offset
                    Vector2 pixelOffset = new Vector2(
                        (slot.Value.X - 0.5f) * Base.Texture.Width,
                        (slot.Value.Y - 0.5f) * Base.Texture.Height
                    );

                    DrawLimb(part.Name, pixelOffset, part, count++, ctx);
                }
            }
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
            // Use ctx.Timer, ctx.BodyPos, etc.
            Vector2 rotatedOffset = Vector2.Transform(offset * Scale, Matrix.CreateRotationZ(ctx.BodyRot));
            Vector2 worldAttachPos = ctx.BodyPos + rotatedOffset;

            float phaseShift = index * 0.5f;
            float rawSine = (float)Math.Sin(ctx.Timer + phaseShift);
            float snappyWobble = Math.Sign(rawSine) * (float)Math.Pow(Math.Abs(rawSine), 0.5f) * ctx.Clip.LimbWobbleScale;

            ctx.SpriteBatch.Draw(part.Texture, worldAttachPos, null, Color.White, ctx.BodyRot + snappyWobble,
                new Vector2(part.Texture.Width * part.PivotX, part.Texture.Height * part.PivotY),
                ctx.DynamicScale, SpriteEffects.None, 0f);
        }
    }
}
