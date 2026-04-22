// pause menu

using GraftGuard.Data;
using GraftGuard.Graphics;
using GraftGuard.Graphics.TextEffects;
using GraftGuard.Graphics.TextEffects.Effects;
using GraftGuard.Map;
using GraftGuard.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace GraftGuard.UI.Screens;
internal class PauseMenu
{
    private readonly static float screenScale = 2;
    private readonly static float padding = 150;

    private static Texture2D backgroundTexture;
    private InputManager inputManager;

    private readonly static string[] options = new string[] { "Return to Game", "Quit Game" };
    private readonly float optionsGap = 50;
    private readonly float arrowsOptionMargin = 25;
    private readonly float arrowLerpSpeed = 20;
    private readonly float textShakeIntensity = 1.5f;
    private readonly float viewAreaSpinSpeed = -1f;

    private int selected = 0;
    private float arrowYPosition = 0;
    private float arrowCenterOffset = 0;


    public static void LoadContent(ContentManager content)
    {
        backgroundTexture = content.Load<Texture2D>("pixel");
    }

    public PauseMenu(InputManager inputManager)
    {
        this.inputManager = inputManager;
    }

    public void Update()
    {

        if (inputManager.WasKeyPressStarted(Keys.Up))
            selected = (selected - 1 + options.Length) % options.Length;
        if (inputManager.WasKeyPressStarted(Keys.Down))
            selected = (selected + 1) % options.Length;

        if (inputManager.WasKeyPressStarted(Keys.Enter))
        {
            switch (selected)
            {
                case 0:
                    PlayerData.CurrentState = GameState.Game;
                    break;
                case 1:
                    PlayerData.CurrentState = GameState.MainMenu;
                    break;
            }
        }

        inputManager.Update();
    }

    private void DrawBorder(DrawManager drawing, GameTime gameTime)
    {
        float distance = 700 * screenScale;
        int size = (int)Math.Max(Interface.Width, Interface.Height);
        for (int i = 0; i < 4; i++)
        {
            float rotation = gameTime.Total() * viewAreaSpinSpeed + (MathF.PI / 2) * i;
            float x = MathF.Cos(rotation) * distance + Interface.Width / 2;
            float y = MathF.Sin(rotation) * distance + Interface.Height / 2;

            drawing.DrawCentered(Placeholders.TexturePixel, new Rectangle((int)x, (int)y, size, size), color: Color.Black, rotation: rotation, isUi: true, drawLayer: 3);
        }
    }

    public void Draw(DrawManager drawing, GameTime gameTime, bool enabled)
    {
        // draw menu items

        Rectangle fullScreenRect = new Rectangle(0, 0, (int)Interface.Width, (int)Interface.Height);
        drawing.Draw(Placeholders.TexturePixel, fullScreenRect, color: new Color(0, 0, 0, 0.75f), isUi: true, drawLayer: 4);

        DrawBorder(drawing, gameTime);

        // options

        new Text(Fonts.SubFont, "GAME PAUSED")
            .SetOrigin(XOrigin.Center, YOrigin.Top)
            .AddEffect(new WavyTextEffect(2, -3))
            .Draw(drawing, gameTime, new Vector2(Interface.Width / 2, padding * screenScale), drawLayer: 5);


        float arrowsTargetY = 0;
        float arrowsTargetOffset = 0;

        for (int i = options.Length - 1; i >= 0; i--)
        {
            int reversedIdx = options.Length - i - 1;
            float yPosition = Interface.Height - padding * 2 - reversedIdx * optionsGap * screenScale;

            Text text = new Text(Fonts.SubFont, options[i])
               .SetOrigin(XOrigin.Center, YOrigin.Bottom);

            if (i == selected)
            {
                text.AddEffect(new ShakeTextEffect(textShakeIntensity * screenScale));
                arrowsTargetY = yPosition - text.Height / 2;
                arrowsTargetOffset = text.Width / 2 + arrowsOptionMargin * 2;
            }

            text.Draw(drawing, gameTime, new Vector2(Interface.Width / 2, yPosition), drawLayer: 5);
        }

        float alpha = Math.Min(gameTime.Delta() * arrowLerpSpeed, 1);
        arrowYPosition = MathHelper.Lerp(arrowYPosition, arrowsTargetY, alpha);
        arrowCenterOffset = MathHelper.Lerp(arrowCenterOffset, arrowsTargetOffset, alpha);

        new Text(Fonts.SubFont, ">")
            .SetOrigin(XOrigin.Right, YOrigin.Center)
            .Draw(drawing, gameTime, new Vector2(Interface.Width / 2 - arrowCenterOffset, arrowYPosition), drawLayer: 5);

        new Text(Fonts.SubFont, "<")
            .SetOrigin(XOrigin.Left, YOrigin.Center)
            .Draw(drawing, gameTime, new Vector2(Interface.Width / 2 + arrowCenterOffset, arrowYPosition), drawLayer: 5);

    }
}