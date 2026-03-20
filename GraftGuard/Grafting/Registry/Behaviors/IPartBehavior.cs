using GraftGuard.Grafting.Towers;
using GraftGuard.Map;
using GraftGuard.Map.Enemies;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraftGuard.Grafting.Registry.Behaviors;
internal interface IPartBehavior
{
    public void Update(Tower tower, GameTime time, World world, InputManager inputManager, TimeState state);
    public void Draw(Tower tower, GameTime time, SpriteBatch batch, World world, InputManager inputManager, TimeState state);
    public void OnHitEnemy(Tower tower, GameTime time, Enemy enemy, World world, InputManager inputManager, TimeState state);
}
