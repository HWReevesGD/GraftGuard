using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraftGuard.Utility;

internal static class Placeholders
{
    public static void LoadContent(ContentManager content)
    {
        TextureButton1 = content.Load<Texture2D>("Placeholder/button_placeholder");
    }
    public static Texture2D TextureButton1;
}
