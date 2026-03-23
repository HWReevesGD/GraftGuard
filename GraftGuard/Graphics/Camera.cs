using GraftGuard.Map;
using GraftGuard.Utility;
using Microsoft.Xna.Framework;
using System;

namespace GraftGuard.Graphics;
internal class Camera
{
    public Vector2 Position { get; set; } = Vector2.Zero;
    public float Zoom { get; private set; } = 1f;           
    public float MinZoom { get; set; } = 0.5f;
    public float MaxZoom { get; set; } = 5.0f;
    public float ZoomSpeed { get; set; } = 8f;

    private float targetZoom = 1f;

    public Matrix ScreenToWorld => Matrix.Invert(WorldToScreen);
    public Matrix WorldToScreen =>
        Matrix.CreateTranslation(new Vector3(-Position, 0)) * // Move player to origin
        Matrix.CreateScale(Zoom, Zoom, 1) * Matrix.CreateTranslation(new Vector3(Interface.ScreenCenter, 0)); 


    public void AdjustZoom(float amount)
    {
        targetZoom += amount;
        targetZoom = MathHelper.Clamp(targetZoom, MinZoom, MaxZoom);

    }

    public void Update(GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        // Smoothly move Zoom toward targetZoom
        Zoom = MathHelper.Lerp(Zoom, targetZoom, ZoomSpeed * dt);

        if (Math.Abs(Zoom - targetZoom) < 0.001f) Zoom = targetZoom;
    }

    public void UpdateFreeMovement(GameTime time, InputManager input)
    {
        Vector2 movement = input.GetMovementDirection() * Player.Speed;
        Position += movement * (float)time.ElapsedGameTime.TotalSeconds * (1/Zoom);
    }
}
