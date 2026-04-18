using GraftGuard.Grafting;
using GraftGuard.Grafting.Registry;
using GraftGuard.Graphics;
using GraftGuard.Map.Pathing;
using GraftGuard.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GraftGuard.Map.Pathing.PathManager;

namespace GraftGuard.Map.Enemies;
internal class EnemyCentipede : Enemy
{
    static BaseDefinition shell = GraftLibrary.GetBaseByName("Shell");
    private Vector2 movement;
    private float mandibleRotation;
    public const int CentipedeLength = 6;
    public const int SanpshotsPerSegment = 3;
    public RotatingArray<Vector2> SegmentPositions;
    public EnemyCentipede Parent { get; private set; }
    public EnemyCentipede Child { get; private set; }
    public EnemyCentipede(Vector2 position, EnemyManager manager, int segmentsLeft = CentipedeLength)
        : base(position, shell, new Vector2(32, 32), 10.0f, 760.0f)
    {
        Mass = 4.0f;
        segmentsLeft--;
        if (segmentsLeft > 0)
        {
            Child = new EnemyCentipede(position, manager, segmentsLeft);
            manager.Enemies.Add(Child);
        }
        PathTimer = new IntervalTimer(1.0f);
        SegmentPositions = new RotatingArray<Vector2>((segmentsLeft + 2) * SanpshotsPerSegment, position);

        EnemyCentipede parent = this;
        EnemyCentipede child = Child;
        while (child is not null)
        {
            child.Parent = parent;
            parent = child;
            child = child.Child;
        }
    }

    public override void Update(GameTime gameTime, InputManager inputManager, World world, PathManager pathManager)
    {
        base.Update(gameTime, inputManager, world, pathManager);
        SegmentPositions.Add(Position);
        if (Parent is not null)
        {
            return;
        }

        EnemyCentipede child = Child;
        int childIndex = 0;
        while (child is not null)
        {
            child.Position = SegmentPositions[childIndex * SanpshotsPerSegment];
            child = child.Child;
            childIndex++;
        }
    }

    public override void OnDeath()
    {
        base.OnDeath();
        if (Parent is not null)
        {
            Parent.Child = null;
        }
        if (Child is not null)
        {
            Child.Parent = null;
        }

    }

    public override void UpdatePathing(GameTime gameTime, InputManager inputManager, World world, PathManager pathManager)
    {
        if (Parent is not null)
        {
            return;
        }

        List<Enemy> children = [];

        EnemyCentipede child = Child;
        int childIndex = 0;
        while (child is not null)
        {
            children.Add(child);
            child = child.Child;
            childIndex++;
        }

        Vector2 steeringPathing = BasicPathing(gameTime, world, pathManager, PathGoal.Player, children);
        movement = steeringPathing;
        steeringPathing += new Vector2(0, MathF.Sin(gameTime.Total()) * 128.0f * gameTime.Delta());
        steeringPathing.Rotate(steeringPathing.OppositeAngle());
        Position += movement;
    }

    public override void Draw(GameTime gameTime, DrawManager drawing, InputManager inputManager, World world)
    {
        base.Draw(gameTime, drawing, inputManager, world);

        if (Parent is null)
        {
            mandibleRotation = mandibleRotation.MoveTowardsAngle(movement.OppositeAngle(), gameTime.Delta() * 10.0f);
            drawing.DrawCentered(TCentipedeMandible, Position, origin: new Vector2(-16, 0), rotation: mandibleRotation);
        }
    }
}