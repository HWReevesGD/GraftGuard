using GraftGuard.Grafting.Registry;
using GraftGuard.Map.Pathing;
using GraftGuard.UI;
using GraftGuard.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using static GraftGuard.Map.Pathing.PathManager;

namespace GraftGuard.Map.Enemies;
internal class EnemyDummy : Enemy
{
    public List<PathNode> Path = [];
    public IntervalTimer PathTimer;
    public EnemyDummy(Vector2 position, BaseDefinition torso)
        : base(position, torso, hitboxSize: new Vector2(32, 48), 30.0f, 1.0f)
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

    public override void Draw(GameTime gameTime, SpriteBatch batch)
    {
        base.Draw(gameTime, batch);
        string text = $"HP: {Health}";
        batch.DrawString(Fonts.Arial, text, Position - Fonts.Arial.MeasureString(text) * 0.5f, Color.White);
    }

    public override void UpdatePathing(GameTime gameTime, InputManager inputManager, World world, PathManager pathManager)
    {
        bool recalculate = PathTimer.Update(gameTime);
        if (Path.Count == 0 || recalculate)
        {
            Path = pathManager.FindPath(world, Position,
                new PathSettings() {
                    Goal = PathGoal.Player,
                });
        }

        PathNode goal = Path[0];
        Position = Position.MovedTowards(goal.WorldPosition, gameTime.Delta() * 256.0f);

        if (Position == goal.WorldPosition)
        {
            Path.RemoveAt(0);
        }
    }
}
