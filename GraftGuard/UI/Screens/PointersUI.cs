using GraftGuard.Data;
using GraftGuard.Graphics;
using GraftGuard.Map;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using GraftGuard.Utility;
using System.Linq;
using System;
using GraftGuard.Grafting;
using GraftGuard.Map.Enemies;

namespace GraftGuard.UI.Screens;

internal class PointersUI
{
    public static Texture2D TArrow;
    public static Texture2D TGarage;
    private static Texture2D TEnemy;
    public static Texture2D TPart;
    private static Vector2 _partDistanceSource = Vector2.Zero;
    private static Vector2 _enemyDistanceSource = Vector2.Zero;
    private static Func<ScatteredPart, float> _partDistanceSquared = (part) => Vector2.DistanceSquared(part.Position, _partDistanceSource);
    private static Func<Enemy, float> _enemyDistanceSquared = (enemy) => Vector2.DistanceSquared(enemy.Position, _enemyDistanceSource);
    private static Func<Enemy, bool> _enemyIsNotDead = (enemy) => !enemy.IsDead;

    public void Draw(DrawManager drawing, World world)
    {
        _partDistanceSource = world.Camera.Position;
        TimeState state = PlayerData.CurrentGame.Time;

        if (state == TimeState.Day)
        {
            return;
        }

        if (state == TimeState.Dawn)
        {
            if (world.ScatteredParts.Count > 0)
            {
                Vector2 nearestPart = world.ScatteredParts.MinBy(_partDistanceSquared).Position;
                DrawPointer(drawing, world, nearestPart, TPart, 64.0f, color: Color.Lime, cutoffRadius: 0.0f);
            }

            if (world.Player.HeldParts.Count > 0)
            {
                DrawPointer(drawing, world, world.Garage.Center + world.Garage.Size * Vector2.UnitY * 0.5f, TGarage);
            }
        }

        if (state == TimeState.Night)
        {
            if (world.EnemyManager.Enemies.Any(_enemyIsNotDead))
            {
                Enemy closest = null;
                closest = world.EnemyManager.Enemies.Where(_enemyIsNotDead).MinBy(_enemyDistanceSquared);
                DrawPointer(drawing, world, closest.Position, TEnemy, 64.0f, color: Color.Red, cutoffRadius: 0.0f);
            }
        }

        
    }

    public void DrawPointer(DrawManager drawing, World world, Vector2 target, Texture2D icon, float radius = 96.0f, float cutoffRadius = 128.0f, Color? color = null, float scale = 1.0f, float iconScale = 1.0f)
    {
        float distance = Vector2.Distance(world.Camera.Position, target);
        Vector2 position = world.Camera.Position + world.Camera.Position.DirectionTo(target) * radius;
        Color trueColor = new Color(color ?? Color.White, MathHelper.Clamp(distance - 128.0f, 0.0f, 10.0f) * 0.1f);
        drawing.DrawCentered(TArrow, position, rotation: world.Camera.Position.AngleTo(target), drawLayer: 2, color: trueColor, scale: Vector2.One * scale);
        drawing.DrawCentered(icon, position, drawLayer: 2, color: trueColor, sortMode: SortMode.Top, scale: Vector2.One * iconScale);
    }

    public static void LoadContent(ContentManager content)
    {
        TArrow = content.Load<Texture2D>("UI/arrow_base");
        TGarage = content.Load<Texture2D>("UI/base_pointer");
        TEnemy = content.Load<Texture2D>("UI/enemy_pointer");
        TPart = content.Load<Texture2D>("UI/part_pointer");
    }
}
