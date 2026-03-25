using GraftGuard.Data;
using GraftGuard.Graphics.TextEffects;
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

    public void SetSession(GameTime gameTime, GameData session)
    {
        this.session = session;
        startShowingTime = (float)gameTime.TotalGameTime.TotalSeconds;
    }

    public void Update(GameTime gameTime)
    {
        if (inputManager.WasKeyPressStarted(Keys.Escape) || inputManager.WasKeyPressStarted(Keys.Enter))
            PlayerData.CurrentState = GameState.MainMenu;

        inputManager.Update();
    }

    public void Draw(SpriteBatch batch, GameTime gameTime)
    {
        batch.End();
        // Draw by the Camera's Position
        batch.Begin(samplerState: SamplerState.PointWrap, transformMatrix: world.Camera.WorldToScreen);
        world.DrawCamera(batch, gameTime, session.Time, inputManager, true);
        batch.End();

        // draw menu items

        batch.Begin();

        Rectangle fullScreenRect = new Rectangle(0, 0, (int)Interface.Width, (int)Interface.Height);
        Color bgColor = new Color(0, 0, 0, 0.75f);
        batch.Draw(backgroundTexture, fullScreenRect, bgColor);

        // title

        float elapsed = (float)gameTime.TotalGameTime.TotalSeconds - startShowingTime;
        float shakeMagnitude = titleShakeMagnitude * Math.Max(1 - elapsed / titleShakeDecayTime, 0);

        new TextEffects(new Text(Fonts.Arial, titleText).SetXOrigin(XOrigin.Center))
            .AddEffect(new ShakeTextEffect(shakeMagnitude))
            .Draw(batch, gameTime, new Vector2(Interface.Width / 2, 100));

        // score stuff

        int score = 5; // _session.CurrentScore
        int hiScore = 12334434; // PlayerData.HighScore

        // stats

        float scoreCountTime = Math.Min(score / (float)countRate, countUpMaxTime);
        float hiScoreCountTime = Math.Min(hiScore / (float)countRate, countUpMaxTime);

        Vector2 scoreTextSize = Fonts.Arial.MeasureString(scoreText);
        Vector2 hiScoreTextSize = Fonts.Arial.MeasureString(hiScoreText);

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

        batch.DrawString(Fonts.Arial, scoreText, new Vector2(
            Interface.Width / 2 - scoreTextSize.X - gap / 2,
            200
            ), Color.White);

        batch.DrawString(
            Fonts.Arial,
            $"{displayScore}",
            new Vector2(
                Interface.Width / 2 + gap / 2,
                200 + (scoreCountIsUp ? -scoreNumJumpDist : 0)
                ),
            Color.White
            );

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
            batch.DrawString(Fonts.Arial, hiScoreText, new Vector2(
                Interface.Width / 2 - hiScoreTextSize.X - gap / 2,
                300
                ), Color.White);

            batch.DrawString(
                Fonts.Arial,
                $"{displayHiScore}",
                new Vector2(
                    Interface.Width / 2 + gap / 2,
                    300 + (hiScoreCountIsUp ? -scoreNumJumpDist : 0)
                    ),
               Color.White
               );
        }
    }
}
