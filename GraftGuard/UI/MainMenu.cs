// render main menu

using GraftGuard.Data;
using GraftGuard.Graphics.TextEffects;
using GraftGuard.Map;
using GraftGuard.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace GraftGuard.UI;

internal class MainMenu {
    private readonly World backgroundWorld;
    private readonly InputManager idleInputManager; // input manager that will never update, because it's for the background (world needs it)
    private readonly InputManager inputManager;
    private readonly TimeState timeState;

    private readonly static float cameraPanSpeed = 0.5f;

    private readonly static float titleLeftPadding = 20;
    private readonly static float itemLeftPadding = 40;
    private readonly static float itemBottomPadding = 20;
    private readonly static float itemGap = 10;
    private readonly static float itemShakeIntensity = 1;
    private readonly static float lerpSpeed = 15;

    private static Texture2D backgroundTexture;
    private readonly static string titleText = "GRAFTGUARD";

    private static readonly string[] menuItemOrder = [
        "Start Game",
        "Continue Game",
        "Options",
        "View Componedium"
        ];

    public static void LoadContent(ContentManager content)
    {
        backgroundTexture = content.Load<Texture2D>("pixel");
    }

    private Game1 game;
    private int selectedItemIndex;
    private float[] itemXOffests;
    private float[] itemWaveAmplitudes;
    private float arrowYPosition;

    public MainMenu(Game1 game, InputManager inputManager)
    {
        this.game = game;
        this.backgroundWorld = new World();
        this.idleInputManager = new InputManager();
        this.inputManager = inputManager;
        this.timeState = TimeState.Night; // always night
        this.selectedItemIndex = 0;
        this.itemXOffests = new float[menuItemOrder.Length];
        this.itemWaveAmplitudes = new float[menuItemOrder.Length];
        this.arrowYPosition = Interface.Height;
        CreateTowers();
    }

    private void CreateTowers()
    {
        // create random towers for the world
    }

    /// <summary>
    /// Update bmain menu
    /// </summary>
    /// <param name="gameTime">gameTime from Game1 Update()</param>
    public void Update(GameTime gameTime)
    {
        if (inputManager.WasKeyPressStarted(Keys.Escape))
            game.Exit();

        UpdateBackgroundWorld(gameTime);
        UpdateMenu(gameTime);
    }

    /// <summary>
    /// Update background world simulation and pan the camera
    /// </summary>
    /// <param name="gameTime">gameTime from Game1 Update()</param>
    public void UpdateBackgroundWorld(GameTime gameTime)
    {
        backgroundWorld.Update(gameTime, idleInputManager, timeState, false);

        double x = 200 + Math.Cos(gameTime.TotalGameTime.TotalSeconds / 3 * cameraPanSpeed) * 400 + 400;
        double y = 200 + Math.Sin(gameTime.TotalGameTime.TotalSeconds * cameraPanSpeed) * 300 + 300;
        Vector2 position = new Vector2((float)x, (float)y);

        backgroundWorld.Camera.Position = position - Interface.ScreenCenter;
    }

    /// <summary>
    /// Update main menu by input requests
    /// </summary>
    /// <param name="gameTime">gameTime from Game1 Update()</param>
    public void UpdateMenu(GameTime gameTime)
    {
        if (inputManager.WasKeyPressStarted(Keys.Up)) // advance back
            selectedItemIndex = (selectedItemIndex + menuItemOrder.Length - 1) % menuItemOrder.Length;
        if (inputManager.WasKeyPressStarted(Keys.Down)) // advance forward
            selectedItemIndex = (selectedItemIndex + 1) % menuItemOrder.Length;

        if (inputManager.WasKeyPressStarted(Keys.Enter))
        {
            switch (selectedItemIndex)
            {
                case 0: // start new game
                    PlayerData.StartNewGame(GameManager.DawnTimeLength);
                    break;
                case 1: // continue game
                    break;
                case 2: // options
                    break;
                case 3: // compendium
                    break;
            }
        }

        inputManager.Update();
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
        backgroundWorld.DrawCamera(batch, gameTime, timeState, inputManager, false);
        batch.End();

        // gui stuff

        batch.Begin();

        // draw transulcent black cover

        Rectangle fullScreenRect = new Rectangle(0, 0, (int)Interface.Width, (int)Interface.Height);
        Color bgColor = new Color(0, 0, 0, 0.75f);
        batch.Draw(backgroundTexture, fullScreenRect, bgColor);

        // draw menu items

        float alpha = Math.Min(gameTime.Delta() * lerpSpeed, 1);
        float yPosition = Interface.Height - itemBottomPadding;

        // draw each menu item

        for (int i = menuItemOrder.Length - 1; i >= 0; i--)
        {
            // update effects for this item

            float targetXOffset;
            float targetWaveAmplitude;

            if (i == selectedItemIndex)
            {
                targetXOffset = 15;
                targetWaveAmplitude = itemShakeIntensity;
                arrowYPosition = MathHelper.Lerp(arrowYPosition, yPosition, alpha);
            }
            else
            {
                targetXOffset = 0;
                targetWaveAmplitude = 0;
            }

            itemXOffests[i] = MathHelper.Lerp(itemXOffests[i], targetXOffset, alpha);
            itemWaveAmplitudes[i] = MathHelper.Lerp(itemWaveAmplitudes[i], targetWaveAmplitude, alpha);

            // render item on the bottom left yea

            Text text = new Text(Fonts.Arial, menuItemOrder[i]).SetYOrigin(YOrigin.Bottom).SetKerning(2);
            new TextEffects(new Text(Fonts.Arial, menuItemOrder[i]).SetYOrigin(YOrigin.Bottom).SetKerning(2))
                .AddEffect(new ShakeTextEffect(itemWaveAmplitudes[i]))
                .Draw(batch, gameTime, new Vector2(itemLeftPadding + itemXOffests[i], yPosition));

            // increment up
            yPosition += -text.Height - itemGap;
        }

        // little arrow thing
        new Text(Fonts.Arial, ">")
            .SetYOrigin(YOrigin.Bottom)
            .Draw(batch, new Vector2(itemLeftPadding, arrowYPosition));

        // title text
        new Text(Fonts.Arial, titleText)
            .SetYOrigin(YOrigin.Bottom)
            .SetScale(3)
            .SetKerning(3)
            .Draw(batch, new Vector2(titleLeftPadding, yPosition));

        
    }
}