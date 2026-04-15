using GraftGuard.Data;
using GraftGuard.Graphics;
using GraftGuard.Graphics.Particles;
using GraftGuard.Graphics.TextEffects;
using GraftGuard.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;

namespace GraftGuard.UI.Screens;

internal class GameHUD
{
    private static Texture2D heartTexture;
    private static Texture2D heartSpriteSheet;
    private static Texture2D timerTexture;
    private static Texture2D timerOverlayTexture;
    private static Texture2D pixelTexture;

    private readonly static Dictionary<TimeState, string> timeNames = new Dictionary<TimeState, string>
    {
        { TimeState.Dawn, "Dawn" },
        { TimeState.Day, "Day" },
        { TimeState.Night, "Night" }
    };
    private readonly static Dictionary<TimeState, string> timeObjectives = new Dictionary<TimeState, string>
    {
        { TimeState.Dawn, "Collect Parts" },
        { TimeState.Day, "Created your Defences" },
        { TimeState.Night, "Protect your Garage" }
    };


    private readonly static float hudScale = 2.0f; 

    private readonly static Vector2 heartSize = new Vector2(50, 50);
    private readonly static float heartGap = 10;
    private readonly static Vector2 heartMargin = new Vector2(40, 40);

    // timer progress bar
    private readonly static float timerTopMargin = 10;
    private readonly static float timerWidth = 345;
    private readonly static float timerHeight = 115;
    private readonly static float timerBarHeightScale = 0.4f; // scale of timerHeight
    private readonly static float timerBarMargin = 8;

    // ticking
    private readonly static float timerTextTickYOffset = -5;
    private readonly static float timerTickRecoverSpeed = 5;
    private float timerTextYOffset = 0;
    private float previousSeconds = 0;

    // whole hud positioning
    private readonly static float hudInactiveTopPosition = -120;
    private readonly static float hudMoveTime = 0.5f;
    private float hudActiveChangeTime = 0;
    private bool hudPrevInactive = false;

    private readonly int numHearts = 3; // full health number split across this amount of visual hearts
    private readonly int numHeartSegments = 8; // spritesheet

    private ParticleManager particles;
    private int previousHealth;
    private int previousHearts;

    public static void LoadContent(ContentManager content)
    {
        heartTexture = content.Load<Texture2D>("UI/Heart");
        heartSpriteSheet = content.Load<Texture2D>("UI/Heart_Spritesheet");
        timerTexture = content.Load<Texture2D>("UI/Timer");
        timerOverlayTexture = content.Load<Texture2D>("UI/Timer_Overlay");
        pixelTexture = content.Load<Texture2D>("pixel");
    }

    public GameHUD()
    {
        particles = new ParticleManager();
        previousHealth = PlayerData.CurrentGame.Health;
        previousHearts = (int)Math.Ceiling((float)PlayerData.CurrentGame.Health / PlayerData.CurrentGame.MaxHealth * numHearts);
    }

