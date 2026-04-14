using GraftGuard.Graphics;
using GraftGuard.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraftGuard.Map.Props;
internal class PlacedProp
{
    private static int translucentOpacity = 160;

    public PropDefinition Definition;
    public Vector2 Position;
    public Rectangle Collision => Definition.Collision.Translated((Position - Definition.Origin).ToPoint());
    public PlacedProp(PropDefinition definition, Vector2 position)
    {
        Definition = definition;
        Position = position;
    }
    public void Draw(DrawManager drawing, Player player)
    {
        Rectangle rect = new Rectangle(
            (int)(Position.X - Definition.Origin.X),
            (int)(Position.Y - Definition.Origin.Y),
            (int)(Definition.Cutout.Width * 0.8f),
            (int)(Definition.Cutout.Height * 0.8f)
            );

        bool isPlayerInFront = player.Position.Y >= Position.Y;
        bool isPlayerColliding = rect.Intersects(player.Hitbox);

        drawing.Draw(
            texture: Definition.Texture,
            position: Position,
            source: Definition.Cutout,
            origin: Definition.Origin,
            sortMode: SortMode.Sorted,
            color: !isPlayerInFront && isPlayerColliding ? new Color(255, 255, 255, translucentOpacity) : Color.White
            );

        //batch.Draw(Placeholders.TexturePixel, rect, new Color(255, 0, 0, 25));
    }
}
