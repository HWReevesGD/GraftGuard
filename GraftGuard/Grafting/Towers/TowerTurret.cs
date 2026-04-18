using GraftGuard.Data;
using GraftGuard.Grafting.Registry;
using GraftGuard.Graphics;
using GraftGuard.Map;
using GraftGuard.Map.Enemies;
using GraftGuard.Map.Projectiles;
using GraftGuard.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraftGuard.Grafting.Towers;
internal class TowerTurret : Tower
{
    public const float IgnoreRadius = 96.0f;
    public const float IgnoreRadiusSquared = IgnoreRadius * IgnoreRadius;
    public static readonly Vector2[] PartSlots = [
        new Vector2(54, 12),
        new Vector2(54, -12),
        new Vector2(52, 7),
        new Vector2(52, -7),
        ];

    public float CurrentDirection { get; set; }

    public TowerTurret(Vector2 position)
        : base(
            position,
            new Vector2(32, 32),
            TexturePlaceholderTower,
            new Rectangle(-24, -24, 48, 48),
            2.0f,
            [new Rectangle(-32, -32, 64, 64)],
            PartSettings.DefaultTower)
    {
    }

    public override void Update(GameTime time, World world, InputManager inputManager, TimeState state, ProjectileManager projectileDiversion = null)
    {
        CurrentDirection = CurrentDirection.MoveTowardsAngle(GetDirectionToNearest(world), 2.0f * time.Delta());
        float direction = CurrentDirection;
        for (int index = 0; index < _attachedParts.Length; index++)
        {
            AttachedPart part = _attachedParts[index];
            if (part is null)
            {
                continue;
            }

            Vector2 slotOffset = PartSlots[index];
            Vector2 slotPosition = Position + Vector2.Rotate(slotOffset, direction);

            PartTransform transform = new PartTransform()
            {
                Position = slotPosition,
                Rotation = direction - MathF.PI / 2.0f
            };

            part.UpdateBehavior(Settings, transform, time, world, inputManager, state, projectileDiversion ?? world.ProjectileManager);
        }
    }

    public override void Draw(GameTime time, DrawManager drawing, World world, InputManager inputManager, TimeState state, bool isUi = false, SortMode defaultSortMode = SortMode.Sorted, int drawLayerOffset = 0)
    {
        float direction = CurrentDirection;
        drawing.DrawCentered(Texture, Position, isUi: isUi, sortMode: defaultSortMode, drawLayer: 1 + drawLayerOffset);
        drawing.DrawCentered(TTurret, Position, origin: new Vector2(-TTurret.Width / 2 + 2, 0), rotation: direction, isUi: isUi, sortMode: defaultSortMode, drawLayer: 1 + drawLayerOffset);

        for (int index = 0; index < _attachedParts.Length; index++)
        {
            AttachedPart part = _attachedParts[index];
            PartDefinition def = part?.Definition;
            if (part is null)
            {
                continue;
            }

            Vector2 slotOffset = PartSlots[index];
            Vector2 slotPosition = Position + Vector2.Rotate(slotOffset, direction);

            PartTransform transform = new PartTransform()
            {
                Position = slotPosition,
                Rotation = direction - MathF.PI / 2.0f
            };

            drawing.DrawCentered(def.Texture, transform.Position, rotation: transform.Rotation, isUi: isUi, sortMode: defaultSortMode, drawLayer: 1 + drawLayerOffset);
            part.DrawBehavior(Settings, transform, time, drawing, world, inputManager, state, isUi: isUi);
        }
    }

    public float GetDirectionToNearest(World world)
    {
        Enemy closest = null;
        float closestDistanceSquared = float.PositiveInfinity;
        foreach (Enemy enemy in world.EnemyManager.Enemies)
        {
            float distanceSquared = Vector2.DistanceSquared(enemy.Position, Position);
            if (distanceSquared < IgnoreRadiusSquared)
            {
                continue;
            }
            if (distanceSquared < closestDistanceSquared)
            {
                closest = enemy;
                closestDistanceSquared = distanceSquared;
            }
        }

        if (closest is null)
        {
            return CurrentDirection;
        }
        return (closest.Position - Position).OppositeAngle();
    }

    /// <summary>
    /// Function which creates a new Spinner Tower. This is passed into the Tower's TowerDefinition
    /// </summary>
    /// <param name="position">Position of the tower</param>
    /// <returns>The created <see cref="Tower"/></returns>
    public static Tower Create(Vector2 position)
    {
        return new TowerTurret(position);
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
