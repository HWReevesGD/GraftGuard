// render main menu

using GraftGuard.Data;
using GraftGuard.Graphics;
using GraftGuard.Graphics.TextEffects;
using GraftGuard.Graphics.TextEffects.Effects;
using GraftGuard.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace GraftGuard.UI.Screens;

internal delegate void ActionEvent();

/// <summary>
/// Class for menu item visual information for easy state keeping and reading
/// </summary>
internal class MainMenuOptionVisual
{
    private readonly static float itemLeftPadding = 40;

    public Text Text;
    public ShakeTextEffect Shake;
    public float XOffset;
    public float YPosition;

    /// <summary>
    /// Initialize Menu visual stuff
    /// </summary>
    /// <param name="text">Text visual</param>
    /// <param name="shake">Text shake effect</param>
    /// <param name="yPosition">Y Position</param>
    public MainMenuOptionVisual(Text text, ShakeTextEffect shake, float yPosition)
    {
        Text = text;
        Shake = shake;
        YPosition = yPosition;
    }

    /// <summary>
    /// Gets the bounding box of the text visual
    /// </summary>
    public Rectangle Bounds { get => new Rectangle(
        (int)Position.X,
        (int)(Position.Y - Text.Height),
        Text.Width,
        Text.Height);
    }

    /// <summary>
    /// Gets the position of the text visual
    /// </summary>
    public Vector2 Position { get => new Vector2(itemLeftPadding + XOffset, YPosition);  }
}

internal class MainMenu {
    public static event Action<GameTime> OnInTransition;

    /// <summary>
    /// Fires the event that tells MainMenu to play the in transition
    /// </summary>
    public static void FireInTransition(GameTime gameTime)
    {
        OnInTransition?.Invoke(gameTime);
    }

    private readonly MainMenuBackgroundWorld backgroundWorld;
    private readonly InputManager inputManager;

    private readonly static float titleLeftPadding = 20;
    private readonly static float itemLeftPadding = 40;
    private readonly static float itemBottomPadding = 20;
    private readonly static float itemGap = 10;
    private readonly static float itemShakeIntensity = 3;
    private readonly static float selectedItemLeftOffset = 34;
    private readonly static float lerpSpeed = 15;

    private readonly static float gameBeginMenuItemsLeftPosition = -1800;
    private readonly static float gameBeginMenuitemsEaseTime = 0.25f;
    private readonly static float gameBeginDelayTime = 0.5f;

    private readonly static string titleText = "GRAFTGUARD";

    private static readonly string[] menuItemOrder = [
        "Start Game",
        "Continue Game",
        "Options",
        //"View Componedium"
        ];

    public static void LoadContent(ContentManager content)
    {
        MainMenuNote.LoadContent(content);
    }

    private Game1 game;
    private int selectedItemIndex;
    private int prevMouseSelectedItemIndex = -1; // mouse is probably not over a button at startup
    private float titleYPosition;

    public MainMenuOptionVisual[] optionVisuals;

    private float arrowYPosition;

    private SwipeTransition outSwipeTransition;
    private SwipeTransition inSwipeTransition;
    private bool optionWasPicked;
    private float optionPickedTime;

    private GameTime currentGameTime;
    private MainMenuNote note;

    public event ActionEvent NewGameStarted;

    public MainMenu(Game1 game, InputManager inputManager, DrawManager drawing)
    {
        this.game = game;
        this.backgroundWorld = new MainMenuBackgroundWorld(inputManager, drawing);
        this.inputManager = inputManager;
        this.selectedItemIndex = 0;
        this.arrowYPosition = Interface.Height;

        optionVisuals = new MainMenuOptionVisual[menuItemOrder.Length];

        float yPosition = Interface.Height - itemBottomPadding;
        for (int i = menuItemOrder.Length - 1; i >= 0; i--)
        {
            Text text = new Text(Fonts.SubFont, menuItemOrder[i])
                .SetYOrigin(YOrigin.Bottom)
                .SetKerning(2);

            ShakeTextEffect shake = new ShakeTextEffect(0);
            text.AddEffect(shake);

            optionVisuals[i] = new MainMenuOptionVisual(text, shake, yPosition);
            // increment up
            yPosition += -text.Height - itemGap;
        }
        titleYPosition = yPosition;

        outSwipeTransition = new SwipeTransition(false);
        inSwipeTransition = new SwipeTransition(true);

        note = new MainMenuNote();

        OnInTransition += (GameTime gameTime) =>
        {
            inSwipeTransition.Start(gameTime, true);
        };
    }

