using GraftGuard.Grafting;
using GraftGuard.Grafting.Registry;
using GraftGuard.Graphics;
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
        public PlayerVisual(Texture2D texture, List<AttachPoint> attachPoints, float scale, AnimationClip initialClip, Vector2 spawnPos, Vector2 sortingOffset)
            : base(new BaseDefinition("PlayerBase", "PlayerTex", texture, true, attachPoints), scale, initialClip, spawnPos, sortingOffset)
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

        internal new void Draw(DrawManager drawing, Vector2 position)
        {
            //draw ponytail behind first then get rid of it
            LimbDrawContext ctx = GetContext(drawing, position);

            //DEPENDENT ON PONYTAIL ADDED LAST BAD PRACTICE BUT WHATEVER
            Vector2 slotPosition = Base.AttachmentPoints[ponytail.Name];
            Vector2 pixelOffset = new Vector2(
                (slotPosition.X - 0.5f) * Base.Texture.Width,
                (slotPosition.Y - 0.5f) * Base.Texture.Height
            );

            SpriteEffects effects = ponytail.FlipHorizonal ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            // Inside the Draw method
            //Vector2 armSortingOffset = SortingOffset + new Vector2(0, 5); // Nudge 5 pixels down


            DrawLimb(ponytail.Name, pixelOffset, ponytail, -1, ctx, SortingOffset, effects, Color.White);

            base.Draw(drawing, position, Color.White);
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
                new Damage(1, 0, 0, 0, 0),
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
