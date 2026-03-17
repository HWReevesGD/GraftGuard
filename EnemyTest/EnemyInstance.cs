using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace EnemyTest
{

    // Represents an actual enemy instance in the game world
    public class EnemyInstance
    {
        public Vector2 Position;
        public float Rotation;
        public float Scale = 4f;
        public TorsoDefinition Base;

        public Dictionary<string, PartDefinition> EquippedParts = new();

        // For procedural animation
        public float AnimationTimer;

        public EnemyInstance(Vector2 position, float rotation, TorsoDefinition @base)
        {
            Position = position;
            Rotation = rotation;
            Base = @base;
            EquippedParts = new();
            AnimationTimer = 0;
        }

        public void Update(GameTime gameTime)
        {
            // Update the global animation timer
            AnimationTimer += (float)gameTime.ElapsedGameTime.TotalSeconds * 5f; // Speed of wobble
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Draw the Torso
            Vector2 torsoOrigin = new Vector2(Base.Texture.Width / 2f, Base.Texture.Height / 2f);

            spriteBatch.Draw(
                Base.Texture,
                Position,
                null,
                Color.White,
                Rotation,
                torsoOrigin,
                Scale, // Apply scale here
                SpriteEffects.None,
                0f);

            // Draw each attached limb
            foreach (var slot in Base.AttachmentPoints)
            {
                if (EquippedParts.TryGetValue(slot.Key, out var part))
                {
                    Vector2 scaledOffset = slot.Value * Scale;
                    Vector2 worldAttachPos = Position + Vector2.Transform(scaledOffset, Matrix.CreateRotationZ(Rotation));

                    float limbWobble = (float)Math.Sin(AnimationTimer) * 0.25f;
                    float totalLimbRotation = Rotation + limbWobble;

                    // Pivot remains in local texture space
                    Vector2 limbPivot = new Vector2(
                        part.Texture.Width * part.Pivot.X,
                        part.Texture.Height * part.Pivot.Y
                    );

                    spriteBatch.Draw(
                        part.Texture,
                        worldAttachPos,
                        null,
                        Color.White,
                        totalLimbRotation,
                        limbPivot,
                        Scale, 
                        SpriteEffects.None,
                        0f
                    );
                }
            }
        }
    }
}
