using GraftGuard.Data;
using GraftGuard.Graphics;
using GraftGuard.Map;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraftGuard.Grafting.Towers;
internal class TowerNest : Tower
{
    public static Tower Create(Vector2 position) => new TowerNest(position);
    public TowerNest(Vector2 position)
        : base(position, new Vector2(64 ,64), TNest, new Rectangle(-32, -32, 64, 64), 0.2f, [new Rectangle(-32, -32, 64, 64)], PartSettings.DefaultTower)
    {

    }
    public override void Draw(GameTime time, DrawManager drawing, World world, InputManager inputManager, TimeState state, bool isUi = false, SortMode defaultSortMode = SortMode.Sorted, int drawLayerOffset = 0)
    {
        drawing.DrawCentered(Texture, Position, drawLayer: 1 + drawLayerOffset, sortMode: defaultSortMode);
    }
    public static void DrawPreview(DrawManager drawing, GameTime time, Vector2 position)
    {
        drawing.DrawCentered(TNest, position);
    }
}
