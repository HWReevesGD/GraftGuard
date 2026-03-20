using GraftGuard.UI;
using GraftGuard.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GraftGuard.Map.Enemies;
internal class EnemyDummy : Enemy
{
    Texture2D torsoTex;
    public EnemyDummy(Vector2 position, Texture2D torsoTex, Texture2D headTex)
        : base(position, 0, 1, new TorsoDefinition("idk", torsoTex), hitboxSize: new Vector2(32, 48), torsoTex, 30.0f, 1.0f, headTex, route: [])
    {

    }
    

    public override void Draw(GameTime gameTime, SpriteBatch batch)
    {
        base.Draw(gameTime, batch);
        string text = $"HP: {Health}";
        batch.DrawString(Fonts.Arial, text, Position - Fonts.Arial.MeasureString(text) * 0.5f, Color.White);
    }
}