    /// <summary>
    /// Update bmain menu
    /// </summary>
    /// <param name="gameTime">gameTime from Game1 Update()</param>
    public void Update(GameTime gameTime)
    {
        if (inputManager.WasKeyPressStarted(Keys.Escape))
            game.Exit();

        currentGameTime = gameTime;

        backgroundWorld.Update(gameTime);
        UpdateMenu(gameTime);
    }

    /// <summary>
    /// Method to be called when a menu item has been picked (pressed enter or clicked on an item)
    /// </summary>
    /// <param name="gameTime">GameTime</param>
    private void OnOptionPicked(GameTime gameTime)
    {
        switch (selectedItemIndex)
        {
            case 0: // start new game
                optionWasPicked = true;
                optionPickedTime = gameTime.Total();
                outSwipeTransition.Start(gameTime, false);

                new TaskSchedule()
                    .Wait(gameBeginDelayTime)
                    // this note transition should only happen on a new game...
                    /// but it is only new game! so it always happen
                    .Run(() => note.Start(currentGameTime.Total()))
                    .Wait(2f)
                    .Run(() => {
                        PlayerData.StartNewGame(GameManager.DawnTimeLength);
                        note.Stop();
                        NewGameStarted?.Invoke();
                        outSwipeTransition.Clear();
                        // reset main menu
                        optionWasPicked = false;
                    });
                break;
            case 1: // continue game
                break;
            case 2: // options
                break;
            case 3: // compendium
                break;
        }
    }

    /// <summary>
    /// Process key inputs for regular stuff (NOT DEBUG KEYS!)
    /// </summary>
    /// <param name="gameTime">GameTime</param>
    public void ProcessKeys(GameTime gameTime)
    {
        if (inputManager.WasKeyPressStarted(Keys.Up)) // advance back
            selectedItemIndex = (selectedItemIndex + menuItemOrder.Length - 1) % menuItemOrder.Length;
        if (inputManager.WasKeyPressStarted(Keys.Down)) // advance forward
            selectedItemIndex = (selectedItemIndex + 1) % menuItemOrder.Length;

        if (inputManager.WasKeyPressStarted(Keys.Enter))
            OnOptionPicked(gameTime);
    }

    /// <summary>
    /// Update for mouse controls on main menu items
    /// </summary>
    /// <param name="gameTime">GameTime</param>
    private void UpdateButtonMouseInputs(GameTime gameTime)
    {
        bool clicked = inputManager.LeftMouseClicked();

        for (int i = 0; i < menuItemOrder.Length; i++)
        {
            MainMenuOptionVisual visual = optionVisuals[i];
            Point mousePos = inputManager.MouseScreenPosition;

            if (visual.Bounds.Contains(inputManager.MouseScreenPosition))
            {
                // if clicked or different item hovered over
                if (clicked || prevMouseSelectedItemIndex != i)
                {
                    selectedItemIndex = i;
                    if (clicked)
                        OnOptionPicked(gameTime);
                }
                return;
            }
        }
        prevMouseSelectedItemIndex = -1;
    }

    /// <summary>
    /// Update main menu by input requests
    /// </summary>
    /// <param name="gameTime">gameTime from Game1 Update()</param>
    public void UpdateMenu(GameTime gameTime)
    {
        if (!optionWasPicked)
        {
            ProcessKeys(gameTime);
            UpdateButtonMouseInputs(gameTime);
        }
        
        UpdateMenuItemVisuals(gameTime);
        inputManager.Update();
    }

