using GraftGuard.Graphics;
using GraftGuard.Utility;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraftGuard.UI.Screens;

internal class SwipeTransition
{
    private static float scale = 2f;
    private static int fadeWidth = 1000;
    private static float swipeTime = 0.5f;
    private static int gap = 50;

    // square is rotated 45deg and it should be big enough to touch the corner of the grid square it is in
    // so it should be as big as its diagonal
    private static float squareTargetSize = (float)Math.Sqrt(2 * Math.Pow(gap, 2)) * 2;

    private bool isRunning;
    private float startTime;
    private bool isReversed;

    public SwipeTransition(bool isReversed)
    {
        this.isReversed = isReversed;
    }

    public void Start(GameTime gameTime)
    {
        isRunning = true;
        startTime = (float)gameTime.TotalGameTime.TotalSeconds;
    }

    public void DrawSquares(DrawManager drawing, float elapsed)
    {
        int numHorizontal = (int)Math.Ceiling(Interface.Width / (gap * scale));
        int numVertical = (int)Math.Ceiling(Interface.Height / (gap * scale));

        float leftSwipePosition = Interface.Width - (Interface.Width + fadeWidth * scale) * (elapsed / swipeTime);
        float rightSwipePosition = leftSwipePosition + fadeWidth * scale;

        if (isReversed) // reversed
        {
            float temp = leftSwipePosition;
            leftSwipePosition = rightSwipePosition;
            rightSwipePosition = temp;
        }

        for (int x = 0; x < numHorizontal; x++)
        {
            for (int y = 0; y < numVertical; y++)
            {
                Vector2 screenPosition = new Vector2(
                    x * gap * scale,
                    y * gap * scale - squareTargetSize * scale / 2
                    );

                float alpha = (screenPosition.X - leftSwipePosition) / (rightSwipePosition - leftSwipePosition);
                if (alpha <= 0)
                    continue;

                float clampedAlpha = Math.Clamp(alpha, 0, 1);
                float squareScale = clampedAlpha * squareTargetSize * scale;
                float posOffset = squareTargetSize * scale / 2 * (1 - clampedAlpha);

                drawing.Draw(
                    texture: Placeholders.TexturePixel,
                    destination: new Rectangle((int)(screenPosition.X + posOffset), (int)(screenPosition.Y + posOffset), (int)squareScale, (int)squareScale),
                    rotation: (float)Math.PI / 4,
                    drawLayer: 2,
                    color: Color.Black,
                    isUi: true
                    );
            }
        }
    }

    public void Draw(DrawManager drawing, GameTime gameTime)
    {
        if (!isRunning)
            return;

        float elapsed = (float)gameTime.TotalGameTime.TotalSeconds - startTime;

        if (elapsed > swipeTime)
        {
            isRunning = false;
            return;
        }

        DrawSquares(drawing, elapsed);
    }
}
