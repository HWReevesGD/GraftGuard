// pause menu

using GraftGuard.Data;
using GraftGuard.Graphics;
using GraftGuard.Map;
using GraftGuard.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GraftGuard.UI.Screens;
internal class PauseMenu
{
    private readonly World world;

    private readonly static string text = "< Press Enter to Return to Play >\n< Press Esc to Exit >";

    private static Texture2D backgroundTexture;
    private InputManager inputManager;

    public static void LoadContent(ContentManager content)
    {
        backgroundTexture = content.Load<Texture2D>("pixel");
    }

    public PauseMenu(World world, InputManager inputManager)
    {
        this.world = world;
        this.inputManager = inputManager;
    }

    public void Update()
    {
        if (inputManager.WasKeyPressStarted(Keys.Enter))
            PlayerData.CurrentState = GameState.Game;

        if (inputManager.WasKeyPressStarted(Keys.Escape))
            PlayerData.CurrentState = GameState.MainMenu;

        inputManager.Update();
    }

    public void Draw(DrawManager drawing, GameTime gameTime, TimeState timeState)
    {

        // Draw by the Camera's Position
        world.DrawCamera(drawing, gameTime, timeState, inputManager, true);

        // draw menu items

        Rectangle fullScreenRect = new Rectangle(0, 0, (int)Interface.Width, (int)Interface.Height);
        Color bgColor = new Color(0, 0, 0, 0.75f);
        drawing.Draw(backgroundTexture, destination: fullScreenRect, color: bgColor, isUi: true);

        Vector2 textSize = Fonts.SubFont.MeasureString(text);
        Vector2 position = new Vector2(Interface.Width / 2 - textSize.X / 2, Interface.Height / 2 - textSize.Y / 2);

        drawing.DrawString(font: Fonts.SubFont, text: text, position: position, isUi: true, sortMode: SortMode.Top, drawLayer: 2);
    }
}