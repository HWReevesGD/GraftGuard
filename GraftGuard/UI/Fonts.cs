using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GraftGuard.UI;

internal static class Fonts
{
    public static SpriteFont Arial;
    public static SpriteFont MainFont;
    public static SpriteFont SubFont;
    public static void LoadContent(ContentManager content)
    {
        Arial = content.Load<SpriteFont>("Arial");
        MainFont = content.Load<SpriteFont>("UI/Fonts/TitleFont");
        SubFont = content.Load<SpriteFont>("UI/Fonts/SubFont");
    }
}
