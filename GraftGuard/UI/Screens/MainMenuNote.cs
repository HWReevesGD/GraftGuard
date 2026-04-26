using GraftGuard.Graphics;
using GraftGuard.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace GraftGuard.UI.Screens;

internal class MainMenuNote
{
    private static Texture2D note;

    private bool active;
    private float startTime;

    public static void LoadContent(ContentManager content)
    {
        note = content.Load<Texture2D>("UI/note");
    }

    public void Start(float time)
    {
        active = true;
        startTime = time;
    }

    public void Stop()
    {
        active = false;
    }

    public void Draw(DrawManager drawing, GameTime gameTime)
    {
        if (!active)
            return;

        float elapsed = gameTime.Total() - startTime;
        float opacity;

        if (elapsed < 0.5f)
            opacity = elapsed * 2; // fade in over 0.5ss
        else if (elapsed > 1.5f)
            opacity = (2f - elapsed) * 2; // fade out over 0.5s
        else
            opacity = 1;

        Color color = new Color(255, 255, 255, opacity);
        float scaleFactor = 1 + elapsed / 12;
        float rotation = MathHelper.ToRadians(elapsed * 2);

        int noteWidth = (int)(Interface.Width * .25 * scaleFactor);
        int noteHeight = (int)(Interface.Height * .4 * scaleFactor);

        drawing.DrawCentered(
            note,
            new Rectangle(
                (int)Interface.Width / 2,
                (int)Interface.Height / 2,
                noteWidth,
                noteHeight),
            color: color,
            drawLayer: 7,
            rotation: rotation,
            isUi: true
            );
    }
}
