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
    public void Draw(SpriteBatch batch)
    {
        batch.Draw(
            texture: Definition.Texture,
            position: Position,
            sourceRectangle: Definition.Cutout,
            origin: Definition.Origin,
            color: Color.White,
            rotation: 0.0f,
            scale: 1.0f,
            effects: SpriteEffects.None,
            layerDepth: 1.0f
            );
    }
}
