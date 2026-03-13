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
        TextureButtonPressed1 = content.Load<Texture2D>("Placeholder/button_placeholder_pressed");
        TextureButtonHover1 = content.Load<Texture2D>("Placeholder/button_placeholder_hover");
    }
    public static Texture2D TextureButton1;
    public static Texture2D TextureButtonPressed1;
    public static Texture2D TextureButtonHover1;

}
