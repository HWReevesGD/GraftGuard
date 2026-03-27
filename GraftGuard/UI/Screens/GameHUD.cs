using GraftGuard.Data;
using GraftGuard.Graphics.TextEffects;
using GraftGuard.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace GraftGuard.UI.Screens;

internal class GameHUD
{
    private static Texture2D heartTexture;
    private static Texture2D timerTexture;
    private static Texture2D timerOverlayTexture;
    private static Texture2D pixelTexture;

    private readonly static Dictionary<TimeState, string> timeNames = new Dictionary<TimeState, string>
    {
        { TimeState.Dawn, "Dawn" },
        { TimeState.Day, "Day" },
        { TimeState.Night, "Night" }
    };

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

    public static void LoadContent(ContentManager content)
    {
        heartTexture = content.Load<Texture2D>("UI/Heart");
        timerTexture = content.Load<Texture2D>("UI/Timer");
        timerOverlayTexture = content.Load<Texture2D>("UI/Timer_Overlay");
        pixelTexture = content.Load<Texture2D>("pixel");
    }

    public GameHUD()
    {

    }

    public void Draw(SpriteBatch batch, GameTime gameTime, bool active)
    {
        int totalSeconds = (int)Math.Ceiling(PlayerData.CurrentGame.Timer);
        int displaySeconds = totalSeconds % 60;
        int displayMinutes = totalSeconds / 60;

        if (previousSeconds != totalSeconds)
        {
            previousSeconds = totalSeconds;
            timerTextYOffset = timerTextTickYOffset;
        } else
        {
            timerTextYOffset = MathHelper.Lerp(timerTextYOffset, 0, Math.Min(gameTime.Delta() * timerTickRecoverSpeed, 1));
        }

        // whole hud offset

        if (hudPrevInactive != active)
        {
            hudPrevInactive = active;
            hudActiveChangeTime = (float)gameTime.TotalGameTime.TotalSeconds;
        }

        float hudTopOffset;
        float baseAlpha = Math.Min(((float)gameTime.TotalGameTime.TotalSeconds - hudActiveChangeTime) / hudMoveTime, 1);

        if (active)
        {
            float easeOutAlpha = 1 - (float)Math.Pow(1 - baseAlpha, 2);
            hudTopOffset = MathHelper.Lerp(hudInactiveTopPosition, 0, easeOutAlpha);
        } else
        {
            float easeInAlpha = (float)Math.Pow(baseAlpha, 2);
            hudTopOffset = MathHelper.Lerp(0, hudInactiveTopPosition, easeInAlpha);
        }

        #region Health

        float pulseCycle = (float)gameTime.TotalGameTime.TotalSeconds % 1;
        float pulseScale = Math.Max(((0.5f - pulseCycle) / 0.5f), 0) * 0.25f + 1;

        for (int i = 0; i < 3; i++)
        {
            float wave = (float)Math.Sin(-gameTime.TotalGameTime.TotalSeconds * 2 + (float)i / 2) * 5;
            Vector2 size = i == 2 ? heartSize * pulseScale : heartSize;
            Vector2 position = heartMargin + new Vector2((heartSize.X + heartGap) * i - size.X / 2, wave - size.Y / 2 + hudTopOffset);
            
            Rectangle rect = new Rectangle(
                (int)(position.X),
                (int)(position.Y),
                (int)size.X,
                (int)size.Y
                );
            //batch.Draw(heartTexture, rect, null, Color.White, 0, size / 2, SpriteEffects.None, 0);
            batch.Draw(heartTexture, rect, Color.White);
        }
        #endregion


        #region Timer HUD

        batch.Draw(timerTexture, new Rectangle(
            (int)(Interface.Width / 2 - timerWidth / 2),
            (int)(timerTopMargin + hudTopOffset),
            (int)timerWidth,
            (int)timerHeight
            ), Color.White);

        string timerText = active ? $"{displayMinutes}:{displaySeconds.ToString("D2")}" : "-:--";

        int baseY = 5; 

        new Text(Fonts.SubFont, $"{timerText} Left")
            .SetXOrigin(XOrigin.Center)
            .Draw(batch, gameTime, new Vector2(Interface.Width / 2, 55 + baseY + timerTextYOffset + hudTopOffset));

        new Text(Fonts.SubFont, timeNames[PlayerData.CurrentGame.Time])
           .SetXOrigin(XOrigin.Center)
           .Draw(batch, gameTime, new Vector2(Interface.Width / 2, 75 + baseY + timerTextYOffset + hudTopOffset));

        // timer progress bar

        float timeLength;
        if (PlayerData.CurrentGame.Time == TimeState.Dawn)
            timeLength = GameManager.DawnTimeLength;
        else if (PlayerData.CurrentGame.Time == TimeState.Night)
            timeLength = GameManager.NightTimeLength;
        else
            timeLength = 1;

        float timerProgressBarScale = PlayerData.CurrentGame.Timer / timeLength;

        // green bar
        batch.Draw(pixelTexture, new Rectangle(
            (int)(Interface.Width / 2 - timerWidth / 2 + timerBarMargin),
            (int)(timerTopMargin + timerBarMargin + hudTopOffset),
            (int)((timerWidth - timerBarMargin * 2) * timerProgressBarScale),
            (int)(timerHeight * timerBarHeightScale - timerBarMargin * 2)
            ), Color.Purple);

        batch.Draw(timerOverlayTexture, new Rectangle(
            (int)(Interface.Width / 2 - timerWidth / 2),
            (int)(timerTopMargin + hudTopOffset),
            (int)timerWidth,
            (int)timerHeight
            ), Color.White);
        #endregion
    }
}
