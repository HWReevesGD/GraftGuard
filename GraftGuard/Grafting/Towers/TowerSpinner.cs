using GraftGuard.Grafting.Registry;
using GraftGuard.Map;
using GraftGuard.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace GraftGuard.Grafting.Towers;
internal class TowerSpinner : Tower
{
    public readonly Vector2 SpinOffset = new Vector2(0, -16);

    public TowerSpinner(Vector2 position) : base(position, new Vector2(64, 64), TexturePlaceholderTower, new Rectangle(new Point(-32, -32), new Point(64, 64)))
    {

    }

    public override void Update(GameTime time, World world, InputManager inputManager, TimeState state)
    {
        base.Update(time, world, inputManager, state);
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

    /// <summary>
    /// Function which creates a new Spinner Tower. This is passed into the Tower's TowerDefinition
    /// </summary>
    /// <param name="position">Position of the tower</param>
    /// <returns>The created <see cref="Tower"/></returns>
    public static Tower Create(Vector2 position)
    {
        return new TowerSpinner(position);
    }

    /// <summary>
    /// Draws the "preview" for the tower, before it is placed. This is generally a transparent version of the tower's base
    /// </summary>
    /// <param name="batch"><see cref="SpriteBatch"/> to use</param>
    /// <param name="time">Current <see cref="GameTime"/></param>
    /// <param name="position">Position to draw at</param>
    public static void DrawPreview(SpriteBatch batch, GameTime time, Vector2 position)
    {
        batch.DrawCentered(TexturePlaceholderTower, position, color: new Color(1.0f, 1.0f, 1.0f, 0.3f));
    }
}
