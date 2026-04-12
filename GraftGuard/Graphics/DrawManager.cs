using GraftGuard.Grafting;
using GraftGuard.UI;
using GraftGuard.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace GraftGuard.Graphics;
internal class DrawManager
{
    public SpriteBatch Batch;
    SortedDictionary<int, List<DrawInstruction>> DrawLayers = [];
    SortedDictionary<int, List<DrawInstruction>> DrawUILayers = [];

    public DrawManager(SpriteBatch batch)
    {
        Batch = batch;
        DrawLayers = [];
    }

    public void DrawCentered(
        Texture2D texture,
        Rectangle destination,
        bool useSorting = false,
        int drawLayer = 1,
        Rectangle? source = null,
        Color? color = null,
        float rotation = 0.0f,
        Vector2? origin = null,
        SpriteEffects? effects = null,
        Vector2? sortingOriginOffset = null,
        bool isUi = false,
        Rectangle? scissor = null)
    {
        Vector2 centeredOrigin = texture.GetSize() * 0.5f + (origin ?? Vector2.Zero);
        Draw(
            texture,
            destination.Location.ToVector(),
            useSorting,
            drawLayer,
            source,
            color,
            rotation,
            destination.Size.ToVector(),
            centeredOrigin,
            effects,
            sortingOriginOffset,
            isUi,
            scissor
            );
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
        Vector2? sortingOriginOffset = null,
        bool isUi = false,
        Rectangle? scissor = null)
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
            sortingOriginOffset,
            isUi,
            scissor
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
        Vector2? sortingOriginOffset = null,
        bool isUi = false,
        Rectangle? scissor = null)
    {
        AddAtLayer(new DrawInstruction(
                texture,
                position,
                useSorting,
                scale ?? Vector2.One,
                source ?? texture.Bounds,
                color ?? Color.White,
                rotation,
                origin ?? Vector2.Zero,
                effects ?? SpriteEffects.None,
                (origin ?? Vector2.Zero) + (sortingOriginOffset ?? Vector2.Zero),
                drawLayer,
                false,
                null,
                null,
                scissor
                ), drawLayer, isUi);
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
        Vector2? sortingOriginOffset = null,
        bool isUi = false,
        Rectangle? scissor = null)
    {
        AddAtLayer(new DrawInstruction(
                texture,
                destination.Location.ToVector(),
                useSorting,
                destination.Size.ToVector(),
                source ?? texture.Bounds,
                color ?? Color.White,
                rotation,
                origin ?? Vector2.Zero,
                effects ?? SpriteEffects.None,
                (origin ?? Vector2.Zero) + (sortingOriginOffset ?? Vector2.Zero),
                drawLayer,
                false,
                null,
                null,
                scissor
                ), drawLayer, isUi);
    }

    public void DrawString(
        string text,
        Vector2 position,
        SpriteFont font = null,
        bool useSorting = false,
        int drawLayer = 1,
        Color? color = null,
        Vector2? scale = null,
        bool centered = false,
        bool isUi = false,
        Rectangle? scissor = null
        )
    {
        font ??= Fonts.Arial;
        if (centered)
        {
            position -= font.MeasureString(text) * 0.5f;
        }
        AddAtLayer(new DrawInstruction(
                null,
                position,
                useSorting,
                Vector2.Zero,
                Rectangle.Empty,
                color ?? Color.White,
                0.0f,
                Vector2.Zero,
                SpriteEffects.None,
                scale ?? Vector2.One,
                drawLayer,
                true,
                text,
                font,
                scissor
                ), drawLayer, isUi);
    }

    public void AddAtLayer(DrawInstruction instruction, int drawLayer, bool isUi)
    {
        SortedDictionary<int, List<DrawInstruction>> drawLayers = isUi ? DrawUILayers : DrawLayers;
        List<DrawInstruction> draws = drawLayers.GetValueOrDefault(drawLayer, []);
        draws.Add(instruction);
        drawLayers[drawLayer] = draws;
    }

    public void DrawCircle(Circle circle, Color? color = null, int drawLayer = 1, bool isUi = false)
    {
        DrawCentered(Placeholders.TextureCircle, destination: new Rectangle(circle.Center.ToPoint(), new Point((int)(circle.Radius * 2.0f))), drawLayer: drawLayer, isUi: isUi);
    }
    public void DrawCircle(Vector2 position, float radius, Color? color = null, int drawLayer = 1, bool isUi = false)
    {
        DrawCentered(Placeholders.TextureCircle, destination: new Rectangle(position.ToPoint(), new Point((int)(radius * 2.0f))), drawLayer: drawLayer, isUi: isUi);
    }

    public void Paint()
    {
        foreach (List<DrawInstruction> layer in DrawLayers.Values)
        {
            Batch.Begin(sortMode: SpriteSortMode.BackToFront, blendState: BlendState.NonPremultiplied, samplerState: SamplerState.PointClamp);

            List<DrawInstruction> scissors = [];
            foreach (DrawInstruction instruction in layer)
            {
                if (instruction.Scissor is not null)
                {
                    scissors.Add(instruction);
                    continue;
                }

                float sorting = 1.0f;

                if (instruction.UseSorting)
                {
                    sorting = instruction.Position.Y + instruction.SortingOrigin.Y * 0.0000001f + 0.001f;
                }

                if (!instruction.IsString)
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
                    sorting
                    );
                }
                else
                {
                    Batch.DrawString(
                        instruction.Font,
                        instruction.Text,
                        instruction.Position,
                        instruction.Color,
                        instruction.Rotation,
                        instruction.Origin,
                        instruction.Scale,
                        instruction.Effects,
                        sorting
                        );
                }
            }

            Batch.End();
            Batch.Begin(sortMode: SpriteSortMode.BackToFront, blendState: BlendState.NonPremultiplied, samplerState: SamplerState.PointClamp, rasterizerState: new RasterizerState() { ScissorTestEnable = true });
            foreach (DrawInstruction instruction in scissors)
            {
                Batch.GraphicsDevice.ScissorRectangle = instruction.Scissor.Value;

                float sorting = 1.0f;

                if (instruction.UseSorting)
                {
                    sorting = instruction.Position.Y + instruction.SortingOrigin.Y * 0.0000001f + 0.001f;
                }

                if (!instruction.IsString)
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
                    sorting
                    );
                }
                else
                {
                    Batch.DrawString(
                        instruction.Font,
                        instruction.Text,
                        instruction.Position,
                        instruction.Color,
                        instruction.Rotation,
                        instruction.Origin,
                        instruction.Scale,
                        instruction.Effects,
                        sorting
                        );
                }
            }
            Batch.End();
        }

        DrawLayers = [];
        DrawUILayers = [];
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
    public bool IsString;
    public string Text;
    public SpriteFont Font;
    public Rectangle? Scissor;
    public bool UseSorting;
    public DrawInstruction(
        Texture2D texture,
        Vector2 position,
        bool useSorting,
        Vector2 scale,
        Rectangle source,
        Color color,
        float rotation,
        Vector2 origin,
        SpriteEffects effects,
        Vector2 sortingOrigin,
        int drawLayer,
        bool isString,
        string text,
        SpriteFont font,
        Rectangle? scissor)
    {
        Texture = texture;
        Position = position;
        UseSorting = useSorting;
        Scale = scale;
        Source = source;
        Color = color;
        Rotation = rotation;
        Origin = origin;
        Effects = effects;
        SortingOrigin = sortingOrigin;
        DrawLayer = drawLayer;
        IsString = isString;
        Text = text;
        Font = font;
        Scissor = scissor;
    }
} 