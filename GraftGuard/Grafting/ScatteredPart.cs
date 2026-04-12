using GraftGuard.Grafting.Registry;
using GraftGuard.Graphics;
using GraftGuard.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace GraftGuard.Grafting;
internal class ScatteredPart : GameObject
{
    private static readonly Random _random = new Random();
    private float _randomRotation;
    public PartDefinition Definition { get; private set; }
    public ScatteredPart(Vector2 position, PartDefinition definition) : base(position, new Vector2(32, 32), null, collisionLayers: CollisionLayer.ScatteredPart)
    {
        Definition = definition;
        _randomRotation = _random.NextSingle() * MathF.Tau;
    }
    public override void Draw(GameTime gameTime, DrawManager drawing)
    {
        drawing.DrawCentered(Definition.Texture, Position, rotation: _randomRotation, useSorting: true);
    }
}
