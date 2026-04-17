using GraftGuard.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace GraftGuard.Graphics.Particles;

/// <summary>
/// Particle visuals and simulation
/// </summary>
internal class Particle
{
    private Texture2D texture;
    private float speed = 5f;
    private float lifetime = 0.5f;
    private float angle = 0f;
    private float rotation = 0f;
    private Vector2 startSize = Vector2.One;
    private Vector2 endSize = Vector2.One;
    private Color startColor = Color.White;
    private Color endColor = Color.White;
    private Vector2 position = Vector2.Zero;
    private float rotVelocity = 0f;
    private Vector2 acceleration = Vector2.Zero;

    private Vector2 velocity;
    private float timeElapsed;

    private Random rng = new Random();

    /// <summary>
    /// Initialize particle to this texture and some default fields
    /// </summary>
    /// <param name="texture">particle texture</param>
    public Particle(Texture2D texture)
    {
        this.texture = texture;
    }

    /// <summary>
    /// Set constant particle speed
    /// </summary>
    /// <param name="speed">Speed</param>
    /// <returns>this</returns>
    public Particle SetSpeed(float speed)
    {
        this.speed = speed;
        return this;
    }

    /// <summary>
    /// Set particle speed to be random in the given range
    /// </summary>
    /// <param name="minSpeed">Minimum particle speed</param>
    /// <param name="maxSpeed">Maximum particle speed</param>
    /// <returns>this</returns>
    public Particle SetSpeed(float minSpeed, float maxSpeed)
    {
        SetSpeed(minSpeed + (float)rng.NextDouble() * (maxSpeed - minSpeed));
        return this;
    }

    /// <summary>
    /// Set particle lifetime
    /// </summary>
    /// <param name="lifetime">Lifetime in seconds</param>
    /// <returns>this</returns>
    public Particle SetLifetime(float lifetime)
    {
        this.lifetime = lifetime;
        return this;
    }

    /// <summary>
    /// Set particles lifetime to be random within the given range
    /// </summary>
    /// <param name="minLifetime">Minimum lifetime in seconds</param>
    /// <param name="maxLifetime">Maximum lifetime in seconds</param>
    /// <returns>this</returns>
    public Particle SetLifetime(float minLifetime, float maxLifetime)
    {
        SetLifetime(minLifetime + (float)rng.NextDouble() * (maxLifetime - minLifetime));
        return this;
    }

    /// <summary>
    /// Set angle of movement direction
    /// </summary>
    /// <param name="angle">Angle in radians, starting from the right vector, counterclockwise</param>
    /// <returns>this</returns>
    public Particle SetAngle(float angle)
    {
        this.angle = angle;
        return this;
    }

    /// <summary>
    /// Set movement angle to be random within a certain range
    /// </summary>
    /// <param name="minAngle">Minimum angle in radians</param>
    /// <param name="maxAngle">Maximum angle in radians</param>
    /// <returns>this</returns>
    public Particle SetAngle(float minAngle, float maxAngle)
    {
        SetAngle(minAngle + (float)rng.NextDouble() * (maxAngle - minAngle));
        return this;
    }

    /// <summary>
    /// Set particle rotation
    /// </summary>
    /// <param name="rotation">Particle rotation in radians</param>
    /// <returns>this</returns>
    public Particle SetRotation(float rotation)
    {
        this.rotation = rotation;
        return this;
    }

    /// <summary>
    /// Set particle rotation to be random in the given range
    /// </summary>
    /// <param name="minRotation">Minimum rotation in radians</param>
    /// <param name="maxRotation">Maximum rotation in radians</param>
    /// <returns>this</returns>
    public Particle SetRotation(float minRotation, float maxRotation)
    {
        SetRotation(minRotation + (float)rng.NextDouble() * (maxRotation - minRotation));
        return this;
    }

    /// <summary>
    /// Set particle rotational velocity
    /// </summary>
    /// <param name="rotVelocity">Rotational velocity in radians/s</param>
    /// <returns>this</returns>
    public Particle SetRotVelocity(float rotVelocity)
    {
        this.rotVelocity = rotVelocity;
        return this;
    }

    /// <summary>
    /// Set particle rotation velocity to be random in the given range
    /// </summary>
    /// <param name="minRotVelocity">Minimum rotational velocity in radians</param>
    /// <param name="maxRotVelocity">Maximum rotational velocity in radians</param>
    /// <returns>this</returns>
    public Particle SetRotVelocity(float minRotVelocity, float maxRotVelocity)
    {
        SetRotVelocity(minRotVelocity + (float)rng.NextDouble() * (maxRotVelocity - minRotVelocity));
        return this;
    }

