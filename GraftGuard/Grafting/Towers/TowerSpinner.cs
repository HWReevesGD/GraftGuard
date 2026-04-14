using GraftGuard.Data;
using GraftGuard.Grafting.Registry;
using GraftGuard.Grafting.Registry.Behaviors;
using GraftGuard.Graphics;
using GraftGuard.Map;
using GraftGuard.Map.Projectiles;
using GraftGuard.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Reflection;

namespace GraftGuard.Grafting.Towers;
internal class TowerSpinner : Tower
{
    public const float DamageInterval = 0.1f;
    public const float DamageCircleRadius = 16.0f;
    public const float SpinSpeed = 8.0f;

    public readonly Vector2 SpinOffset = new Vector2(0, -16);
    private IntervalTimer _damageInterval;

    public TowerSpinner(Vector2 position) : base(position, new Vector2(64, 64), TexturePlaceholderTower, new Rectangle(new Point(-32, -32), new Point(64, 64)),
        2.0f, [new Rectangle(-32, -32, 64, 64)])
    {
        _damageInterval = new IntervalTimer(DamageInterval);
    }

    public override void Update(GameTime time, World world, InputManager inputManager, TimeState state, ProjectileManager projectileDiversion = null)
    {
        bool dealDamage = _damageInterval.Update(time);

        for (int index = 0; index < _attachedParts.Length; index++)
        {
            AttachedPart part = _attachedParts[index];
            if (part is null) continue;

            float rotation = GetPartRotation(time, index);
            Vector2 partPosition = Position + SpinOffset + Vector2.Rotate(-Vector2.UnitY, rotation) * 48.0f;

            PartTransform transform = new PartTransform()
            {
                Position = partPosition,
                Rotation = rotation + MathF.PI,
            };

            part.UpdateBehavior(Settings, transform, time, world, inputManager, state, projectileDiversion ?? world.ProjectileManager);
        }
    }

    public override void Draw(GameTime time, DrawManager drawing, World world, InputManager inputManager, TimeState state, bool isUi = false, SortMode defaultSortMode = SortMode.Sorted, int drawLayerOffset = 0)
    {
        drawing.DrawCentered(Texture, Position, isUi: isUi, drawLayer: 1 + drawLayerOffset);

        for (int index = 0; index < _attachedParts.Length; index++)
        {
            AttachedPart part = _attachedParts[index];
            if (part is null) continue;

            float rotation = GetPartRotation(time, index);
            //Vector2 partPosition = Position + SpinOffset + Vector2.Rotate(-Vector2.UnitY, rotation) * 48.0f;

            //Circle damageCircle = new Circle(partPosition, DamageCircleRadius);
            //batch.DrawCircle(damageCircle, Color.Red);
            drawing.DrawCentered(Placeholders.TextureSpinnerArm, Position + SpinOffset, rotation: rotation, origin: new Vector2(0, 24), effects: SpriteEffects.FlipVertically, isUi: isUi, sortMode: defaultSortMode, drawLayer: 1 + drawLayerOffset);
            drawing.DrawCentered(part.Definition.Texture, Position + SpinOffset, rotation: rotation, origin: new Vector2(0, 48), effects: SpriteEffects.FlipVertically, isUi: isUi, sortMode: defaultSortMode, drawLayer: 1 + drawLayerOffset);

            PartTransform transform = new PartTransform()
            {
                Position = Position + SpinOffset + new Vector2(MathF.Cos(rotation - MathF.PI * 0.5f), MathF.Sin(rotation - MathF.PI * 0.5f)) * 48.0f,
                Rotation = rotation + MathF.PI,
            };

            part.DrawBehavior(Settings, transform, time, drawing, world, inputManager, state, isUi: isUi);
        }
    }

    public float GetPartRotation(GameTime time, int partIndex)
    {
        // Base Rotation
        float rotation = (float)time.TotalGameTime.TotalSeconds * SpinSpeed % MathF.Tau;
        // Index-Based offset
        rotation += MathF.Tau / TotalAttachedParts * partIndex;
        // Return Final
        return rotation;
    }

    /// <summary>
    /// Function which creates a new Spinner Tower. This is passed into the Tower's TowerDefinition
    /// </summary>
    /// <param name="position">Position of the tower</param>
    /// <returns>The created <see cref="Tower"/></returns>
    public static Tower Create(Vector2 position)
    {
        return new TowerSpinner(position);
    }

    /// <summary>
    /// Draws the "preview" for the tower, before it is placed. This is generally a transparent version of the tower's base
    /// </summary>
    /// <param name="drawing"><see cref="SpriteBatch"/> to use</param>
    /// <param name="time">Current <see cref="GameTime"/></param>
    /// <param name="position">Position to draw at</param>
    public static void DrawPreview(DrawManager drawing, GameTime time, Vector2 position)
    {
        drawing.DrawCentered(TexturePlaceholderTower, position, color: new Color(1.0f, 1.0f, 1.0f, 0.3f));
    }
}
