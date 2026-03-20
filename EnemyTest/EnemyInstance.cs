using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace EnemyTest
{

    public class EnemyInstance
    {
        public Vector2 Position { get; set; }
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

        public EnemyInstance(Vector2 position, float rotation, float scale, TorsoDefinition @base)
        {
            Position = position;
            currentPosition = position;
            lastPosition = position;    
            Rotation = rotation;
            Scale = scale;
            Base = @base;
            EquippedParts = new();
            animator = new Animator(Idle);
            AnimationTimer = 0;
        }

        public void Update(GameTime gameTime)
        {
            animator.Update(gameTime, Position);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            float timer = animator.AnimationTimer;
            AnimationClip clip = animator.CurrentClip;
            float bodyRotation = animator.GetRotation();

            // Calculate general offsets once
            float walkBob = (float) Math.Sin(timer * 2f) * clip.BobIntensity;
            float sideSway = (float) Math.Sin(timer) * clip.SwayIntensity;
            Vector2 animatedPos = new Vector2(Position.X + sideSway, Position.Y + walkBob);

            float squash = (float) Math.Sin(timer * 2f) * 0.05f;
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
                    new Vector2(part.Texture.Width * part.Pivot.X, part.Texture.Height * part.Pivot.Y),
                    scale, SpriteEffects.None, 0f);
        }
    }
}
