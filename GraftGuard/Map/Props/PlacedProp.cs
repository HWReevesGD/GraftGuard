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
        //Rectangle rect = new Rectangle(
        //    (int)(Position.X - Definition.Origin.X),
        //    (int)(Position.Y - Definition.Origin.Y),
        //    (int)(Definition.Cutout.Width * 0.8f),
        //    (int)(Definition.Cutout.Height * 0.8f)
        //    );
        //rect.AddX((int)(Position.X - rect.Width * Definition.Origin.X));
        //rect.AddY((int)(Position.Y - rect.Height * Definition.Origin.Y));
        //rect.AddX((int)Position.X);
        //rect.AddY((int)Position.Y);

        //bool isPlayerInFront = player.Position.Y >= Position.Y;
        //bool isPlayerColliding = rect.Intersects(player.Hitbox);
        drawing.Draw(
            texture: Definition.Texture,
            position: Position,
            source: Definition.Cutout,
            origin: Definition.Origin,
            useSorting: true
            );

        //batch.Draw(Placeholders.TexturePixel, rect, new Color(255, 0, 0, 25));
    }
}
