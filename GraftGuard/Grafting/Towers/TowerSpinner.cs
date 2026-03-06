using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraftGuard.Grafting.Towers
{
    internal class TowerSpinner : Tower
    {
        public readonly Vector2 SpinOffset = new Vector2(16, 0);

        public TowerSpinner(Vector2 position) : base(position, new Vector2(64, 64), TexturePlaceholder)
        {
            SetPart(new PartDefinition("test", PartDefinition.TexturePlaceholder1, 1.201f), Slot.First);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch batch)
        {
            base.Draw(gameTime, batch);
            
            foreach (PartDefinition part in _attachedParts)
            {
                if (part is null) continue;
                batch.Draw(part.Texture, Position + SpinOffset, null, Color.White, (float)gameTime.TotalGameTime.TotalSeconds % MathF.Tau, new Vector2(8, 0), Vector2.One, SpriteEffects.None, 1.0f);
            }
        }
    }
}
