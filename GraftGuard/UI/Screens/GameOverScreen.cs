using GraftGuard.Data;
using GraftGuard.Graphics;
using GraftGuard.Graphics.Particles;
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

/// <summary>
/// Game over screen visuals
/// </summary>
internal class GameOverScreen
{
    private World world;
    private GameData session;
    private InputManager inputManager;
    private float startShowingTime;
    private string failReason = "None";

    private static Texture2D backgroundTexture;

    private static readonly float screenScale = 2.0f;

    private static readonly string titleText = "GAME OVER";
    private static readonly string scoreText = "SCORE: ";
    private static readonly string hiScoreText = "HIGH SCORE: ";
    private static readonly int countRate = 3;
    private static readonly float countUpMaxTime = 3f;
    private static readonly int gap = 50; // gap between score text and score number

    private static readonly float titleShakeMagnitude = 10;
    private static readonly float titleShakeDecayTime = 0.5f;
    private static readonly float reasonShakeDecayTime = 0.35f;
    private static readonly float returnTextFlashPeriod = 0.25f;

    private bool isScoreShowing = false;
    private bool isHiScoreShowing = false;
    private int displayScore = 0;
    private int displayHiScore = 0;
    private bool isReturnShowing = false;

    private TaskSchedule currentTasks;
    private Text reasonText;
    private ShakeTextEffect reasonTextShake;
    private ParticleManager particles;
    private bool allowExiting = true;

    private SwipeTransition swipeTransition;
    private GameTime currentGameTime;

    public static void LoadContent(ContentManager content)
    {
        backgroundTexture = content.Load<Texture2D>("pixel");
    }

    /// <summary>
    /// Initialize game over screen to the given world
    /// </summary>
    /// <param name="world">World</param>
    public GameOverScreen(World world)
    {
        this.world = world;
        inputManager = new InputManager(); // idle InputManager

        reasonTextShake = new ShakeTextEffect(titleShakeMagnitude);
        reasonText = new Text(Fonts.SubFont, failReason)
            .SetXOrigin(XOrigin.Center)
            .AddEffect(reasonTextShake);

        particles = new ParticleManager();
        swipeTransition = new SwipeTransition(false);
    }

    /// <summary>
    /// Get the position to draw game over reason at
    /// </summary>
    /// <returns>Game over reason text postiion</returns>
    private Vector2 GetReasonTextPosition()
    {
        float centerX = Interface.Width / 2;
        float centerY = Interface.Height / 2;
        return new Vector2(centerX, centerY - (110 * screenScale));
    }

    /// <summary>
    /// Start Game over visuals flow
    /// </summary>
    /// <param name="gameTime">GameTime</param>
    /// <param name="session">Session of the current game</param>
    /// <param name="failReason">Reason why game over happened</param>
    public void SetGameOver(GameTime gameTime, GameData session, string failReason)
    {
        this.session = session;
        startShowingTime = (float)gameTime.TotalGameTime.TotalSeconds;
        this.failReason = failReason;

        reasonText.TextString = "";

        isScoreShowing = false;
        isHiScoreShowing = false;
        displayScore = 0;
        displayHiScore = 0;

        isReturnShowing = false;

        // do the kind of typing effect
        currentTasks = new TaskSchedule();
        currentTasks.Wait(0.5f);

        string[] words = failReason.Split(" ");
        for (int i = 0; i < words.Length; i++)
        {
            int wordIndex = i;
            currentTasks
                .Run(() =>
                {
                    string stringAt = "";
                    for (int j = 0; j < wordIndex + 1; j++)
                    {
                        stringAt += " " + words[j];
                    }
                    reasonText.TextString = stringAt.TrimStart();
                    reasonTextShake.Magnitude = titleShakeMagnitude;

                    Text wordText = new Text(Fonts.SubFont, words[wordIndex]);

                    Random rng = new Random();
                    Vector2 reasonTextPos = GetReasonTextPosition();

                    for (int _ = 0; _ < 10; _++)
                    {
                        particles.Add(
                            new Particle(Placeholders.TexturePixel)
                                .SetLifetime(0.25f, 0.5f)
                                .SetSize(new Vector2(30, 30), Vector2.Zero)
                                .SetAngle(-(float)Math.PI / 4, -(float)Math.PI * 3 / 4)
                                .SetSpeed(75, 160)
                                .SetPosition(new Vector2(
                                    float.Lerp(reasonTextPos.X + reasonText.Width / 2 - wordText.Width, reasonTextPos.X + reasonText.Width / 2, (float)rng.NextDouble()),
                                    float.Lerp(reasonTextPos.Y, reasonTextPos.Y + reasonText.Height, (float)rng.NextDouble())
                                    ))
                                .SetAcceleration(new Vector2(0, 1500))
                            );
                    }

                })
                .Loop(reasonShakeDecayTime, (_, elapsed) =>
                {
                    reasonTextShake.Magnitude = titleShakeMagnitude * Math.Max(1 - elapsed / reasonShakeDecayTime, 0);
                });
        }

        int score = session.CurrentScore;
        int hiScore = PlayerData.HighScore; // PlayerData.HighScore

        float scoreCountTime = Math.Min(score / (float)countRate, countUpMaxTime);
        float hiScoreCountTime = Math.Min(hiScore / (float)countRate, countUpMaxTime);

        currentTasks.Run(() => isScoreShowing = true);

        if (score != 0)
        {
            currentTasks
                .Loop(scoreCountTime, (_, elapsed) =>
                {
                    displayScore = (int)Math.Min(Math.Round(score * elapsed / scoreCountTime), score);
                })
                .Run(() => displayScore = score).Wait(1f);
        }

        currentTasks
            .Wait(1f)
            .Run(() => isHiScoreShowing = true);

        if (hiScore != 0)
        {
            currentTasks
                .Loop(scoreCountTime, (_, elapsed) =>
                {
                    displayScore = (int)Math.Min(Math.Round(score * elapsed / scoreCountTime), score);
                })
                .Run(() => displayHiScore = hiScore);
        }

        currentTasks
            .Wait(1f)
            .Run(() => isReturnShowing = true);
    }

