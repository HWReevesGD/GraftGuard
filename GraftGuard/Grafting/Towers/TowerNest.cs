using GraftGuard.Data;
using GraftGuard.Graphics;
using GraftGuard.Map;
using GraftGuard.Utility;
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
    public static readonly Vector2[] PartOffsets = [
        new Vector2(-10, 2),
        new Vector2(11, 3),
        new Vector2(3, 27),
        new Vector2(-17, 17),
        ];
    public TowerNest(Vector2 position)
        : base(position, new Vector2(64 ,64), TNest, new Rectangle(-32, -32, 64, 64), 0.2f, [new Rectangle(-32, -32, 64, 64)], PartSettings.DefaultTower)
    {

    }
    public override void Draw(GameTime time, DrawManager drawing, World world, InputManager inputManager, TimeState state, bool isUi = false, SortMode defaultSortMode = SortMode.Sorted, int drawLayerOffset = 0)
    {
        drawing.DrawCentered(Texture, Position, drawLayer: 1 + drawLayerOffset, sortMode: defaultSortMode, isUi: isUi);
        for (int index = 0; index < _attachedParts.Length; index++)
        {
            AttachedPart part = _attachedParts[index];

            if (part is null)
            {
                continue;
            }

            Vector2 position = Position + PartOffsets[index];
            drawing.Draw(part.Definition.Texture, position, isUi: isUi, drawLayer: 1 + drawLayerOffset, origin: new Vector2(part.Definition.Texture.Width / 2, 0), rotation: -MathF.PI + MathF.Sin(time.Total() + index) * 0.3f, sortMode: defaultSortMode,
                sortingOriginOffset: new Vector2(0, 64.0f));
            part.DrawBehavior(Settings, new PartTransform() { Position = position, Rotation = -MathF.PI + MathF.Sin(time.Total() + index) * 0.3f, Origin = new Vector2(-part.Definition.Texture.Width / 2, 0) },
                time, drawing, world, inputManager, state, isUi);
        }
    }
    public static void DrawPreview(DrawManager drawing, GameTime time, Vector2 position)
    {
        drawing.DrawCentered(TNest, position);
    }
}
