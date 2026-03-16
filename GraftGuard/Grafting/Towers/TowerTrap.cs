using GraftGuard.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraftGuard.Grafting.Towers;
internal class TowerTrap : Tower, ITower
{
    private const int GridSize = 5;
    private const int GridOffsets = 16;

    public bool TrapActive = true;

    public TowerTrap(Vector2 position) : base(position, new Vector2(64, 64), TexturePlaceholder2)
    {
        SetPart(new PartDefinition("test_zombie_limb", PartDefinition.TexturePlaceholder1, 1.201f), Slot.First);
        SetPart(new PartDefinition("test_knife_limb", PartDefinition.TexturePlaceholder2, 1.0f), Slot.Second);
    }

    public override void Draw(GameTime gameTime, SpriteBatch batch)
    {
        batch.DrawCentered(Texture, Position);
        if (!TrapActive || !HasParts) return;

        for (int x = 0; x < GridSize; x++)
        {
            for (int y = 0; y < GridSize; y++)
            {
                PartDefinition part = GetPartFromIndex((x + y) % _attachedParts.Length, shiftIfNull: true);
                Point partSize = part.Texture.GetSizePoint();

                float sinHeight = MathF.Sin(x + y + (float)gameTime.TotalGameTime.TotalSeconds * 3.0f) * 4.0f;

                Vector2 gridOffset = new Vector2(x - GridSize / 2, y - GridSize / 2) * GridOffsets;
                gridOffset.Y += sinHeight;

                Vector2 positionalOffset = new Vector2(-partSize.X / 2.0f, -partSize.Y / 2.0f);
                
                batch.Draw(part.Texture, Position + gridOffset + positionalOffset, new Rectangle(Point.Zero, new Point(partSize.X, (int)(partSize.Y * 0.5f - sinHeight))), Color.White);
            }
        }
    }

    public static new Tower Create(Vector2 position)
    {
        return new TowerTrap(position);
    }
}
