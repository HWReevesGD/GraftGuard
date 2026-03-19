using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraftGuard.Map.Enemies;
internal class EnemyManager
{
    private List<PathNode> _pathNodes;
    public List<Enemy> Enemies;

    public EnemyManager()
    {
        _pathNodes = [];
        Enemies = [
            new EnemyDummy(new Vector2(400, 400)),
            ];
    }

    public void Update(GameTime time, InputManager inputManager)
    {
        foreach (Enemy enemy in Enemies)
        {
            enemy.Update(time, inputManager);
        }
    }

    public void Draw(SpriteBatch batch, GameTime time)
    {
        foreach (Enemy enemy in Enemies)
        {
            enemy.Draw(time, batch);
        }
    }
}
