using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraftGuard.Map.Pathing;
internal class PathManager
{
    public readonly Vector2 Start = new Vector2(0, 0);
    public readonly Vector2 End = new Vector2(1000, 1000);
    List<PathNode> Nodes = [];
    
    public void BuildGrid()
    {
        for (float x = Start.X; x < End.X; x += PathNode.GridDistance)
        {
            for (float y = Start.Y; y < End.Y; y += PathNode.GridDistance)
            {
                Nodes.Add(new PathNode(new Vector2(x, y)));
            }
        }
    }

    public void Draw(SpriteBatch batch, GameTime time)
    {
        foreach (PathNode node in Nodes)
        {
            node.Draw(batch);
        }
    }
}