    public void DrawHealth(DrawManager drawing, GameTime gameTime, bool active, float hudTopOffset)
    {
        int currentHearts = (int)Math.Ceiling((float)PlayerData.CurrentGame.Health / PlayerData.CurrentGame.MaxHealth * numHearts);

        float pulseCycle = (float)gameTime.TotalGameTime.TotalSeconds % 1;
        float pulseScale = Math.Max(((0.5f - pulseCycle) / 0.5f), 0) * 0.25f + 1;

        for (int i = 0; i < Math.Max(previousHearts, currentHearts); i++)
        {
            Vector2 baseSize = heartSize * hudScale;
            Vector2 scaledMargin = heartMargin * hudScale;
            float scaledGap = heartGap * hudScale;

            float wave = (float)Math.Sin(-gameTime.TotalGameTime.TotalSeconds * 2 + (float)i / 2) * 5;

            // hearts that have been lost since  the last frame
            if (i >= currentHearts)
            {
                Vector2 particleOrigin = scaledMargin + new Vector2(
                    (baseSize.X + scaledGap) * i - baseSize.X / 2,
                    wave - baseSize.Y / 2 + hudTopOffset
                );

                // particles for the hearts that the player lost
                for (int j = 0; j < 10; j++)
                {
                    // big particle
                    particles.Add(
                        new Particle(Placeholders.TexturePixel)
                            .SetLifetime(0.5f, 1f)
                            .SetColor(Color.Red)
                            .SetSize(Vector2.One * 100, Vector2.Zero)
                            .SetSpeed(200f, 400f)
                            .SetAngle(0, (float)Math.PI * 2)
                            .SetPosition(particleOrigin)
                        );

                    // small particle
                    particles.Add(
                        new Particle(Placeholders.TexturePixel)
                            .SetLifetime(0.3f, 2f)
                            .SetColor(Color.Red)
                            .SetSize(Vector2.One * 35, Vector2.Zero)
                            .SetSpeed(400f, 800f)
                            .SetAngle(0, (float)Math.PI * 2)
                            .SetPosition(particleOrigin)
                        );
                }

                continue;
            }

            // draw other hearts

            Texture2D textureToDraw = heartSpriteSheet;
            int spritesheetIndex = 0;


            // render current heart in fractions
            if (i == currentHearts - 1)
            {
                // figure out what spritesheet index is needed
                // this took a bit of thinking so its thoroughly annotated

                float percentSegmentSize = 1f / numHearts;
                // overall health percentage
                float healthPercent = (float)PlayerData.CurrentGame.Health / PlayerData.CurrentGame.MaxHealth;
                // health percentage relevant to this heart
                float percentMod = healthPercent % percentSegmentSize;
                // as percentage of the percentage of one heart
                float heartPercent = percentMod / percentSegmentSize;
                // force round down (so that spritesheetIndex will not equal numHeartSegments)
                spritesheetIndex = (int)Math.Round(heartPercent * numHeartSegments - 0.5f);
            }
            else
            {
                // full heart
                spritesheetIndex = numHeartSegments - 1;
            }

            int xSegmentSize = heartSpriteSheet.Bounds.Width / numHeartSegments;

            Rectangle sourceRect = new Rectangle(spritesheetIndex * xSegmentSize, (spritesheetIndex + 1) * xSegmentSize, xSegmentSize, heartSpriteSheet.Bounds.Height);
            Vector2 finalSize = i == currentHearts - 1 ? baseSize * pulseScale : baseSize;
            Vector2 position = scaledMargin + new Vector2(
                (baseSize.X + scaledGap) * i - finalSize.X / 2,
                wave - finalSize.Y / 2 + hudTopOffset
            );

            Rectangle rect = new Rectangle(
                (int)position.X,
                (int)position.Y,
                (int)finalSize.X,
                (int)finalSize.Y
                );

            drawing.Draw(textureToDraw, destination: rect, source: sourceRect, isUi: true, sortMode: SortMode.Top);
            //drawing.DrawString($"HEALTH: {PlayerData.CurrentGame.Health}", new Vector2(64, 64), isUi: true);
        }

        previousHealth = PlayerData.CurrentGame.Health;
        previousHearts = currentHearts;
    }

