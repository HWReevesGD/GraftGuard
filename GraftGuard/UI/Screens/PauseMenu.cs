  // pause menu

using GraftGuard.Data;
using GraftGuard.Graphics;
using GraftGuard.Graphics.TextEffects;
using GraftGuard.Graphics.TextEffects.Effects;
using GraftGuard.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using System;

namespace GraftGuard.UI.Screens;

internal class PauseMenuOptionVisual
{
    public Text Text;
    public ShakeTextEffect Shake;
    public float YPosition;

    /// <summary>
    /// Initialize this item visual
    /// </summary>
    /// <param name="text">Text visual</param>
    /// <param name="shake">Text shake effect</param>
    /// <param name="yPosition">Y postition</param>
    public PauseMenuOptionVisual(string text, float yPosition)
    {
        Text = new Text(Fonts.SubFont, text).SetOrigin(XOrigin.Center, YOrigin.Bottom);
        Shake = new ShakeTextEffect(0);
        YPosition = yPosition;

        Text.AddEffect(Shake);
    }

    /// <summary>
    /// Bounding box of the text visual
    /// </summary>
    public Rectangle Bounds { get => new Rectangle(
        (int)(Interface.Width / 2 - Text.Width / 2),
        (int)(YPosition - Text.Height),
        Text.Width,
        Text.Height);
    }

    /// <summary>
    /// Draw the text to the screen
    /// </summary>
    /// <param name="drawing">DrawManager</param>
    /// <param name="gameTime">GameTime</param>
    public void Draw(DrawManager drawing, GameTime gameTime)
    {
        Text.Draw(drawing, gameTime, new Vector2(Interface.Width / 2, YPosition), drawLayer: 5);
    }
}

internal class PauseMenu
{
    private readonly static float screenScale = 2;
    private readonly static float padding = 150;

    private InputManager inputManager;

    private readonly static string[] options = { "Return to Game", "Quit Game" };
    private readonly float optionsGap = 50;
    private readonly float arrowsOptionMargin = 25;
    private readonly float arrowLerpSpeed = 20;
    private readonly float textShakeIntensity = 1.5f;

    private readonly float viewAreaSpinSpeed = -1f;
    private readonly float viewAreaTargetDistance = 700;
    private readonly float viewAreaOutDistance = 1150;
    private readonly float viewAreaLerpSpeed = 5;
    private readonly float viewAreaLerpTime = 0.5f;

    private int selected = 0;
    private float prevMouseSelected = -1;
    private float arrowYPosition = 0;
    private float arrowCenterOffset = 0;

    private PauseMenuOptionVisual[] optionVisuals;

    private float viewAreaDistance = 800;
    private float lastMenuInTime = 0;
    private bool allowItemSelecting = true;

    private GameTime currentGameTime;
    private SwipeTransition swipeTransition;

    public static void LoadContent(ContentManager content)
    {
        
    }

    public PauseMenu(InputManager inputManager)
    {
        this.inputManager = inputManager;
        this.optionVisuals = new PauseMenuOptionVisual[options.Length];

        for (int i = options.Length - 1; i >= 0; i--)
        {
            int reversedIdx = options.Length - i - 1;
            float yPosition = Interface.Height - padding * 2 - reversedIdx * optionsGap * screenScale;
            optionVisuals[i] = new PauseMenuOptionVisual(options[i], yPosition);
        }

        this.swipeTransition = new SwipeTransition(false);
    }

    public void Update(GameTime gameTime)
    {
        currentGameTime = gameTime;

        if (allowItemSelecting)
        {
            UpdateKeys(gameTime);
            UpdateMouseInputs(gameTime);
        }

        inputManager.Update();
    }

    /// <summary>
    /// Handle key presses
    /// </summary>
    public void UpdateKeys(GameTime gameTime)
    {
        if (inputManager.WasKeyPressStarted(Keys.Up))
            selected = (selected - 1 + options.Length) % options.Length;
        if (inputManager.WasKeyPressStarted(Keys.Down))
            selected = (selected + 1) % options.Length;

        if (inputManager.WasKeyPressStarted(Keys.Enter))
            OnOptionPicked(gameTime);
    }

