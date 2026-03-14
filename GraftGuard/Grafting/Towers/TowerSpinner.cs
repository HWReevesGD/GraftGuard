using GraftGuard.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraftGuard.Grafting.Towers
{
    internal class TowerSpinner : Tower, ITower
    {
        public readonly Vector2 SpinOffset = new Vector2(0, -16);

        public TowerSpinner(Vector2 position) : base(position, new Vector2(64, 64), TexturePlaceholder1)
        {
            SetPart(new PartDefinition("test", PartDefinition.TexturePlaceholder1, 1.201f), Slot.First);
            SetPart(new PartDefinition("test_knife_limb", PartDefinition.TexturePlaceholder2, 1.0f), Slot.Second);
            SetPart(new PartDefinition("test_knife_limb", PartDefinition.TexturePlaceholder2, 1.0f), Slot.Third);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch batch)
        {
            batch.DrawCentered(Texture, Position);
            int attachedSoFar = 0;

            foreach (PartDefinition part in _attachedParts)
            {
                if (part is null) continue;
                float rotation = ((float)gameTime.TotalGameTime.TotalSeconds * 8.0f) % MathF.Tau;
                rotation += MathF.Tau / TotalAttachedParts * attachedSoFar;
                attachedSoFar++;
                batch.Draw(part.Texture, Position + SpinOffset, null, Color.White, rotation, new Vector2(8, 32), Vector2.One, SpriteEffects.None, 1.0f);
                batch.Draw(part.Texture, Position + SpinOffset, null, Color.White, rotation - 0.2f, new Vector2(8, 48), Vector2.One, SpriteEffects.None, 1.0f);
                batch.Draw(part.Texture, Position + SpinOffset, null, Color.White, rotation - 0.4f, new Vector2(8, 64), Vector2.One, SpriteEffects.None, 1.0f);
            }
        }

        public static new Tower Create(Vector2 position)
        {
            return new TowerSpinner(position);
        }
    }
}
