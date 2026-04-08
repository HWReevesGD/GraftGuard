using GraftGuard.Grafting.Registry;
using GraftGuard.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GraftGuard.Map.Enemies;
internal class EnemyDummy : Enemy
{
    public EnemyDummy(Vector2 position, BaseDefinition torso)
        : base(position, torso, hitboxSize: new Vector2(32, 48), 30.0f, 1.0f)
    {

    }

    public override void Update(GameTime gameTime, InputManager inputManager)
    {
        base.Update(gameTime, inputManager);

        if(inputManager.IsKeyDown(Keys.I))
        {
            Health = -100;
        }
    }

    public override void Draw(GameTime gameTime, SpriteBatch batch)
    {
        base.Draw(gameTime, batch);
        string text = $"HP: {Health}";
        batch.DrawString(Fonts.Arial, text, Position - Fonts.Arial.MeasureString(text) * 0.5f, Color.White);
    }

    public override void UpdatePathing(GameTime gameTime, InputManager inputManager, World world)
    {
        
    }
}
