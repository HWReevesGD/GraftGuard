using GraftGuard.Grafting;
using GraftGuard.Grafting.Registry;
using GraftGuard.Graphics;
using GraftGuard.Map.Enemies.Animation;
using GraftGuard.Map.Pathing;
using GraftGuard.Map.Projectiles;
using GraftGuard.UI;
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
    public const float BaseSpeed = 256.0f;
    public IntervalTimer ShootTimer = new IntervalTimer(6.0f);
    public IntervalTimer FiringTimer = new IntervalTimer(0.1f);
    public const float ShootTime = 4.0f;
    public float ShootTimeLeft { get; private set; }
    public static Random Random = new Random();
    public bool IsCircling = false;
    public bool IsShooting = false;
    public EnemyArachnid(Vector2 position) : base(position, GraftLibrary.GetBaseByName("Arachnid"), new Vector2(64, 64), 128.0f, BaseSpeed)
    {
        DoContactDamage = false;
        Legs.AddLast(new LegPair(
            new AracnidLeg(position, new Vector2(-24, 32), 76.0f, 168.0f, new Vector2(-64, 128)),
            new AracnidLeg(position, new Vector2(24, -32), 76.0f, 168.0f, new Vector2(64, 128))
            ));
        Legs.AddLast(new LegPair(
            new AracnidLeg(position, new Vector2(-24, -32), 76.0f, 168.0f, new Vector2(-64, 128)),
            new AracnidLeg(position, new Vector2(24, 32), 76.0f, 168.0f, new Vector2(64, 128))
            ));
    }

    public override void Update(GameTime time, InputManager inputManager, World world, PathManager pathManager)
    {
        base.Update(time, inputManager, world, pathManager);

        if (IsCircling)
        {
            if (!IsShooting)
            {
                ShootTimeLeft = ShootTime;
                IsShooting = ShootTimer.Update(time);
            }
        }

        foreach (var pair in Legs)
        {
            pair.Update(time, Position);
        }

        // Update Legs
        LegPair current = Legs.First();
        current.TryMove();
        if (!current.Moving)
        {
            Legs.RemoveFirst();
            Legs.AddLast(current);
        }

        int movingLegs = Legs.Count((pair) => pair.Moving);
        float movingFactor = MathF.Min((float)movingLegs / Legs.Count + 0.8f, 1.0f);

        Speed = BaseSpeed * movingFactor;

        // Shooting Logic
        if (IsShooting)
        {
            if (ShootTimeLeft <= 0.0f)
            {
                IsShooting = false;
                ShootTimeLeft = 0.0f;
            }

            ShootTimeLeft -= time.Delta();
            bool fire = FiringTimer.Update(time);
            if (fire)
            {
                world.ProjectileManager.Add(new ProjectileDarkOrb(Position, Position.AngleTo(world.Player.Position + world.Player.LastMovement) + MathF.PI * 0.5f + Random.NextSingle() * 0.1f, ProjectileTarget.Player));
            }
        }
    }

    public override void UpdatePathing(GameTime gameTime, InputManager inputManager, World world, PathManager pathManager)
    {
        if (IsShooting)
        {
            return;
        }
        float distanceToPLayer = Vector2.Distance(Position, world.Player.Position);

        Vector2 directionToPlayer = (world.Player.Position - Position).Normalized();

        if (distanceToPLayer > 512.0f)
        {
            IsCircling = false;
            Position += directionToPlayer * Speed * gameTime.Delta();
        }
        else
        {
            IsCircling = true;
            directionToPlayer.Rotate(MathF.PI * 0.5f);
            Position += directionToPlayer * Speed * gameTime.Delta();
        }

    }

    public override void Draw(GameTime gameTime, DrawManager drawing, InputManager inputManager, World world)
    {
        base.Draw(gameTime, drawing, inputManager, world);
        foreach (var legPair in Legs)
        {
            //legPair.DebugDraw(drawing);
            DrawLeg(legPair.First, drawing);
            DrawLeg(legPair.Second, drawing);
        }
        //LegPair first = Legs.First.Value;
        //drawing.DrawCircle(first.First.FootPosition, 16.0f, color: Color.BurlyWood);
        //drawing.DrawCircle(first.Second.FootPosition, 16.0f, color: Color.BurlyWood);
        //drawing.DrawString($"TIME: {ShootTimer.TimeLeft}", Position);
    }
    public void DrawLeg(AracnidLeg leg, DrawManager drawing)
    {
        drawing.Draw(TArachnidShin, leg.KneePosition, origin: new Vector2(TArachnidShin.Width / 2, 20.0f), rotation: leg.KneePosition.AngleTo(leg.FootPosition));
        drawing.Draw(TArachnidUpper, leg.Position, origin: new Vector2(TArachnidUpper.Width / 2, 4.0f), rotation: leg.Position.AngleTo(leg.KneePosition), sortMode: SortMode.Top);
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
        if (First.IsGoalTooFar(32f, 10f) || First.IsGoalTooClose(16.0f) || First.AngleToFar(-0.2f))
        {
            First.Moving = true;
            First.Goal = First.Position + First.GoalOffset;
        }
        if (Second.IsGoalTooFar(32f, 10f) || Second.IsGoalTooClose(16.0f) || First.AngleToFar(-0.2f))
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
        drawing.DrawString($"{MathHelper.WrapAngle(MainAngle) * 180.0f / MathF.PI}", FootPosition, Fonts.SubFont);
    }

    public override bool IsGoalTooFar(float distance = 0.0f)
    {
        return Math.Sign(Goal.X - Position.X) != Side || base.IsGoalTooFar(distance);
    }
    public bool IsGoalTooFar(float distance, float insideDistance)
    {
        return Math.Sign(Goal.X - Position.X + (insideDistance * -Side)) != Side || base.IsGoalTooFar(distance);
    }
    public bool AngleToFar(float angleOffset)
    {
        return MathHelper.WrapAngle(MainAngle) <= -180 + angleOffset || MathHelper.WrapAngle(MainAngle) >= 0 - angleOffset;
    }
}