    /// <summary>
    /// Update mouse inputs for the pause menu
    /// </summary>
    public void UpdateMouseInputs(GameTime gameTime)
    {
        bool clicked = inputManager.LeftMouseClicked();

        for (int i = 0; i < options.Length; i++)
        {
            PauseMenuOptionVisual visual = optionVisuals[i];
            if (visual.Bounds.Contains(inputManager.MouseScreenPosition))
            {
                if (clicked || prevMouseSelected != i)
                {
                    selected = i;
                    if (clicked)
                        OnOptionPicked(gameTime);
                }
                prevMouseSelected = i;
                return;
            }
        }
        prevMouseSelected = -1;
    }

    /// <summary>
    /// Reset the pause menu to have the first option selected
    /// </summary>
    public void Reset()
    {
        selected = 0;
    }

    /// <summary>
    /// Handle what happens when an option is selected
    /// </summary>
    public void OnOptionPicked(GameTime gameTime)
    {
        switch (selected)
        {
            case 0:
                PlayerData.CurrentState = GameState.Game;
                break;
            case 1:
                allowItemSelecting = false;
                swipeTransition.Start(gameTime, false);

                new TaskSchedule()
                    .Wait(0.5f)
                    .Run(() =>
                    {
                        MainMenu.FireInTransition(currentGameTime);
                        PlayerData.CurrentState = GameState.MainMenu;
                        swipeTransition.Clear();
                        allowItemSelecting = true;
                    });
                break;
        }
    }

    /// <summary>
    /// Draw the square border around the center of the screen
    /// </summary>
    /// <param name="drawing">DrawManager</param>
    /// <param name="gameTime">GameTime</param>
    /// <param name="enabled">Whether or not the game is pause</param>
    private void DrawBorder(DrawManager drawing, GameTime gameTime, bool enabled)
    {
        float targetViewDistance = enabled ? viewAreaTargetDistance : viewAreaOutDistance;
        viewAreaDistance = MathHelper.Lerp(viewAreaDistance, targetViewDistance, Math.Min(gameTime.Delta() * viewAreaLerpSpeed, 1));

        if (gameTime.Total() - lastMenuInTime > viewAreaLerpTime)
            return;

        float distance = viewAreaDistance * screenScale;
        int size = (int)Math.Max(Interface.Width, Interface.Height);

        for (int i = 0; i < 4; i++)
        {
            float rotation = gameTime.Total() * viewAreaSpinSpeed + (MathF.PI / 2) * i;
            float x = MathF.Cos(rotation) * distance + Interface.Width / 2;
            float y = MathF.Sin(rotation) * distance + Interface.Height / 2;

            drawing.DrawCentered(Placeholders.TexturePixel, new Rectangle((int)x, (int)y, size, size), color: Color.Black, rotation: rotation, isUi: true, drawLayer: 3);
        }
    }

    /// <summary>
    /// Draw the pause menu
    /// </summary>
    /// <param name="drawing">DrawManager</param>
    /// <param name="gameTime">GameTime</param>
    /// <param name="enabled">Whether or not the game is paused</param>
    public void Draw(DrawManager drawing, GameTime gameTime, bool enabled)
    {
        DrawBorder(drawing, gameTime, enabled);

        if (!enabled)
            return;

        lastMenuInTime = gameTime.Total();

        // draw menu items

        Rectangle fullScreenRect = new Rectangle(0, 0, (int)Interface.Width, (int)Interface.Height);
        drawing.Draw(Placeholders.TexturePixel, fullScreenRect, color: new Color(0, 0, 0, 0.75f), isUi: true, drawLayer: 4);
        

        // options

        new Text(Fonts.SubFont, "GAME PAUSED")
            .SetOrigin(XOrigin.Center, YOrigin.Top)
            .AddEffect(new WavyTextEffect(2, -3))
            .Draw(drawing, gameTime, new Vector2(Interface.Width / 2, padding * screenScale), drawLayer: 5);


        float arrowsTargetY = 0;
        float arrowsTargetOffset = 0;

        for (int i = 0; i < options.Length; i++)
        {
            PauseMenuOptionVisual visual = optionVisuals[i];
            if (i == selected)
            {
                arrowsTargetY = visual.YPosition - visual.Text.Height / 2;
                arrowsTargetOffset = visual.Text.Width / 2 + arrowsOptionMargin * 2;
                visual.Shake.Magnitude = textShakeIntensity * screenScale;
            }
            else
            {
                visual.Shake.Magnitude = 0;
            }
            visual.Draw(drawing, gameTime);
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

        swipeTransition.Draw(drawing, gameTime, 5);
    }
}