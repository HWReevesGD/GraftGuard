using GraftGuard.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace GraftGuard.Graphics;
internal class DrawManager
{
    public SpriteBatch Batch;
    SortedDictionary<int, List<DrawInstruction>> DrawLayers; 

    public DrawManager(SpriteBatch batch)
    {
        Batch = batch;
        DrawLayers = [];
    }

    public void DrawCentered(
        Texture2D texture,
        Vector2 position,
        bool useSorting = false,
        int drawLayer = 1,
        Rectangle? source = null,
        Color? color = null,
        float rotation = 0.0f,
        Vector2? scale = null,
        Vector2? origin = null,
        SpriteEffects? effects = null,
        Vector2? sortingOriginOffset = null)
    {
        Vector2 centeredOrigin = texture.GetSize() * 0.5f + (origin ?? Vector2.Zero);
        Draw(
            texture,
            position,
            useSorting,
            drawLayer,
            source,
            color,
            rotation,
            scale,
            centeredOrigin,
            effects,
            sortingOriginOffset
            );
    }
    public void Draw(
        Texture2D texture,
        Vector2 position,
        bool useSorting = false,
        int drawLayer = 1,
        Rectangle? source = null,
        Color? color = null,
        float rotation = 0.0f,
        Vector2? scale = null,
        Vector2? origin = null,
        SpriteEffects? effects = null,
        Vector2? sortingOriginOffset = null)
    {
        List<DrawInstruction> draws = DrawLayers.GetValueOrDefault(drawLayer, []);
        draws.Add(new DrawInstruction(
                texture,
                position,
                scale ?? Vector2.One,
                source ?? texture.Bounds,
                color ?? Color.White,
                rotation,
                origin ?? Vector2.Zero,
                effects ?? SpriteEffects.None,
                (origin ?? Vector2.Zero) + (sortingOriginOffset ?? Vector2.Zero),
                drawLayer
                ));
        DrawLayers[drawLayer] = draws;
    }

    public void Draw(
        Texture2D texture,
        Rectangle destination,
        bool useSorting = false,
        int drawLayer = 1,
        Rectangle? source = null,
        Color? color = null,
        float rotation = 0.0f,
        Vector2? origin = null,
        SpriteEffects? effects = null,
        Vector2? sortingOriginOffset = null)
    {
        List<DrawInstruction> draws = DrawLayers.GetValueOrDefault(drawLayer, []);
        draws.Add(new DrawInstruction(
                texture,
                destination.Location.ToVector(),
                destination.Size.ToVector(),
                source ?? texture.Bounds,
                color ?? Color.White,
                rotation,
                origin ?? Vector2.Zero,
                effects ?? SpriteEffects.None,
                (origin ?? Vector2.Zero) + (sortingOriginOffset ?? Vector2.Zero),
                drawLayer
                ));
        DrawLayers[drawLayer] = draws;
    }

    public void Paint()
    {
        foreach (List<DrawInstruction> layer in DrawLayers.Values)
        {
            Batch.Begin(sortMode: SpriteSortMode.BackToFront, blendState: BlendState.NonPremultiplied, samplerState: SamplerState.PointClamp);

            foreach (DrawInstruction instruction in layer)
            {
                Batch.Draw(
                    instruction.Texture,
                    instruction.Position,
                    instruction.Source,
                    instruction.Color,
                    instruction.Rotation,
                    instruction.Origin,
                    instruction.Scale,
                    instruction.Effects,
                    instruction.Position.Y + instruction.SortingOrigin.Y * 0.0000001f + 0.001f
                    );
            }

            Batch.End();
        }
    }
}
internal struct DrawInstruction
{
    public Texture2D Texture;
    public Vector2 Position;
    public Vector2 Scale;
    public Rectangle Source;
    public Color Color;
    public float Rotation;
    public Vector2 Origin;
    public SpriteEffects Effects;
    public Vector2 SortingOrigin;
    public int DrawLayer;
    public DrawInstruction(Texture2D texture, Vector2 position, Vector2 scale, Rectangle source, Color color, float rotation, Vector2 origin, SpriteEffects effects, Vector2 sortingOrigin, int drawLayer)
    {
        Texture = texture;
        Position = position;
        Scale = scale;
        Source = source;
        Color = color;
        Rotation = rotation;
        Origin = origin;
        Effects = effects;
        SortingOrigin = sortingOrigin;
        DrawLayer = drawLayer;
    }
} 