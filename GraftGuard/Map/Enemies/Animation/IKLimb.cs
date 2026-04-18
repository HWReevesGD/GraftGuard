using GraftGuard.Graphics;
using GraftGuard.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraftGuard.Map.Enemies.Animation;
internal class IKLimb
{
    public IKLimb(Vector2 position, float upperLength, float lowerLength)
    {
        UpperLength = upperLength;
        LowerLength = lowerLength;

        Position = position;
        Goal = position;// + new Vector2(64, 0);
    }

    public Vector2 Position { get; set; }
    public Vector2 CurrentGoal { get; private set; }
    public Vector2 Goal { get; set; }
    public int Side { get; set; }

    public bool AtGoal => Vector2.DistanceSquared(Goal, CurrentGoal) < 3.0f;
    public virtual bool IsGoalTooFar(float distance = 0.0f)
    {
        return Vector2.Distance(Position, Goal) > (upperLength + lowerLength - distance);
    }
    public virtual bool IsGoalTooClose(float distance = 0.0f)
    {
        return Vector2.Distance(Position, Goal) < (lowerLength - upperLength + distance);
    }

    private float upperLength;
    private float lowerLength;
    public float UpperLength
    {
        get => upperLength; set
        {
            upperLength = value;
            UpperSquare = value * value;
        }
    }
    public float LowerLength
    {
        get => lowerLength; set
        {
            lowerLength = value;
            LowerSquare = value * value;
        }
    }

    public float UpperSquare { get; private set; }
    public float LowerSquare { get; private set; }

    public Vector2 KneePosition { get; private set; }
    public Vector2 FootPosition { get; private set; }

    private float MainAngle = 0.0f;
    private float KneeAngle = 0.0f;
    //private float FootAngle = 0.0f;
    private float OuterAngle = 0.0f;

    public virtual void Update(GameTime time)
    {
        if (!AtGoal)
        {
            CurrentGoal = Vector2.Lerp(CurrentGoal, Goal, 0.4f);
        }
        else
        {
            CurrentGoal = Goal;
        }

            float dist = Vector2.Distance(Position, CurrentGoal);
        dist = MathHelper.Clamp(dist, MathF.Abs(UpperLength - LowerLength), UpperLength + LowerLength);
        float distSquare = dist * dist;

        MainAngle = (CurrentGoal - Position).Angle() - MathF.PI / 2.0f;

        KneeAngle = -Side * MathF.Acos((UpperSquare + distSquare - LowerSquare) / (2 * UpperLength * dist));
        //FootAngle = sign * MathF.Acos((NearSquare + FarSquare - distSquare) / (2 * NearLength * FarLength));
        OuterAngle = -Side * MathF.Acos((LowerSquare + distSquare - UpperSquare) / (2 * LowerLength * dist));

        KneePosition = Position + (KneeAngle - MainAngle).AngleToVector() * UpperLength;
        FootPosition = KneePosition + (-OuterAngle - MainAngle).AngleToVector() * LowerLength;
    }

    public virtual void DrawDebug(DrawManager drawing)
    {
        drawing.DrawCircle(Position, 8.0f, color: Color.Red);
        drawing.DrawCircle(KneePosition, 3.0f, color: Color.Blue);
        drawing.DrawCircle(FootPosition, 4.0f, color: Color.Green);

        drawing.Draw(Placeholders.TexturePixel, Position, rotation: KneeAngle - MainAngle - MathF.PI / 2.0f, scale: new Vector2(1, upperLength));
        drawing.Draw(Placeholders.TexturePixel, KneePosition, rotation: -OuterAngle - MainAngle - MathF.PI / 2.0f, scale: new Vector2(1, lowerLength));

        drawing.DrawString($"{Position}, {KneePosition}, {FootPosition}", Position, color: Color.Black);
    }
}
