using GraftGuard.UI;
using GraftGuard.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GraftGuard.Map.Enemies;
internal class EnemyDummy : Enemy
{
    public EnemyDummy(Vector2 position)
        : base(position, hitboxSize: new Vector2(32, 48), Placeholders.TextureEnemyDummy, 30.0f, 1.0f)
    {

    }

    public override void Draw(GameTime gameTime, SpriteBatch batch)
    {
        base.Draw(gameTime, batch);
        string text = $"HP: {Health}";
        batch.DrawString(Fonts.Arial, text, Position - Fonts.Arial.MeasureString(text) * 0.5f, Color.White);
    }
}
