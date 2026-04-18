using GraftGuard.Grafting;
using GraftGuard.Grafting.Registry;
using GraftGuard.Graphics;
using GraftGuard.Map.Enemies.Animation;
using GraftGuard.Map.Pathing;
using GraftGuard.Utility;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraftGuard.Map.Enemies;
internal class EnemyArachnid : Enemy
{
    public LinkedList<LegPair> Legs = [];
    public const float BaseSpeed = 512.0f;
    public EnemyArachnid(Vector2 position) : base(position, GraftLibrary.GetRandomBase(), new Vector2(64, 64), 128.0f, BaseSpeed)
    {
        DoContactDamage = false;
        Legs.AddLast(new LegPair(
            new AracnidLeg(position, new Vector2(-32, 32), 76.0f, 168.0f, new Vector2(-64, 128)),
            new AracnidLeg(position, new Vector2(32, -32), 76.0f, 168.0f, new Vector2(64, 128))
            ));
        Legs.AddLast(new LegPair(
            new AracnidLeg(position, new Vector2(-32, -32), 76.0f, 168.0f, new Vector2(-64, 128)),
            new AracnidLeg(position, new Vector2(32, 32), 76.0f, 168.0f, new Vector2(64, 128))
            ));
    }

    private bool _legAtGoal(AracnidLeg leg) => Vector2.DistanceSquared(leg.Goal, leg.Position) < 16f * 16f;

    public override void Update(GameTime time, InputManager inputManager, World world, PathManager pathManager)
    {
        base.Update(time, inputManager, world, pathManager);

        foreach (var pair in Legs)
        {
            pair.Update(time, Position);
        }

        LegPair current = Legs.First();
        current.TryMove();
        if (!current.Moving)
        {
            Legs.RemoveFirst();
            Legs.AddLast(current);
        }

        int movingLegs = Legs.Count((pair) => pair.Moving);
        float movingFacor = MathF.Min((float)movingLegs / Legs.Count + 0.6f, 1.0f);

        Speed = BaseSpeed * movingFacor;
    }

    public override void UpdatePathing(GameTime gameTime, InputManager inputManager, World world, PathManager pathManager)
    {
        Position += BasicPathing(gameTime, world, pathManager, PathManager.PathGoal.Player);
    }

    public override void Draw(GameTime gameTime, DrawManager drawing, InputManager inputManager, World world)
    {
        base.Draw(gameTime, drawing, inputManager, world);
        foreach (var leg in Legs)
        {
            leg.DebugDraw(drawing);
        }
        LegPair first = Legs.First.Value;
        drawing.DrawCircle(first.First.FootPosition, 16.0f, color: Color.BurlyWood);
        drawing.DrawCircle(first.Second.FootPosition, 16.0f, color: Color.BurlyWood);
    }
}

internal class LegPair
{
    public AracnidLeg First;
    public AracnidLeg Second;
    public bool Moving => First.Moving || Second.Moving;
    public LegPair(AracnidLeg first, AracnidLeg second)
    {
        First = first;
        Second = second;
    }
    public void Update(GameTime time, Vector2? newSource = null)
    {
        if (newSource is Vector2 source)
        {
            First.SourcePosition = source;
            Second.SourcePosition = source;
        }

        First.Update(time);
        Second.Update(time);
    }
    public void DebugDraw(DrawManager drawing)
    {
        First.DrawDebug(drawing);
        Second.DrawDebug(drawing);
    }
    public void TryMove()
    {
        if (First.IsGoalTooFar(40f, 10f) || First.IsGoalTooClose(16.0f))
        {
            First.Moving = true;
            First.Goal = First.Position + First.GoalOffset;
        }
        if (Second.IsGoalTooFar(40f, 10f) || Second.IsGoalTooClose(16.0f))
        {
            Second.Moving = true;
            Second.Goal = Second.Position + Second.GoalOffset;
        }
    }
}

internal class AracnidLeg : IKLimb
{
    public Vector2 GoalOffset;
    public Vector2 PositionOffset;
    public Vector2 SourcePosition;
    public bool Moving = false;
    public AracnidLeg(Vector2 sourcePosition, Vector2 positionOffset, float upperLength, float lowerLength, Vector2 offset) : base(sourcePosition, upperLength, lowerLength)
    {
        Side = MathF.Sign(offset.X);
        GoalOffset = offset;
        PositionOffset = positionOffset;
        SourcePosition = sourcePosition;
        Position = SourcePosition + PositionOffset;
        Goal = Position + offset;
    }
    public override void Update(GameTime time)
    {
        Position = SourcePosition + PositionOffset;
        base.Update(time);
        if (AtGoal)
        {
            Moving = false;
        }
    }
    public override void DrawDebug(DrawManager drawing)
    {
        base.DrawDebug(drawing);
        if (AtGoal)
        {
            drawing.DrawCircle(FootPosition, 4.0f, color: Color.Lime, drawLayer: 2);
        }
        if (IsGoalTooFar())
        {
            drawing.DrawCircle(FootPosition, 3.0f, color: Color.Orange, drawLayer: 3);
        }
    }

    public override bool IsGoalTooFar(float distance = 0.0f)
    {
        return Math.Sign(Goal.X - Position.X) != Side || base.IsGoalTooFar(distance);
    }
    public bool IsGoalTooFar(float distance, float insideDistance)
    {
        return Math.Sign(Goal.X - Position.X + (insideDistance * -Side)) != Side || base.IsGoalTooFar(distance);
    }
}