    /// <summary>
    /// Exit to the main menu with a transition
    /// </summary>
    /// <param name="gameTime">GameTime</param>
    public void Exit(GameTime gameTime)
    {
        allowExiting = false;
        swipeTransition.Start(gameTime, false);

        new TaskSchedule()
            .Wait(0.5f)
            .Run(() =>
            {
                MainMenu.FireInTransition(currentGameTime);
                PlayerData.CurrentState = GameState.MainMenu;
                swipeTransition.Clear();
                particles.Clear();
                currentTasks.Cancel();
                allowExiting = true;
            });
    }

    /// <summary>
    /// Update the GameOverScreen visuals
    /// </summary>
    /// <param name="gameTime">GameTime</param>
    public void Update(GameTime gameTime)
    {
        currentGameTime = gameTime;

        if (inputManager.WasKeyPressStarted(Keys.Escape) || inputManager.WasKeyPressStarted(Keys.Enter))
            if (allowExiting)
                Exit(gameTime);

        inputManager.Update();
        particles.Update(gameTime);
    }

    /// <summary>
    /// Draw GameOverScreen visuals
    /// </summary>
    /// <param name="drawing">DrawManager</param>
    /// <param name="gameTime">GameTime</param>
    public void Draw(DrawManager drawing, GameTime gameTime)
    {
        // Draw by the Camera's Position
        world.DrawCamera(drawing, gameTime, session.Time, inputManager, true);

        // draw menu items

        Rectangle fullScreenRect = new Rectangle(0, 0, (int)Interface.Width, (int)Interface.Height);
        Color bgColor = new Color(0, 0, 0, 0.75f);
        drawing.Draw(backgroundTexture, destination: fullScreenRect, color: bgColor, isUi: true);

        float screenScale = 2.0f;
        float centerX = Interface.Width / 2;
        float centerY = Interface.Height / 2;

        // title

        float elapsed = (float)gameTime.TotalGameTime.TotalSeconds - startShowingTime;
        float shakeMagnitude = titleShakeMagnitude * Math.Max(1 - elapsed / titleShakeDecayTime, 0);

        new Text(Fonts.SubFont, titleText).SetXOrigin(XOrigin.Center)
            .AddEffect(new ShakeTextEffect(shakeMagnitude))
            .Draw(drawing, gameTime, new Vector2(centerX, centerY - (150 * screenScale)));

        // reason
        reasonText.Draw(drawing, gameTime, GetReasonTextPosition());

        // score stuff

        Vector2 scoreTextSize = Fonts.SubFont.MeasureString(scoreText);
        Vector2 hiScoreTextSize = Fonts.SubFont.MeasureString(hiScoreText);

        float scoreY = centerY - (25 * screenScale);
        float scaledGap = gap * screenScale;
        //float scaledJump = scoreNumJumpDist * screenScale;

        if (isScoreShowing)
        {
            drawing.DrawString(
                font: Fonts.SubFont,
                text: scoreText,
                position: new Vector2(centerX - scoreTextSize.X - scaledGap / 2, scoreY),
                isUi: true,
                drawLayer: 2);

            drawing.DrawString(
                font: Fonts.SubFont,
                text: $"{displayScore}",
                position: new Vector2(
                    centerX + scaledGap / 2,
                    scoreY //+ (scoreCountIsUp ? -scaledJump : 0)
                    ),
                isUi: true,
                drawLayer: 2);
        }

        // hi score text

        if (isHiScoreShowing)
        {
            float hiScoreY = centerY + (25 * screenScale);

            drawing.DrawString(
                font: Fonts.SubFont,
                text: hiScoreText,
                position: new Vector2(
                    centerX - hiScoreTextSize.X - scaledGap / 2,
                    hiScoreY),
                isUi: true,
                drawLayer: 2);

            drawing.DrawString(
                font: Fonts.SubFont,
                text: $"{displayHiScore}",
                position: new Vector2(
                    centerX + scaledGap / 2,
                    hiScoreY //+ (hiScoreCountIsUp ? -scaledJump : 0)
                    ),
                isUi: true,
                drawLayer: 2);
        }

        //

        if (isReturnShowing && MathF.Floor((float)gameTime.Total() / returnTextFlashPeriod) % 2 == 0)
        {
            new Text(Fonts.SubFont, "< Press Enter or Esc to return to Menu >")
                .SetXOrigin(XOrigin.Center)
                .SetYOrigin(YOrigin.Bottom)
                .AddEffect(new WavyTextEffect(2 * screenScale, -5))
                .Draw(
                    drawing,
                    gameTime,
                    new Vector2(centerX, centerY + 150 * screenScale),
                    isUi: true
                    );
        }

        particles.Draw(drawing, gameTime, isUi: true, drawLayer: 4);
        swipeTransition.Draw(drawing, gameTime, 5);
    }
}
