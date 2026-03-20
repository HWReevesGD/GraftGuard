// pause menu

using GraftGuard.Map;
using GraftGuard.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace GraftGuard.UI;

internal class PauseMenu {
    private World world;

    private static string text = "< Press Enter to Return to Play >\n< Press Esc to Exit >";

    private static Texture2D backgroundTexture;

    public static void LoadContent(ContentManager content) {
        backgroundTexture = content.Load<Texture2D>("pixel");
    }

    public PauseMenu(World world) {
        this.world = world;
    }

    public void Draw(SpriteBatch batch, GameTime gameTime, TimeState timeState) {

        batch.End();
        // Draw by the Camera's Position
        batch.Begin(samplerState: SamplerState.PointWrap, transformMatrix: world.Camera.WorldToScreen);
        world.DrawCamera(batch, gameTime, timeState);
        batch.End();

        // draw menu items

        batch.Begin();

        Rectangle fullScreenRect = new Rectangle(0, 0, (int)Interface.Width, (int)Interface.Height);
        Color bgColor = new Color(0, 0, 0, 0.75f);
        batch.Draw(backgroundTexture, fullScreenRect, bgColor);

        Vector2 textSize = Fonts.Arial.MeasureString(text);
        Vector2 position = new Vector2(Interface.Width / 2 - textSize.X / 2, Interface.Height / 2 - textSize.Y / 2);

        batch.DrawString(Fonts.Arial, text, position, Color.White);
    }
}