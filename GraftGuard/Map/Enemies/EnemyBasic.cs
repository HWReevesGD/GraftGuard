using GraftGuard.Grafting.Registry;
using GraftGuard.Graphics;
using GraftGuard.Map.Pathing;
using GraftGuard.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using static GraftGuard.Map.Pathing.PathManager;

namespace GraftGuard.Map.Enemies;
internal class EnemyBasic : Enemy
{
    public EnemyBasic(Vector2 position, BaseDefinition torso)
        : base(position, torso, hitboxSize: new Vector2(32, 48), 15.0f, 180.0f)
    {
        PathTimer = new IntervalTimer(0.5f);
    }

    public override void Update(GameTime gameTime, InputManager inputManager, World world, PathManager pathManager)
    {
        base.Update(gameTime, inputManager, world, pathManager);

        if (inputManager.IsKeyDown(Keys.I))
        {
            Health = -100;
        }
    }

    public override void Draw(GameTime gameTime, DrawManager drawing)
    {
        base.Draw(gameTime, drawing);
        string text = $"HP: {Health}";
        drawing.DrawString(text, Position, color: Color.Red, centered: true);
    }

    public override void UpdatePathing(GameTime gameTime, InputManager inputManager, World world, PathManager pathManager)
    {
        Vector2 steeringPathing = BasicPathing(gameTime, world, pathManager, PathGoal.Garage);
        Position += steeringPathing;
    }
}
