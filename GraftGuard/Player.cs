using GraftGuard.Grafting;
using GraftGuard.Grafting.Registry;
using GraftGuard.UI;
using GraftGuard.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace GraftGuard
{
    internal class Player : GameObject
    {
        public const int MaxHeldParts = 4;
        public const float PickupRadius = 32;
        public static readonly Vector2 CenterOffset = new Vector2(25, 50) * 0.5f;

        private static readonly float Speed = 600;
        private static Texture2D texture;

        public List<PartDefinition> HeldParts { get; private set; }
        private Circle _collectionCircle;
        public bool InventoryFull => HeldParts.Count >= MaxHeldParts;

        public static void LoadContent(ContentManager content)
        {
            Player.texture = content.Load<Texture2D>("playerplaceholder");
        }

        public Player(Vector2 position) : base(position, new Vector2(25, 50), Player.texture, collisionLayers: CollisionLayer.Player, collisionMasks: CollisionLayer.Solid | CollisionLayer.Terrain)
        {
            _collectionCircle = new Circle(CenterOffset, PickupRadius);
            HeldParts = [];
        }

        public void Update(GameTime gameTime, InputManager inputManager, World world)
        {
            // Delta Time
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Get Movement from Input
            Vector2 moveVector = inputManager.GetMovementDirection();

            // Move with Collision
            Move(moveVector * Speed * delta, world);

            // Set the Camera's position
            world.Camera.Position = Position - Interface.ScreenCenter;

            HandlePartPickups(world);
        }

        public override void Draw(GameTime gameTime, SpriteBatch batch)
        {
            base.Draw(gameTime, new Rectangle(Position.ToPoint(), new Point(25, 50)), batch);

            // Draw Held Parts
            for (int index = 0; index < HeldParts.Count; index++)
            {
                Texture2D part = HeldParts[index].Texture;
                batch.Draw(part, Position - Vector2.UnitY * (index - 2) * 8, null, Color.White, -MathF.PI / 2.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);
            }
        }

        /// <summary>
        /// Handles picking up <see cref="ScatteredPart"/>s in the provided<see cref="World"/>. Should be called from <see cref="Update"/>
        /// </summary>
        /// <param name="world">World to use</param>
        private void HandlePartPickups(World world)
        {
            // Get all overlapping ScatteredParts
            List<ScatteredPart> scatteredParts = GameObject.GetIntersects<ScatteredPart>(
                _collectionCircle.Translated(Position),
                layers: CollisionLayer.ScatteredPart,
                world.ScatteredParts
                );

            // Pickup overlapping parts
            foreach (ScatteredPart part in scatteredParts)
            {
                PickupPart(part, world);
                // Early break if can't hold more parts
                if (HeldParts.Count >= MaxHeldParts)
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Removes a <see cref="ScatteredPart"/> from the world and adds it's <see cref="PartDefinition"/> to <see cref="HeldParts"/>. This method does nothing if <see cref="HeldParts"/> is full
        /// </summary>
        /// <param name="scatteredPart">Scattered Part to pickup</param>
        /// <param name="world"><see cref="World"/> to use</param>
        public void PickupPart(ScatteredPart scatteredPart, World world)
        {
            // Don't do anything if the inventory is full
            if (InventoryFull)
            {
                return;
            }

            // Add part definition to _heldParts
            HeldParts.Add(scatteredPart.Definition);

            // Remove ScatteredPart from the World
            world.ScatteredParts.Remove(scatteredPart);
        }
    }
}
