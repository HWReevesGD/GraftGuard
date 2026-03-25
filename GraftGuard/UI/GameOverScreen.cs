using GraftGuard.Data;
using GraftGuard.Map;
using GraftGuard.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace GraftGuard.UI;

internal class GameOverScreen
{
    private World _world;
    private GameData _session;
    private InputManager _inputManager;
    private float _startShowingTime;

    private static Texture2D backgroundTexture;

    private static readonly string titleText = "GAME OVER";
    private static readonly string scoreText = "SCORE: ";
    private static readonly string hiScoreText = "HIGH SCORE: ";
    private static readonly int countRate = 3;
    private static readonly float countUpMaxTime = 3f;
    private static readonly int gap = 50; // gap between score text and score number
    private static readonly int scoreNumJumpDist = 5;

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
        _world = world;
        _inputManager = new InputManager(); // idle InputManager
    }

    public void SetSession(GameTime gameTime, GameData session)
    {
        _session = session;
        _startShowingTime = (float)gameTime.TotalGameTime.TotalSeconds;
    }

    public void Update(GameTime gameTime)
    {

    }

    public void Draw(SpriteBatch batch, GameTime gameTime)
    {
        batch.End();
        // Draw by the Camera's Position
        batch.Begin(samplerState: SamplerState.PointWrap, transformMatrix: _world.Camera.WorldToScreen);
        _world.DrawCamera(batch, gameTime, _session.Time, _inputManager, true);
        batch.End();

        // draw menu items

        batch.Begin();

        Rectangle fullScreenRect = new Rectangle(0, 0, (int)Interface.Width, (int)Interface.Height);
        Color bgColor = new Color(0, 0, 0, 0.75f);
        batch.Draw(backgroundTexture, fullScreenRect, bgColor);

        // title

        Vector2 titleSize = Fonts.Arial.MeasureString(titleText);
        Vector2 position = new Vector2(
            Interface.Width / 2 - titleSize.X / 2,
            100
            );

        batch.DrawString(Fonts.Arial, titleText, position, Color.White);

        int score = 500; // _session.CurrentScore
        int hiScore = 12334434; // PlayerData.HighScore

        // stats

        float scoreCountTime = Math.Min(score / (float)countRate, countUpMaxTime);
        float hiScoreCountTime = Math.Min(hiScore / (float)countRate, countUpMaxTime);

        Vector2 scoreTextSize = Fonts.Arial.MeasureString(scoreText);
        Vector2 hiScoreTextSize = Fonts.Arial.MeasureString(hiScoreText);

        // score text

        float scoreAlpha = ((float)gameTime.TotalGameTime.TotalSeconds - _startShowingTime) / scoreCountTime;
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

        float hiScoreAlpha = ((float)gameTime.TotalGameTime.TotalSeconds - _startShowingTime - scoreCountTime) / hiScoreCountTime;
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
