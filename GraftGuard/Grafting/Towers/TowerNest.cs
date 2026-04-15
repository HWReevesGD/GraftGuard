using GraftGuard.Data;
using GraftGuard.Graphics;
using GraftGuard.Map;
using GraftGuard.Map.Enemies;
using GraftGuard.Map.Projectiles;
using GraftGuard.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GraftGuard.Grafting.Towers;
internal class TowerNest : Tower
{
    public static Tower Create(Vector2 position) => new TowerNest(position);
    public static readonly Vector2[] PartOffsets = [
        new Vector2(-10, 2),
        new Vector2(11, 3),
        new Vector2(3, 27),
        new Vector2(-17, 17),
        ];
    public List<Nestling> Nestlings = [];
    public IntervalTimer SpawnTimer = new IntervalTimer(2.0f);
    public TowerNest(Vector2 position)
        : base(position, new Vector2(64 ,64), TNest, new Rectangle(-32, -32, 64, 64), 0.2f, [new Rectangle(-32, -32, 64, 64)], PartSettings.DefaultTower)
    {

    }
    public override void Update(GameTime time, World world, InputManager inputManager, TimeState state, ProjectileManager projectileDiversion = null)
    {
        if (SpawnTimer.Update(time) && state == TimeState.Night)
        {
            Nestlings.Add(new Nestling(Position, _attachedParts));
        }
        for (int index = 0; index < Nestlings.Count; index++)
        {
            Nestling nestling = Nestlings[index];
            nestling.Update(time, world, inputManager, state, projectileDiversion ?? world.ProjectileManager);
            if (nestling.NeedsRemoval)
            {
                Nestlings.RemoveAt(index);
                index--;
            }
        }
    }

    public override void Draw(GameTime time, DrawManager drawing, World world, InputManager inputManager, TimeState state, bool isUi = false, SortMode defaultSortMode = SortMode.Sorted, int drawLayerOffset = 0)
    {
        drawing.DrawCentered(Texture, Position, drawLayer: 1 + drawLayerOffset, sortMode: defaultSortMode, isUi: isUi);
        for (int index = 0; index < _attachedParts.Length; index++)
        {
            AttachedPart part = _attachedParts[index];

            if (part is null)
            {
                continue;
            }

            Vector2 position = Position + PartOffsets[index];
            drawing.Draw(part.Definition.Texture, position, isUi: isUi, drawLayer: 1 + drawLayerOffset, origin: new Vector2(part.Definition.Texture.Width / 2, 0), rotation: -MathF.PI + MathF.Sin(time.Total() + index) * 0.3f, sortMode: defaultSortMode,
                sortingOriginOffset: new Vector2(0, 64.0f));
        }
        foreach (Nestling nestling in Nestlings)
        {
            nestling.Draw(drawing, time, world, inputManager, state, isUi, drawLayerOffset);
        }
    }

    public static void DrawPreview(DrawManager drawing, GameTime time, Vector2 position)
    {
        drawing.DrawCentered(TNest, position);
    }
}
internal class Nestling
{
    public static readonly Texture2D Texture = Tower.TNestling;
    public Vector2 Position { get; set; }
    public Vector2 Velocity { get; set; }
    public readonly AttachedPart[] Parts = new AttachedPart[Tower.MaxParts];
    public Enemy Target { get; set; }
    public float Lifetime { get; set; } = 0.0f;
    public const float MaxLifetime = 5.0f;
    public const float Acceleration = 256.0f;
    public const float MaxSpeed = 320.0f;
    public bool NeedsRemoval => Lifetime > MaxLifetime;
    public Nestling(Vector2 position, AttachedPart[] parts)
    {
        Position = position;
        for (int index = 0; index < Tower.MaxParts; index++)
        {
            Parts[index] = parts[index] is null ? null : new AttachedPart(parts[index].Definition);
        }
    }
    public void Update(GameTime time, World world, InputManager input, TimeState state, ProjectileManager projectiles)
    {
        Lifetime += time.Delta();
        if (Target?.IsDead is not false && state == TimeState.Night)
        {
            GetTarget(world.EnemyManager);
        }
        for (int index = 0; index < Parts.Length; index++)
        {
            AttachedPart part = Parts[index];
            if (part is null)
            {
                continue;
            }
            part.UpdateBehavior(PartSettings.DefaultTower, new PartTransform() { Position = Position, Origin = new Vector2(-part.Definition.Texture.Width / 2, 0), Rotation = (time.Total() + index * MathF.PI * 0.5f) % MathF.Tau }, time, world, input, state, projectiles);
        }
        if (state == TimeState.Night && Target is not null)
        {
            Velocity += (Target.Position - Position).Normalized() * Acceleration * time.Delta();
            float magnitude = Velocity.Length();
            if (magnitude > MaxSpeed)
            {
                Velocity = Velocity.Normalized() * MaxSpeed;
            }
            Position += Velocity * time.Delta();
        }
    }

    public void Draw(DrawManager drawing, GameTime time, World world, InputManager input, TimeState state, bool isUi, int drawLayerOffset = 0)
    {
        drawing.DrawCentered(Texture, Position, drawLayer: 1 + drawLayerOffset);
        for (int index = 0; index < Parts.Length; index++)
        {
            AttachedPart part = Parts[index];
            if (part is null)
            {
                continue;
            }
            drawing.Draw(part.Definition.Texture, Position, isUi: isUi, drawLayer: 1 + drawLayerOffset, origin: new Vector2(part.Definition.Texture.Width / 2, 0), rotation: (time.Total() + index * MathF.PI * 0.5f) % MathF.Tau,
                sortingOriginOffset: new Vector2(0, 64.0f));
            part.DrawBehavior(PartSettings.DefaultTower, new PartTransform() { Position = Position, Origin = new Vector2(-part.Definition.Texture.Width / 2, 0), Rotation = (time.Total() + index * MathF.PI * 0.5f) % MathF.Tau }, time, drawing, world, input, state, isUi: isUi);
        }
    }

    public void GetTarget(EnemyManager enemyManager)
    {
        Target = enemyManager.Enemies.Where((enemy) => !enemy.IsDead).MaxBy((enemy) => enemy.Health);
    }

    public bool TargetsExist(EnemyManager enemyManager)
    {
        return enemyManager.Enemies is not null && enemyManager.Enemies.Any((enemy) => !enemy.IsDead);
    }
}