    public void DrawTimer(DrawManager drawing, GameTime gameTime, bool active, float hudTopOffset)
    {
        int totalSeconds = (int)Math.Ceiling(PlayerData.CurrentGame.Timer);
        int displaySeconds = totalSeconds % 60;
        int displayMinutes = totalSeconds / 60;

        if (previousSeconds != totalSeconds)
        {
            // tick the timer
            previousSeconds = totalSeconds;
            timerTextYOffset = timerTextTickYOffset;
        }
        else
        {
            // ease back down
            timerTextYOffset = MathHelper.Lerp(timerTextYOffset, 0, Math.Min(gameTime.Delta() * timerTickRecoverSpeed, 1));
        }

        float sWidth = timerWidth * hudScale;
        float sHeight = timerHeight * hudScale;
        float sTopMargin = timerTopMargin * hudScale;

        drawing.Draw(timerTexture, new Rectangle(
            (int)(Interface.Width / 2 - sWidth / 2),
            (int)(sTopMargin + hudTopOffset),
            (int)sWidth,
            (int)sHeight
            ),
            isUi: true);

        string timerText = active ? $"{displayMinutes}:{displaySeconds.ToString("D2")}" : "-:--";

        float baseY = 5 * hudScale;
        float textY1 = (50 * hudScale) + baseY + (timerTextYOffset * hudScale) + hudTopOffset;
        float textY2 = (70 * hudScale) + baseY + (timerTextYOffset * hudScale) + hudTopOffset;

        new Text(Fonts.SubFont, $"{timerText} Left")
            .SetXOrigin(XOrigin.Center)
            .Draw(drawing, gameTime, new Vector2(Interface.Width / 2, textY1 + baseY + timerTextYOffset + hudTopOffset),
            isUi: true);

        new Text(Fonts.SubFont, timeNames[PlayerData.CurrentGame.Time])
           .SetXOrigin(XOrigin.Center)
           .Draw(drawing, gameTime, new Vector2(Interface.Width / 2, textY2 + baseY + timerTextYOffset + hudTopOffset),
            isUi: true);

        // draw objective
        if (PlayerData.CurrentGame.Time != TimeState.Day)
        {
            float offset = 4.0f;
            if (PlayerData.CurrentGame.Time == TimeState.Dawn)
            {
                offset -= 44f;
            }

            new Text(Fonts.SubFont, timeObjectives[PlayerData.CurrentGame.Time])
           .SetXOrigin(XOrigin.Center)
           .Draw(drawing, gameTime, new Vector2(Interface.Width / 2 - 390f * hudScale, hudTopOffset + (64f + offset) * hudScale),
            isUi: true);
        }

        // timer progress bar

        float timerProgressBarScale = PlayerData.CurrentGame.Timer / PlayerData.CurrentGame.PhaseTimeLength;
        float sBarMargin = timerBarMargin * hudScale;

        // Scale the internal progress bar rectangle
        drawing.Draw(pixelTexture, new Rectangle(
            (int)(Interface.Width / 2 - sWidth / 2 + sBarMargin),
            (int)(sTopMargin + sBarMargin + hudTopOffset),
            (int)((sWidth - sBarMargin * 2) * timerProgressBarScale),
            (int)(sHeight * timerBarHeightScale - sBarMargin * 2)
            ), color: Color.Purple,
            isUi: true);

        drawing.Draw(timerOverlayTexture, new Rectangle(
            (int)(Interface.Width / 2 - sWidth / 2),
            (int)(sTopMargin + hudTopOffset),
            (int)sWidth,
            (int)sHeight
            ), color: Color.White,
            isUi: true);
    }

    public void Draw(DrawManager drawing, GameTime gameTime, bool active)
    {

        // whole hud offset

        if (hudPrevInactive != active)
        {
            hudPrevInactive = active;
            hudActiveChangeTime = (float)gameTime.TotalGameTime.TotalSeconds;
        }

        float hudTopOffset;
        float baseAlpha = Math.Min(((float)gameTime.TotalGameTime.TotalSeconds - hudActiveChangeTime) / hudMoveTime, 1);

        float scaledInactiveTop = hudInactiveTopPosition * hudScale;

        if (active)
        {
            float easeOutAlpha = 1 - (float)Math.Pow(1 - baseAlpha, 2);
            hudTopOffset = MathHelper.Lerp(scaledInactiveTop, 0, easeOutAlpha);
        } else
        {
            float easeInAlpha = (float)Math.Pow(baseAlpha, 2);
            hudTopOffset = MathHelper.Lerp(0, scaledInactiveTop, easeInAlpha);
        }

        if (PlayerData.CurrentGame.Time == TimeState.Night)
        {
            DrawHealth(drawing, gameTime, active, hudTopOffset);
        }

        DrawTimer(drawing, gameTime, active, hudTopOffset);

        particles.Update(gameTime);
        particles.Draw(drawing, gameTime, isUi: true);
    }
}
