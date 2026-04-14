using GraftGuard.Grafting;
using GraftGuard.Grafting.Registry;
using GraftGuard.Map.Enemies.Animation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraftGuard.Map
{
    public class PlayerVisual : Enemies.EnemyVisual
    {
        PartDefinition ponytail;
        /// <summary>
        /// Creates a PlayerVisual with a custom base texture and defined attachment points.
        /// </summary>
        public PlayerVisual(Texture2D texture, List<AttachPoint> attachPoints, float scale, AnimationClip initialClip, Vector2 spawnPos)
            : base(new BaseDefinition("PlayerBase", "PlayerTex", texture, true, attachPoints), scale, initialClip, spawnPos)
        {
            AttachedParts.Clear();
        }

        /// <summary>
        /// Overriding to prevent the random part generation logic from the base class.
        /// </summary>
        protected new void InitializeRandomParts()
        {
            // Do nothing: We will assign parts manually via SetPart
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            //draw ponytail behind first then get rid of it
            LimbDrawContext ctx = GetContext(spriteBatch, position);


            //DEPENDENT ON PONYTAIL ADDED LAST BAD PRACTICE BUT WHATEVER
            Vector2 slotPosition = Base.AttachmentPoints[ponytail.Name];
            Vector2 pixelOffset = new Vector2(
                (slotPosition.X - 0.5f) * Base.Texture.Width,
                (slotPosition.Y - 0.5f) * Base.Texture.Height
            );

            SpriteEffects effects = ponytail.FlipHorizonal ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            DrawLimb(ponytail.Name, pixelOffset, ponytail, -1, ctx, effects);


            base.Draw(spriteBatch, position);
        }
        /// <summary>
        /// Creates a new PartDefinition on the fly and attaches it to the specified slot.
        /// </summary>
        public void CreatePart(string slotName, string partName, Texture2D texture, float pivotX, float pivotY, PartType type, bool flipHorizontal)
        {
            AttachedParts.RemoveAll(p => p.SlotName == slotName);

            PartDefinition newDef = new PartDefinition(
                partName,
                texture,
                texture.Name ?? "custom_part",
                pivotX,
                pivotY,
                type,
                1.0f,
                flipHorizonal: flipHorizontal
            );

            if(newDef.Name.Equals("Ponytail"))
            {
                ponytail = newDef;
            }
            else
            {
                AttachedParts.Add(new AttachedPart(newDef, slotName));
            }
        }
    }
}
