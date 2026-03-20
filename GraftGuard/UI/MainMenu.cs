// render main menu

using GraftGuard.Map;
using GraftGuard.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace GraftGuard.UI;

internal class MainMenu {
    private readonly World backgroundWorld;
    private readonly InputManager idleInputManager; // input manager that will never update, because it's for the background (world needs it)
    private readonly InputManager inputManager;
    private readonly TimeState timeState;

    private readonly static int titleTextTopPosition = 150;
    private readonly static int startTextBottomPosition = 150;
    private readonly static float cameraPanSpeed = 0.5f;
    private readonly static float textWaveSpeed = 3f;
    private readonly static float kerning = 5;

    private static Texture2D backgroundTexture;
    private readonly static string titleText = "GRAFTGUARD";
    private readonly static string startText = "< Press Enter to Begin >";

    public static void LoadContent(ContentManager content)
    {
        backgroundTexture = content.Load<Texture2D>("pixel");
    }

    public MainMenu(InputManager inputManager)
    {
        this.backgroundWorld = new World();
        this.idleInputManager = new InputManager();
        this.inputManager = inputManager;
        this.timeState = TimeState.Night; // always night
        CreateTowers();
    }

    private void CreateTowers()
    {
        // create random towers for the world
    }

    /// <summary>
    /// Update background world simulation and pan the camera
    /// </summary>
    /// <param name="gameTime">gameTime from Game1 Update()</param>
    public void Update(GameTime gameTime)
    {
        backgroundWorld.Update(gameTime, idleInputManager, timeState);

        // TODO: animate this
        double x = 200 + Math.Cos(gameTime.TotalGameTime.TotalSeconds / 3 * cameraPanSpeed) * 400 + 400;
        double y = 200 + Math.Sin(gameTime.TotalGameTime.TotalSeconds * cameraPanSpeed) * 300 + 300;
        Vector2 position = new Vector2((float)x, (float)y);

        backgroundWorld.Camera.Position = position - Interface.ScreenCenter;
    }

    /// <summary>
    /// Draw Main Menu
    /// </summary>
    /// <param name="batch">SpriteBatch</param>
    /// <param name="gameTime">gameTime from Game1 Draw()</param>
    public void Draw(SpriteBatch batch, GameTime gameTime)
    {
        // simulate and render world in the background
        // with random towers and constantly spawning enemies
        // if the world succumbs to an enemy, don't

        // draw background world

        batch.End();
        // Draw by the Camera's Position
        batch.Begin(samplerState: SamplerState.PointWrap, transformMatrix: backgroundWorld.Camera.WorldToScreen);
        backgroundWorld.DrawCamera(batch, gameTime, timeState);
        batch.End();

        // draw menu items

        batch.Begin();

        Rectangle fullScreenRect = new Rectangle(0, 0, (int)Interface.Width, (int)Interface.Height);
        Color bgColor = new Color(0, 0, 0, 0.75f);
        batch.Draw(backgroundTexture, fullScreenRect, bgColor);

        Vector2 textSize = Fonts.Arial.MeasureString(titleText) + new Vector2((titleText.Length - 1) * kerning, 0);
        Vector2 leftPosition = new Vector2(Interface.Width / 2 - textSize.X / 2, titleTextTopPosition);

        float leftSize = 0;

        for (int i = 0; i < titleText.Length; i++)
        {
            string character = titleText.Substring(i, 1);
            float yOffset = (float)Math.Sin(gameTime.TotalGameTime.TotalSeconds * textWaveSpeed + i / 2) * 6;

            Vector2 charSize = Fonts.Arial.MeasureString(character);
            Vector2 position = leftPosition + new Vector2(leftSize + charSize.X + kerning, yOffset);
            batch.DrawString(Fonts.Arial, character, position, Color.White);

            leftSize += charSize.X + kerning;
        }

        Vector2 startTextSize = Fonts.Arial.MeasureString(startText);
        Vector2 startTextPosition = new Vector2(
            Interface.Width / 2 - startTextSize.X / 2,
            Interface.Height - startTextSize.Y - startTextBottomPosition
        );

        batch.DrawString(Fonts.Arial, startText, startTextPosition, Color.White);
    }
}