using GraftGuard.Graphics;
using GraftGuard.UI;
using GraftGuard.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ShapeUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraftGuard;
internal class Terrain
{
    public List<Rectangle> Boxes { get; set; } = [];
    public NinePatch TerrainPatch;
    public Terrain()
    {
        TerrainPatch = new NinePatch(Placeholders.TextureTerrain, 15, 15, 15, 15);
        Boxes = [
            new Rectangle(-128, -1280, 1280, 1280),
            new Rectangle(-1280, -128, 1280, 1280),
            new Rectangle(960, 0, 1280, 1280),
            new Rectangle(0, 960, 1280, 1280),
            new Rectangle(320, 421, 320, 149),
            ];
    }

    public void Update(GameTime time)
    {

    }
    public void Draw(SpriteBatch batch, GameTime time)
    {
        foreach (Rectangle box in Boxes)
        {
            TerrainPatch.Draw(batch, box);
        }
    }
}
