using Microsoft.Xna.Framework;

namespace GraftGuard.Utility;
internal static class Interface
{
    public static float Width => ScreenSize.X;
    public static float Height => ScreenSize.Y;
    public static Vector2 ScreenSize { get; private set; }
    public static Vector2 ScreenCenter { get; private set; }
    public static Vector2 TopLeft => Vector2.Zero;
    public static Vector2 BottomRight => ScreenSize;
    public static Vector2 TopRight => new Vector2(ScreenSize.X, 0);
    public static Vector2 BottomLeft => new Vector2(0, ScreenSize.Y);
    public static void Initialize(Game1 game)
    {
        ScreenSize = new Vector2(game.Graphics.PreferredBackBufferWidth, game.Graphics.PreferredBackBufferHeight);
        ScreenCenter = ScreenSize * 0.5f;
    }
}
