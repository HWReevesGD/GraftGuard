using GraftGuard.Grafting;
using GraftGuard.UI;
using GraftGuard.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace GraftGuard.Graphics;
internal class DrawManager
{
    public SpriteBatch Batch;
    public const int LayerCount = 8;
    public List<List<DrawInstruction>> DrawLayers { get; private set; } = [];
    public List<List<DrawInstruction>> DrawUILayers { get; private set; } = [];
    public const float SortingCompressment = 1.0f / 100_000_000_000.0f;
    public const float SortingCompressmentAddition = 1.0f / 1_000_000;
    public Rectangle? ForceScissor { get; set; } = null;
    public Matrix? ExtraMatrix { get; set; } = null;
    public BeginBatch CurrentBatchBegin = null;


    public DrawManager(SpriteBatch batch)
    {
        Batch = batch;
        for (int i = 0; i < LayerCount; i++)
        {
            DrawLayers.Add([]);
            DrawUILayers.Add([]);
        }
    }

    public void DrawCentered(
        Texture2D texture,
        Rectangle destination,
        SortMode sortMode = SortMode.Sorted,
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
            destination,
            sortMode,
            drawLayer,
            source,
            color,
            rotation,
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
        SortMode sortMode = SortMode.Sorted,
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
            sortMode,
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
        SortMode sortMode = SortMode.Sorted,
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
                sortMode,
                scale ?? Vector2.One,
                source ?? texture.Bounds,
                null,
                color ?? Color.White,
                rotation,
                origin ?? Vector2.Zero,
                effects ?? SpriteEffects.None,
                (origin ?? Vector2.Zero) + (sortingOriginOffset ?? Vector2.Zero),
                drawLayer,
                false,
                null,
                null,
                ForceScissor ?? scissor,
                extraMatrix: ExtraMatrix
                ), drawLayer, isUi);
    }

    public void Draw(
        Texture2D texture,
        Rectangle destination,
        SortMode sortMode = SortMode.Sorted,
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
                Vector2.Zero,
                sortMode,
                Vector2.One,
                source ?? texture.Bounds,
                destination,
                color ?? Color.White,
                rotation,
                origin ?? Vector2.Zero,
                effects ?? SpriteEffects.None,
                (origin ?? Vector2.Zero) + (sortingOriginOffset ?? Vector2.Zero),
                drawLayer,
                false,
                null,
                null,
                ForceScissor ?? scissor,
                extraMatrix: ExtraMatrix
                ), drawLayer, isUi);
    }

    public void DrawString(
        string text,
        Vector2 position,
        SpriteFont font = null,
        SortMode sortMode = SortMode.Sorted,
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
                sortMode,
                Vector2.One,
                Rectangle.Empty,
                null,
                color ?? Color.White,
                0.0f,
                Vector2.Zero,
                SpriteEffects.None,
                scale ?? Vector2.One,
                drawLayer,
                true,
                text,
                font,
                ForceScissor ?? scissor,
                extraMatrix: ExtraMatrix
                ), drawLayer, isUi);
    }

    public void AddAtLayer(DrawInstruction instruction, int drawLayer, bool isUi)
    {
        List<List<DrawInstruction>> drawLayers = isUi ? DrawUILayers : DrawLayers;
        drawLayers[drawLayer].Add(instruction);
    }

    public void DrawCircle(Circle circle, Color? color = null, int drawLayer = 1, bool isUi = false)
    {
        DrawCentered(Placeholders.TextureCircle, destination: new Rectangle(circle.Center.ToPoint(), new Point((int)(circle.Radius * 2.0f))), drawLayer: drawLayer, isUi: isUi);
    }
    public void DrawCircle(Vector2 position, float radius, Color? color = null, int drawLayer = 1, bool isUi = false)
    {
        DrawCentered(Placeholders.TextureCircle, destination: new Rectangle(position.ToPoint(), new Point((int)(radius * 2.0f))), drawLayer: drawLayer, isUi: isUi);
    }

    float WrapOne(float number)
    {
        if (number > 1.0)
        {
            return number - MathF.Floor(number);
        }
        if (number < 0.0)
        {
            return number + MathF.Ceiling(Math.Abs(number));
        }
        return number;
    }

    private static RasterizerState UseScissor = new RasterizerState() { ScissorTestEnable = true };

    public delegate void BeginBatch(Matrix matrix, Camera camera);

    public void BeginWorld(Matrix matrix, Camera camera) => Batch.Begin(sortMode: SpriteSortMode.FrontToBack, blendState: BlendState.NonPremultiplied, samplerState: SamplerState.PointWrap, transformMatrix: matrix * camera.WorldToScreen);
    public void BeginWorldScissor(Matrix matrix, Camera camera) => Batch.Begin(sortMode: SpriteSortMode.FrontToBack, blendState: BlendState.NonPremultiplied, samplerState: SamplerState.PointWrap, rasterizerState: UseScissor, transformMatrix: matrix * camera.WorldToScreen);
    public void BeginUi(Matrix matrix, Camera camera) => Batch.Begin(sortMode: SpriteSortMode.BackToFront, blendState: BlendState.NonPremultiplied, samplerState: SamplerState.PointWrap, transformMatrix: matrix);
    public void BeginUiScissor(Matrix matrix, Camera camera) => Batch.Begin(sortMode: SpriteSortMode.BackToFront, blendState: BlendState.NonPremultiplied, samplerState: SamplerState.PointWrap, rasterizerState: UseScissor, transformMatrix: matrix);

    public void Paint(Camera camera)
    {
        for (int index = 0; index < LayerCount; index++)
        {
            List<DrawInstruction> gameLayer = DrawLayers[index];
            List<DrawInstruction> uiLayer = DrawUILayers[index];

            List<DrawInstruction> gameScissors = [];
            List<DrawInstruction> uiScissors = [];

            CurrentBatchBegin = BeginWorld;
            CurrentBatchBegin(Matrix.Identity, camera);
            foreach (DrawInstruction instruction in gameLayer)
            {
                DrawInstruction(instruction, camera, gameScissors);
            }
            Batch.End();
            CurrentBatchBegin = BeginWorldScissor;
            CurrentBatchBegin(Matrix.Identity, camera);
            foreach (DrawInstruction instruction in gameScissors)
            {
                Batch.GraphicsDevice.ScissorRectangle = instruction.Scissor.Value;
                DrawInstruction(instruction, camera);
            }
            Batch.End();
            CurrentBatchBegin = BeginUi;
            CurrentBatchBegin(Matrix.Identity, camera);
            foreach (DrawInstruction instruction in uiLayer)
            {
                DrawInstruction(instruction, camera, uiScissors);
            }
            Batch.End();
            CurrentBatchBegin = BeginUiScissor;
            CurrentBatchBegin(Matrix.Identity, camera);
            foreach (DrawInstruction instruction in uiScissors)
            {
                Batch.GraphicsDevice.ScissorRectangle = instruction.Scissor.Value;
                DrawInstruction(instruction, camera);
            }
            Batch.End();

        }

        foreach (List<DrawInstruction> layer in DrawLayers) layer.Clear();
        foreach (List<DrawInstruction> layer in DrawUILayers) layer.Clear();
    }

    private void DrawInstruction(DrawInstruction instruction, Camera camera, List<DrawInstruction> scissors = null)
    {
        if (scissors is not null && instruction.Scissor is not null)
        {
            scissors.Add(instruction);
            return;
        }

        if (instruction.ExtraMatrix is Matrix || instruction.Scissor is not null)
        {
            Batch.End();
            CurrentBatchBegin(instruction.ExtraMatrix ?? Matrix.Identity, camera);
        }

        float sorting = 1.0f;

        switch (instruction.SortMode)
        {
            case SortMode.Sorted:
                sorting = WrapOne((instruction.Position.Y + instruction.SortingOrigin.Y) * SortingCompressment + SortingCompressmentAddition);
                break;
            case SortMode.Top:
                sorting = 0.0f;
                break;
            case SortMode.Middle:
                sorting = 0.5f;
                break;
            case SortMode.Bottom:
                break;
        }

        if (instruction.Destination is null)
        {
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
        else
        {
            Batch.Draw(
                instruction.Texture,
                instruction.Destination.Value,
                instruction.Source,
                instruction.Color,
                instruction.Rotation,
                instruction.Origin,
                instruction.Effects,
                sorting
                );
        }

        if (instruction.ExtraMatrix is Matrix || instruction.Scissor is not null)
        {
            Batch.End();
            CurrentBatchBegin(Matrix.Identity, camera);
        }
    }
}

internal struct DrawInstruction
{
    public Texture2D Texture;
    public Vector2 Position;
    public Vector2 Scale;
    public Rectangle Source;
    public Rectangle? Destination;
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
    public SortMode SortMode;
    public Matrix? ExtraMatrix;
    public DrawInstruction(
        Texture2D texture,
        Vector2 position,
        SortMode sortMode,
        Vector2 scale,
        Rectangle source,
        Rectangle? destination,
        Color color,
        float rotation,
        Vector2 origin,
        SpriteEffects effects,
        Vector2 sortingOrigin,
        int drawLayer,
        bool isString,
        string text,
        SpriteFont font,
        Rectangle? scissor,
        Matrix? extraMatrix)
    {
        Texture = texture;
        Position = position;
        SortMode = sortMode;
        Scale = scale;
        Source = source;
        Destination = destination;
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
        ExtraMatrix = extraMatrix;
    }
}

internal enum SortMode
{
    Sorted,
    Top,
    Middle,
    Bottom,
}