    /// <summary>
    /// Set particle size at the start and ennd
    /// </summary>
    /// <param name="startSize">Start size</param>
    /// <param name="endSize">End size</param>
    /// <returns>this</returns>
    public Particle SetSize(Vector2 startSize, Vector2 endSize)
    {
        this.startSize = startSize;
        this.endSize = endSize;
        return this;
    }

    /// <summary>
    /// Set static particle size
    /// </summary>
    /// <param name="size">Particle size</param>
    /// <returns>this</returns>
    public Particle SetSize(Vector2 size)
    {
        SetSize(size, size);
        return this;
    }

    /// <summary>
    /// Set particle color at the start and end
    /// </summary>
    /// <param name="startColor">Start color</param>
    /// <param name="endColor">End color</param>
    /// <returns>this</returns>
    public Particle SetColor(Color startColor, Color endColor)
    {
        Color newStartColor = new Color(startColor.R, startColor.G, startColor.B, this.startColor.A);
        Color newEndColor = new Color(endColor.R, endColor.G, endColor.B, this.startColor.B);
        this.startColor = newStartColor;
        this.endColor = newEndColor;
        return this;
    }

    /// <summary>
    /// Set static particle color
    /// </summary>
    /// <param name="color">Color</param>
    /// <returns>this</returns>
    public Particle SetColor(Color color)
    {
        SetColor(color, color);
        return this;
    }

    /// <summary>
    /// Set start and end opacity
    /// </summary>
    /// <param name="startOpacity">Start opacity</param>
    /// <param name="endOpacity">End opacity</param>
    /// <returns>this</returns>
    public Particle SetOpacity(float startOpacity, float endOpacity)
    {
        Color newStartColor = new Color(startColor.R, startColor.G, startColor.B, startOpacity);
        Color newEndColor = new Color(endColor.R, endColor.G, endColor.B, endOpacity);
        SetColor(newStartColor, newEndColor);
        return this;
    }

    /// <summary>
    /// Set static particle opacity
    /// </summary>
    /// <param name="opacity">Particle opacity</param>
    /// <returns>this</returns>
    public Particle SetOpacity(float opacity)
    {
        SetOpacity(opacity, opacity);
        return this;
    }

    /// <summary>
    /// Set particle position
    /// </summary>
    /// <param name="position">Position</param>
    /// <returns>this</returns>
    public Particle SetPosition(Vector2 position)
    {
        this.position = position;
        return this;
    }

    /// <summary>
    /// Set particle acceleration
    /// </summary>
    /// <param name="acceleration">Acceleration in pixels/s^2</param>
    /// <returns>this</returns>
    public Particle SetAcceleration(Vector2 acceleration)
    {
        this.acceleration = acceleration;
        return this;
    }

    /// <summary>
    /// Sets initial velocity according to speed and angle. *Should* only be used by ParticleManager.
    /// </summary>
    internal void SetInitialVelocity()
    {
        this.velocity = new Vector2(
            (float)Math.Cos(angle) * speed,
            (float)Math.Sin(angle) * speed
            );
    }

    /// <summary>
    /// Update particle position and lifetime
    /// </summary>
    /// <param name="gameTime">This frame's GameTime</param>
    /// <returns>Whether or not the particle should still be used</returns>
    public bool Update(GameTime gameTime)
    {
        float delta = gameTime.Delta();
        position += velocity * delta;
        velocity += acceleration * delta;
        rotation += rotVelocity * delta;

        timeElapsed += delta;
        return timeElapsed < lifetime;
    }

    /// <summary>
    /// Draw this particle
    /// </summary>
    /// <param name="drawing">SpriteBatch</param>
    /// <param name="gametime">This frame's GameTime</param>
    public void Draw(DrawManager drawing, GameTime gametime, bool isUi, int drawLayer)
    {
        float progress = Math.Min(timeElapsed / lifetime, 1);

        Vector2 scale = new Vector2(
            MathHelper.Lerp(startSize.X, endSize.X, progress),
            MathHelper.Lerp(startSize.Y, endSize.Y, progress)
            );
        Color color = startColor.Lerp(endColor, progress);

        drawing.DrawCentered(
            texture,
            position, 
            scale: scale,
            color: color,
            rotation: rotation,
            drawLayer: drawLayer,
            isUi: isUi);
    }
}
