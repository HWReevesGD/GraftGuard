// render main menu

using GraftGuard.Map;
using GraftGuard.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GraftGuard.UI;

internal class MainMenu {
    private World backgroundWorld;
    private InputManager inputManager; // input manager that will never update, because it's for the background (world needs it)
    private TimeState timeState;

    private static Texture2D backgroundTexture;

    public static void LoadContent(ContentManager content) {
        backgroundTexture = content.Load<Texture2D>("pixel");
    }

    public MainMenu() {
        backgroundWorld = new World();
        inputManager = new InputManager();
        timeState = TimeState.Night; // always night
    }

    /// <summary>
    /// Update background world simulation and pan the camera
    /// </summary>
    /// <param name="gameTime">gameTime from Game1 Update()</param>
    public void Update(GameTime gameTime) {
        backgroundWorld.Update(gameTime, inputManager, timeState);

        // TODO: animate this
        Vector2 position = new Vector2(5f, 5f);

        backgroundWorld.Camera.Position = position - Interface.ScreenCenter;
    }

    /// <summary>
    /// Draw Main Menu
    /// </summary>
    /// <param name="batch">SpriteBatch</param>
    /// <param name="gameTime">gameTime from Game1 Draw()</param>
    public void Draw(SpriteBatch batch, GameTime gameTime) {
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

        Rectangle fullScreenRect = new Rectangle(0, 0, (int)Interface.Width, (int)Interface.Height);
        Color bgColor = new Color(0, 0, 0, 128f);

        batch.Draw(backgroundTexture, fullScreenRect, bgColor);

        batch.DrawString(Fonts.Arial, "MAIN MENU!!! PRESS ENTER TO START", Vector2.Zero, Color.White);
    }
}