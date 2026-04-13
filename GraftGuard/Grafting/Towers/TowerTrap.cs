using GraftGuard.Data;
using GraftGuard.Grafting.Registry;
using GraftGuard.Grafting.Registry.Behaviors;
using GraftGuard.Graphics;
using GraftGuard.Map;
using GraftGuard.Map.Enemies;
using GraftGuard.Map.Projectiles;
using GraftGuard.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.XPath;
using static System.Net.Mime.MediaTypeNames;

namespace GraftGuard.Grafting.Towers;
internal class TowerTrap : Tower
{
    private const int GridSize = 5;
    private const int GridOffsets = 16;
    private const float DamageInterval = 0.5f;

    private IntervalTimer damageInterval;

    public TowerTrap(Vector2 position) : base(
        position,
        new Vector2(96, 96),
        TexturePlaceholderGround,
        new Rectangle(new Point(-48, -48), new Point(96, 96)),
        2.0f,
        [new Rectangle(-48, -48, 96, 96)],
        settings: PartSettings.DefaultTower with
        {
            PartsAreVertical = true,
        })
    {
        damageInterval = new IntervalTimer(DamageInterval);
    }

    public override void Update(GameTime time, World world, InputManager inputManager, TimeState state, ProjectileManager projectileDiversion = null)
    {
        bool dealDamage = damageInterval.Update(time);

        if (dealDamage)
        {
            // Damage is the Sum of all part damages, and the sun of all part Critical Modifiers of random strength 0% - 100%
            // If there is a null part, that part adds 0 damage to the sum
            float damage = _attachedParts.Sum(
                (part) => part is not null ? (part.Definition.BaseDamage + part.Definition.CriticalModifier * random.NextSingle()) : 0.0f
                );
            world.EnemyManager.DealDamageInAreas([Hitbox], [], damage);
        }

        for (int x = 0; x < GridSize; x++)
        {
            for (int y = 0; y < GridSize; y++)
            {
                AttachedPart part = GetPart((x + y) % _attachedParts.Length, shiftIfNull: false);

                if (part is null) continue;

                Vector2 partPosition = GetPartPosition(time, part, x, y);
                Vector2 partSize = part.Definition.Texture.GetSize();

                PartTransform transform = new PartTransform()
                {
                    Position = partPosition + partSize * 0.5f,
                    Rotation = -MathF.PI / 2.0f,
                };

                part.UpdateBehavior(Settings, transform, time, world, inputManager, state, projectileDiversion ?? world.ProjectileManager);

                if (dealDamage)
                {
                    part.BehaviorOnDealDamage(0.25f, Settings, transform, time, world, inputManager, state, projectileDiversion ?? world.ProjectileManager);
                }
            }
        }
    }

    public override void Draw(GameTime time, DrawManager drawing, World world, InputManager inputManager, TimeState state, bool isUi = false, SortMode sortMode = SortMode.Sorted)
    {
        drawing.DrawCentered(Texture, Position, isUi: isUi);
        if (!HasParts) return;

        for (int x = 0; x < GridSize; x++)
        {
            for (int y = 0; y < GridSize; y++)
            {
                AttachedPart part = GetPart((x + y) % _attachedParts.Length, shiftIfNull: false);
                if (part is null) continue;

                Vector2 partPosition = GetPartPosition(time, part, x, y);
                Point partSize = part.Definition.Texture.GetSizePoint();
                float sinHeight = MathF.Sin(x + y + (float)time.TotalGameTime.TotalSeconds * 3.0f) * 4.0f;

                drawing.Draw(part.Definition.Texture, partPosition, source: new Rectangle(new Point(0, (int)(partSize.Y * 0.5f - sinHeight)), new Point(partSize.X, (int)(partSize.Y * 0.5f - sinHeight))),
                    effects: SpriteEffects.FlipVertically,
                    origin: Vector2.Zero, isUi: isUi);

                PartTransform transform = new PartTransform()
                {
                    Position = partPosition + partSize.ToVector() * 0.5f,
                    Rotation = -MathF.PI / 2.0f,
                };

                part.DrawBehavior(Settings, transform, time, drawing, world, inputManager, state, isUi: isUi);
            }
        }
    }

    public Vector2 GetPartPosition(GameTime time, AttachedPart part, int partX, int partY)
    {
        Point partSize = part.Definition.Texture.GetSizePoint();

        float sinHeight = MathF.Sin(partX + partY + (float)time.TotalGameTime.TotalSeconds * 3.0f) * 4.0f;

        Vector2 gridOffset = new Vector2(partX - GridSize / 2, partY - GridSize / 2) * GridOffsets;
        gridOffset.Y += sinHeight;

        Vector2 positionalOffset = new Vector2(-partSize.X / 2.0f, -partSize.Y / 2.0f);
        return Position + gridOffset + positionalOffset;
    }

    /// <summary>
    /// Function which creates a new Trap Tower. This is passed into the Tower's TowerDefinition
    /// </summary>
    /// <param name="position">Position of the tower</param>
    /// <returns>The created <see cref="Tower"/></returns>
    public static Tower Create(Vector2 position)
    {
        return new TowerTrap(position);
    }

    /// <summary>
    /// Draws the "preview" for the tower, before it is placed. This is generally a transparent version of the tower's base
    /// </summary>
    /// <param name="drawing"><see cref="SpriteBatch"/> to use</param>
    /// <param name="time">Current <see cref="GameTime"/></param>
    /// <param name="position">Position to draw at</param>
    public static void DrawPreview(DrawManager drawing, GameTime time, Vector2 position)
    {
        drawing.DrawCentered(TexturePlaceholderGround, position, color: new Color(1.0f, 1.0f, 1.0f, 0.3f));
    }
}
