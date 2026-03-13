using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraftGuard.UI;

internal static class Fonts
{
    public static SpriteFont Arial;
    public static void LoadContent(ContentManager content)
    {
        Arial = content.Load<SpriteFont>("Arial");
    }
}
