using GraftGuard.Grafting.Registry;
using GraftGuard.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace GraftGuard.Map.Enemies
{
    public class FallingPart
    {
        public PartDefinition Definition { get; private set; }
        private Vector2 position;
        private Vector2 velocity;
        private float zHeight; // Simulated height
        private float zVelocity;
        private float rotation;
        private float rotationSpeed;
        private float _gravity = 15f;

        public bool HasLanded { get; private set; }

        public FallingPart(PartDefinition def, Vector2 startPos, Vector2 direction)
        {
            Definition = def;
            position = startPos;
            // Randomize outward force
            velocity = direction * (float)(new Random().NextDouble() * 100 + 50);
            zVelocity = (float)(new Random().NextDouble() * -200 - 100); // Initial upward "pop"
            rotationSpeed = (float)(new Random().NextDouble() * 10 - 5);
        }

        public void Update(float deltaTime)
        {
            if (HasLanded) return;

            position += velocity * deltaTime;
            zVelocity += _gravity;
            zHeight += zVelocity * deltaTime;
            rotation += rotationSpeed * deltaTime;

            // Check if it hit the "ground" (z >= 0)
            if (zHeight >= 0)
            {
                zHeight = 0;
                HasLanded = true;
            }
        }

        internal void Draw(DrawManager drawing)
        {
            // Apply zHeight to the Y position to simulate depth
            Vector2 drawPos = new Vector2(position.X, position.Y + zHeight);
            drawing.Draw(
                texture: Definition.Texture,
                position: drawPos,
                rotation: rotation,
                origin: new Vector2(Definition.Texture.Width / 2, Definition.Texture.Height / 2));
        }

        public Vector2 GetLandingPosition() => position;
    }
}
