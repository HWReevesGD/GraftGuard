using GraftGuard.Grafting.Registry;
using GraftGuard.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace GraftGuard.Grafting.Towers;
internal class TowerTrap : Tower
{
    private const int GridSize = 5;
    private const int GridOffsets = 16;

    public TowerTrap(Vector2 position) : base(position, new Vector2(96, 96), TexturePlaceholderGround, new Rectangle(new Point(-48, -48), new Point(96, 96)))
    {

    }

    public override void Draw(GameTime gameTime, SpriteBatch batch)
    {
        batch.DrawCentered(Texture, Position);
        if (!HasParts) return;

        for (int x = 0; x < GridSize; x++)
        {
            for (int y = 0; y < GridSize; y++)
            {
                PartDefinition part = GetPartFromIndex((x + y) % _attachedParts.Length, shiftIfNull: false);
                if (part is null) continue;
                Point partSize = part.Texture.GetSizePoint();

                float sinHeight = MathF.Sin(x + y + (float)gameTime.TotalGameTime.TotalSeconds * 3.0f) * 4.0f;

                Vector2 gridOffset = new Vector2(x - GridSize / 2, y - GridSize / 2) * GridOffsets;
                gridOffset.Y += sinHeight;

                Vector2 positionalOffset = new Vector2(-partSize.X / 2.0f, -partSize.Y / 2.0f);

                batch.Draw(part.Texture, Position + gridOffset + positionalOffset, new Rectangle(Point.Zero, new Point(partSize.X, (int)(partSize.Y * 0.5f - sinHeight))), Color.White);
            }
        }
    }

    /// <summary>
    /// Function which creates a new Trap Tower. This is passed into the Tower's TowerDefinition
    /// </summary>
    /// <param name="position">Position of the tower</param>
    /// <returns>The created <see cref="Tower"/></returns>
    public static Tower Create(Vector2 position)
    {
        return new TowerTrap(position);
    }

    /// <summary>
    /// Draws the "preview" for the tower, before it is placed. This is generally a transparent version of the tower's base
    /// </summary>
    /// <param name="batch"><see cref="SpriteBatch"/> to use</param>
    /// <param name="time">Current <see cref="GameTime"/></param>
    /// <param name="position">Position to draw at</param>
    public static void DrawPreview(SpriteBatch batch, GameTime time, Vector2 position)
    {
        batch.DrawCentered(TexturePlaceholderGround, position, color: new Color(1.0f, 1.0f, 1.0f, 0.3f));
    }
}
