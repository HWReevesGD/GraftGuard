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

internal class GameOverScreen
{
    private World world;
    private GameData session;
    private InputManager inputManager;
    private float startShowingTime;
    private string failReason = "None";

    private static Texture2D backgroundTexture;

    private static readonly string titleText = "GAME OVER";
    private static readonly string scoreText = "SCORE: ";
    private static readonly string hiScoreText = "HIGH SCORE: ";
    private static readonly int countRate = 3;
    private static readonly float countUpMaxTime = 3f;
    private static readonly int gap = 50; // gap between score text and score number
    private static readonly int scoreNumJumpDist = 5;

    private static readonly float titleShakeMagnitude = 10;
    private static readonly float titleShakeDecayTime = 0.5f;

    private bool scoreCountIsUp = false;
    private bool hiScoreCountIsUp = false;
    private int prevDisplayScore = 0;
    private int prevDisplayHiScore = 0;

    public static void LoadContent(ContentManager content)
    {
        backgroundTexture = content.Load<Texture2D>("pixel");
    }

    public GameOverScreen(World world)
    {
        this.world = world;
        inputManager = new InputManager(); // idle InputManager
    }

    public void SetSession(GameTime gameTime, GameData session, string failReason)
    {
        this.session = session;
        startShowingTime = (float)gameTime.TotalGameTime.TotalSeconds;
        this.failReason = failReason;
    }

    public void Update(GameTime gameTime)
    {
        if (inputManager.WasKeyPressStarted(Keys.Escape) || inputManager.WasKeyPressStarted(Keys.Enter))
            PlayerData.CurrentState = GameState.MainMenu;

        inputManager.Update();
    }

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
        new Text(Fonts.SubFont, failReason).SetXOrigin(XOrigin.Center)
            .AddEffect(new ShakeTextEffect(shakeMagnitude))
            .Draw(drawing, gameTime, new Vector2(centerX, centerY - (110 * screenScale)));

        // score stuff

        int score = 5; // _session.CurrentScore
        int hiScore = 12334434; // PlayerData.HighScore

        // stats

        float scoreCountTime = Math.Min(score / (float)countRate, countUpMaxTime);
        float hiScoreCountTime = Math.Min(hiScore / (float)countRate, countUpMaxTime);

        Vector2 scoreTextSize = Fonts.SubFont.MeasureString(scoreText);
        Vector2 hiScoreTextSize = Fonts.SubFont.MeasureString(hiScoreText);

        // score text

        float scoreAlpha = ((float)gameTime.TotalGameTime.TotalSeconds - startShowingTime) / scoreCountTime;
        int displayScore = (int)(score * (float)Math.Min(scoreAlpha, 1));

        if (prevDisplayScore != displayScore)
        {
            scoreCountIsUp = !scoreCountIsUp;
            prevDisplayScore = displayScore;
        } else
        {
            scoreCountIsUp = false;
        }

        float scoreY = centerY - (25 * screenScale);
        float scaledGap = gap * screenScale;
        float scaledJump = scoreNumJumpDist * screenScale;

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
                scoreY + (scoreCountIsUp ? -scaledJump : 0)
                ),
            isUi: true,
            drawLayer: 2);

        // hi score text

        float hiScoreAlpha = ((float)gameTime.TotalGameTime.TotalSeconds - startShowingTime - scoreCountTime) / hiScoreCountTime;
        int displayHiScore = (int)(hiScore * (float)Math.Clamp(hiScoreAlpha, 0, 1));

        if (prevDisplayHiScore != displayHiScore)
        {
            hiScoreCountIsUp = !hiScoreCountIsUp;
            prevDisplayHiScore = displayHiScore;
        }
        else
        {
            hiScoreCountIsUp = false;
        }

        if (scoreAlpha >= 1)
        {
            float hiScoreY = centerY + (75 * screenScale);

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
                    hiScoreY + (hiScoreCountIsUp ? -scaledJump : 0)
                    ),
                isUi: true,
                drawLayer: 2);
        }
    }
}