    /// <summary>
    /// Get the base left position of menu items at the current moment (changes when item is picked)
    /// </summary>
    /// <param name="gameTime">GameTime</param>
    /// <returns>Item left position</returns>
    private float GetUnpickedItemLeftPosition(GameTime gameTime)
    {
        float unpickedXOffsetPosition = 0;
        if (optionWasPicked)
        {
            float elapsed = gameTime.Total() - optionPickedTime;
            float prog = Math.Min(elapsed / gameBeginMenuitemsEaseTime, 1);
            unpickedXOffsetPosition = gameBeginMenuItemsLeftPosition * (float)Math.Pow(prog, 2);
        }
        return unpickedXOffsetPosition;
    }

    /// <summary>
    /// Updates values related to positioning of menu item elements
    /// </summary>
    /// <param name="gameTime">GameTime</param>
    public void UpdateMenuItemVisuals(GameTime gameTime)
    {
        float alpha = Math.Min(gameTime.Delta() * lerpSpeed, 1);
        float unpickedXOffsetPosition = GetUnpickedItemLeftPosition(gameTime);

        for (int i = 0; i < menuItemOrder.Length; i++)
        {
            MainMenuOptionVisual visual = optionVisuals[i];

            float targetXOffset;
            float targetWaveAmplitude;

            if (i == selectedItemIndex)
            {
                targetXOffset = selectedItemLeftOffset;
                targetWaveAmplitude = itemShakeIntensity;
                arrowYPosition = MathHelper.Lerp(arrowYPosition, visual.YPosition, alpha);
            }
            else
            {
                targetXOffset = unpickedXOffsetPosition;
                targetWaveAmplitude = 0;
            }

            visual.XOffset = MathHelper.Lerp(visual.XOffset, targetXOffset, alpha);
            visual.Shake.Magnitude = MathHelper.Lerp(visual.Shake.Magnitude, targetWaveAmplitude, alpha);
        }
    }

    /// <summary>
    /// Draw Main Menu
    /// </summary>
    /// <param name="drawing">SpriteBatch</param>
    /// <param name="gameTime">gameTime from Game1 Draw()</param>
    public void Draw(DrawManager drawing, GameTime gameTime)
    {
        // draw background world

        backgroundWorld.Draw(drawing, gameTime);

        // draw transulcent black cover

        drawing.Draw(
            Placeholders.TexturePixel,
            new Rectangle(0, 0, (int)Interface.Width, (int)Interface.Height),
            color: new Color(0, 0, 0, 0.75f),
            isUi: true
            );

        // draw menu items

        float alpha = Math.Min(gameTime.Delta() * lerpSpeed, 1);
        float unpickedXOffsetPosition = GetUnpickedItemLeftPosition(gameTime);
        bool shouldPickedItemDraw = !optionWasPicked
            || Math.Round(gameTime.TotalGameTime.TotalSeconds / 0.05f) % 2 == 0;

        for (int i = 0; i < menuItemOrder.Length; i++)
        {
            MainMenuOptionVisual visual = optionVisuals[i];
            if (shouldPickedItemDraw || i != selectedItemIndex)
                visual.Text.Draw(drawing, gameTime, visual.Position);
        }

        // little arrow thing
        if (shouldPickedItemDraw)
            new Text(Fonts.SubFont, ">")
                .SetYOrigin(YOrigin.Bottom)
                .DrawRaw(drawing, new Vector2(itemLeftPadding, arrowYPosition));

        // title text
        new Text(Fonts.MainFont, titleText)
            .SetYOrigin(YOrigin.Bottom)
            .SetKerning(3)
            .AddEffect(new WavyTextEffect(7 + unpickedXOffsetPosition, -3))
            .Draw(drawing, gameTime, new Vector2(titleLeftPadding + unpickedXOffsetPosition, titleYPosition));

        outSwipeTransition.Draw(drawing, gameTime);
        inSwipeTransition.Draw(drawing, gameTime);

        note.Draw(drawing, gameTime);
    }
}