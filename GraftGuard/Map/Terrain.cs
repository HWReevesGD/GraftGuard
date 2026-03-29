using GraftGuard.Graphics;
using GraftGuard.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace GraftGuard.Map;
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
            new Rectangle(0, 960, 320, 1280),
            new Rectangle(0, 960 + 320, 1280, 1280),
            new Rectangle(312 + 480, 960, 1280, 1280),
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

        //test props
        EnvironmentProps.Draw(batch, PropType.LargeTree1, new Vector2(200, 200));
        EnvironmentProps.Draw(batch, PropType.LargeTree2, new Vector2(400, 200));
        EnvironmentProps.Draw(batch, PropType.Flowers1, new Vector2(200, 400));
        EnvironmentProps.Draw(batch, PropType.Flowers2, new Vector2(300, 400));
        EnvironmentProps.Draw(batch, PropType.Flowers3, new Vector2(400, 400));

        EnvironmentProps.Draw(batch, PropType.BushSpiky, new Vector2(200, 300));
        EnvironmentProps.Draw(batch, PropType.BushSpiky, new Vector2(300, 300));
        EnvironmentProps.Draw(batch, PropType.SmallBush, new Vector2(400, 300));
    }
    public bool Overlaps(Circle circle)
    {
        return Boxes.Any((box) => box.Intersects(circle));
    }
}
