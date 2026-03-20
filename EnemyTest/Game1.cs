using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;

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

        //edit mode
        private bool isEditMode = true;
        private int selectedDebugIndex = 0;
        private List<string> debugOptions = new List<string>();

        // Keys for toggling clips
        private Keys toggleClipKey = Keys.C;

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
            myEnemy = new EnemyInstance(new Vector2(vp.Width / 2, vp.Height / 2), 0, 4, crawlerBase);

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
            KeyboardState kState = Keyboard.GetState();
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (kState.IsKeyDown(Keys.Space) && prevKBState.IsKeyUp(Keys.Space))
            {
                isEditMode = !isEditMode;
            }

            if (isEditMode)
            {

                // --- EDIT MODE ---

                myEnemy.EnemyAnimator.IsDebugMode = true;

                debugOptions.Clear();
                debugOptions.AddRange(slotNames); 
                debugOptions.Add("StrideLength");
                debugOptions.Add("BobIntensity");
                debugOptions.Add("SwayIntensity");
                debugOptions.Add("LimbWobble");
                debugOptions.Add("LeanFactor");

                // Cycle through slots with Tab
                if (prevKBState.IsKeyUp(Keys.Tab) && kState.IsKeyDown(Keys.Tab))
                {
                    selectedDebugIndex = (selectedDebugIndex + 1) % debugOptions.Count;
                }
              
                //Toggle Clips manually to test settings
                if (kState.IsKeyDown(toggleClipKey) && prevKBState.IsKeyUp(toggleClipKey))
                {
                    Debug.WriteLine($"Current Clip Before: {myEnemy.EnemyAnimator.CurrentClip.ClipName}");
                    myEnemy.EnemyAnimator.CurrentClip = (myEnemy.EnemyAnimator.CurrentClip == EnemyInstance.Walk)
                                       ? EnemyInstance.Idle : EnemyInstance.Walk;
                    Debug.WriteLine($"Current Clip After: {myEnemy.EnemyAnimator.CurrentClip.ClipName}");
                }

                float changeDir = 0;

                if (kState.IsKeyDown(Keys.W) || kState.IsKeyDown(Keys.Up) || kState.IsKeyDown(Keys.D) || kState.IsKeyDown(Keys.Right))
                    changeDir = 1;
                else if (kState.IsKeyDown(Keys.S) || kState.IsKeyDown(Keys.Down) || kState.IsKeyDown(Keys.A) || kState.IsKeyDown(Keys.Left))
                    changeDir = -1;

                if (changeDir != 0)
                {
                    string target = debugOptions[selectedDebugIndex];
                    ApplyDebugChange(kState, target, changeDir, deltaTime);
                }

            }
            else
            {
                myEnemy.EnemyAnimator.IsDebugMode = false;

                //MOVED OUT OF DEBUG SINCE IT WAS INTERFERING WITH DATA MOD
                // Scale with up and down, could be interpreted as zoom
                if (prevKBState.IsKeyUp(Keys.Up) && kState.IsKeyDown(Keys.Up))
                {
                    myEnemy.Scale++;
                }
                if (prevKBState.IsKeyUp(Keys.Down) && kState.IsKeyDown(Keys.Down))
                {
                    myEnemy.Scale--;
                }


                // --- ENEMY MOVE MODE ---
                float moveSpeed = 200f * (float)gameTime.ElapsedGameTime.TotalSeconds;
                Vector2 moveDir = Vector2.Zero;

                if (kState.IsKeyDown(Keys.W)) moveDir.Y -= 1;
                if (kState.IsKeyDown(Keys.S)) moveDir.Y += 1;
                if (kState.IsKeyDown(Keys.A)) moveDir.X -= 1;
                if (kState.IsKeyDown(Keys.D)) moveDir.X += 1;

                if (moveDir != Vector2.Zero)
                {
                    moveDir.Normalize();
                    myEnemy.Position += moveDir * moveSpeed;
                }
            }

            // Update the procedural animation timer
            myEnemy.Update(gameTime);

            prevKBState = kState;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {

            GraphicsDevice.Clear(Color.CornflowerBlue);
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);

            myEnemy.Draw(_spriteBatch);

            string lockStatus = myEnemy.EnemyAnimator.IsDebugMode ? "[LOCKED]" : "[AUTO]";

            _spriteBatch.DrawString(debugFont, $"Clip: {myEnemy.EnemyAnimator.CurrentClip.ClipName} {lockStatus}", new Vector2(20, 20), Color.Magenta);

            if (isEditMode)
            {
                // Header
            
                for (int i = 0; i < debugOptions.Count; i++)
                {
                    string target = debugOptions[i];
                    string valueDisplay = "";

                    // Pull current value for display
                    if (slotNames.Contains(target))
                    {
                        valueDisplay = myEnemy.Base.AttachmentPoints[target].ToString();
                    }
                    else
                    {
                        var clip = myEnemy.EnemyAnimator.CurrentClip;
                        valueDisplay = target switch
                        {
                            "StrideLength" => clip.StrideLength.ToString("F2"),
                            "BobIntensity" => clip.BobIntensity.ToString("F1"),
                            "SwayIntensity" => clip.SwayIntensity.ToString("F1"),
                            "LimbWobble" => clip.LimbWobbleScale.ToString("F2"),
                            "LeanFactor" => clip.LeanFactor.ToString("F2"),
                            _ => ""
                        };
                    }

                    Color textColor = (i == selectedDebugIndex) ? Color.Yellow : Color.White;
                    string text = (i == selectedDebugIndex) ? $"> {target}: {valueDisplay} <" : $"{target}: {valueDisplay}";

                    _spriteBatch.DrawString(debugFont, text, new Vector2(20, 45 + (i * 25)), textColor);
                }
            }

            _spriteBatch.End();
            base.Draw(gameTime);
        }

        private void ApplyDebugChange(KeyboardState kState, string target, float dir, float dt)
        {
            float pivotSpeed = 100f * dt;
            float animSpeed = 2f * dt;
            var clip = myEnemy.EnemyAnimator.CurrentClip;

            if (slotNames.Contains(target))
            {
                Vector2 offset = myEnemy.Base.AttachmentPoints[target];
                if (kState.IsKeyDown(Keys.W) || kState.IsKeyDown(Keys.S) || kState.IsKeyDown(Keys.Up) || kState.IsKeyDown(Keys.Down))
                    offset.Y += dir * pivotSpeed;
                else
                    offset.X += dir * pivotSpeed;

                myEnemy.Base.AttachmentPoints[target] = offset;
            }
            else
            {
                switch (target)
                {
                    case "StrideLength": clip.StrideLength += dir * animSpeed * 0.1f; break;
                    case "BobIntensity": clip.BobIntensity += dir * animSpeed; break;
                    case "SwayIntensity": clip.SwayIntensity += dir * animSpeed; break;
                    case "LimbWobble": clip.LimbWobbleScale += dir * animSpeed * 0.1f; break;
                    case "LeanFactor": clip.LeanFactor += dir * animSpeed * 0.05f; break;
                }
            }
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
