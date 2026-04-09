using GraftGuard.Grafting;
using GraftGuard.Grafting.Registry;
using GraftGuard.Map.Pathing;
using GraftGuard.Utility;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraftGuard.Map.Enemies;
internal class EnemyCentipede : Enemy
{
    static BaseDefinition shell = GraftLibrary.GetBaseByName("Shell");
    public EnemyCentipede(Vector2 position)
        : base(position, shell, new Vector2(32, 32), 10.0f, 256.0f)
    {
        PathTimer = new IntervalTimer(1.0f);
    }

    public override void UpdatePathing(GameTime gameTime, InputManager inputManager, World world, PathManager pathManager)
    {
        
    }
}