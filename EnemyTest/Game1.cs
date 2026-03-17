using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace EnemyTest
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        Dictionary<string, PartDefinition> availableParts;

        private int selectedSlotIndex = 0;
        private List<string> slotNames;
        private SpriteFont debugFont;

        private EnemyInstance myEnemy;

        private KeyboardState prevKBState;
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Example: Setting up a basic humanoid torso
            Texture2D torsoTex = Content.Load<Texture2D>("enemy_0");
            TorsoDefinition crawlerBase = new TorsoDefinition("Crawler", torsoTex);

            debugFont = Content.Load<SpriteFont>("Arial16");

            // Define where limbs attach relative to the center of the torso sprite (0,0)
            //TEMPORARY, will be handled in grafter eventually.
            crawlerBase.AttachmentPoints.Add("Head", new Vector2(-1, -10));  // Up from center
            crawlerBase.AttachmentPoints.Add("RightArm", new Vector2(5, -10)); // Right and slightly up
            crawlerBase.AttachmentPoints.Add("LeftArm", new Vector2(-6.75f, -10));// Left and slightly up
            crawlerBase.AttachmentPoints.Add("RightLeg", new Vector2(5.75f, 8.25f));  // Down-right
            crawlerBase.AttachmentPoints.Add("LeftLeg", new Vector2(-6.5f, 8.25f)); // Down-left

            Viewport vp = GraphicsDevice.Viewport;
            myEnemy = new EnemyInstance(new Vector2(vp.Width / 2, vp.Height / 2), 0, crawlerBase);

            // Load the parts you defined in your Grafter tool
            availableParts = PartLoader.LoadParts("p.csv", Content);

            List<PartDefinition> partList = new(availableParts.Values);

            int partIndex = 0;
            foreach (string slotName in myEnemy.Base.AttachmentPoints.Keys)
            {
                if (partIndex < partList.Count)
                {
                    
                    // Plug the part into the socket
                    myEnemy.EquippedParts[slotName] = partList[partIndex];
                    

                    // Move to the next part in the CSV for the next limb
                    partIndex++;
                }
            }

            Texture2D headTex = Content.Load<Texture2D>("enemy_5");

            Vector2 headPivot = new Vector2(0.45f, 0.67f);

            PartDefinition manualHead = new PartDefinition("Manual Head", headTex, headPivot);

            // Force-equip to the "Head" socket
            myEnemy.EquippedParts["Head"] = manualHead;

            slotNames = new List<string>(myEnemy.Base.AttachmentPoints.Keys);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            KeyboardState kState = Keyboard.GetState();

            // Cycle through slots with Tab
            if (prevKBState.IsKeyUp(Keys.Tab) && kState.IsKeyDown(Keys.Tab))
            {
                selectedSlotIndex = (selectedSlotIndex + 1) % slotNames.Count;
            }

            // Cycle through slots with Tab
            if (prevKBState.IsKeyUp(Keys.Up) && kState.IsKeyDown(Keys.Up))
            {
                myEnemy.Scale++;
            }
            if (prevKBState.IsKeyUp(Keys.Down) && kState.IsKeyDown(Keys.Down))
            {
                myEnemy.Scale--;
            }

            string currentSlot = slotNames[selectedSlotIndex];
            Vector2 currentOffset = myEnemy.Base.AttachmentPoints[currentSlot];

            // WASD Nudging
            if (kState.IsKeyDown(Keys.W)) currentOffset.Y -= 0.25f;
            if (kState.IsKeyDown(Keys.S)) currentOffset.Y += 0.25f;
            if (kState.IsKeyDown(Keys.A)) currentOffset.X -= 0.25f;
            if (kState.IsKeyDown(Keys.D)) currentOffset.X += 0.25f;

            // Apply the update back to the dictionary
            myEnemy.Base.AttachmentPoints[currentSlot] = currentOffset;

            // Run the procedural animation timer for your enemy
            myEnemy.Update(gameTime);

            prevKBState = kState;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);

            myEnemy.Draw(_spriteBatch);

            // DEBUG OVERLAY
            for (int i = 0; i < slotNames.Count; i++)
            {
                string name = slotNames[i];
                Vector2 offset = myEnemy.Base.AttachmentPoints[name];
                Color textColor = (i == selectedSlotIndex) ? Color.Yellow : Color.White;
                string displayText = (i == selectedSlotIndex) ? $"> {name}: {offset} <" : $"{name}: {offset}";

                // Draw the text list in the corner
                _spriteBatch.DrawString(debugFont, displayText, new Vector2(20, 20 + (i * 25)), textColor);
            }

            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }

    // Defines where things can be attached to a specific Torso
    public class TorsoDefinition
    {
        public string Name { get; set; }
        public Texture2D Texture { get; set; }

        // Key: Slot Name (e.g., "LeftArm", "Head")
        // Value: The Vector2 offset from the CENTER of the torso sprite
        public Dictionary<string, Vector2> AttachmentPoints { get; set; } = new();

        public TorsoDefinition(string name, Texture2D texture)
        {
            Name = name;
            Texture = texture;
        }
    }
}
