using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraftGuard.UI;
internal static class Interface
{
    public static Vector2 ScreenSize { get; private set; }
    public static Vector2 ScreenCenter { get; private set; }
    public static void Initialize(Game1 game)
    {
        ScreenSize = new Vector2(game.Graphics.PreferredBackBufferWidth, game.Graphics.PreferredBackBufferHeight);
        ScreenCenter = ScreenSize * 0.5f;
    }
}
