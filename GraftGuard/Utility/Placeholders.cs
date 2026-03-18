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
        TextureButton = content.Load<Texture2D>("Placeholder/button_placeholder");
        TextureButtonPressed = content.Load<Texture2D>("Placeholder/button_placeholder_pressed");
        TextureButtonHover = content.Load<Texture2D>("Placeholder/button_placeholder_hover");
        TextureMissingIcon = content.Load<Texture2D>("Placeholder/missing_texture_1");
        TexturePatchLabel = content.Load<Texture2D>("Placeholder/label_placeholder");
        TexturePixel = content.Load<Texture2D>("pixel");
        TextureTerrain = content.Load<Texture2D>("Placeholder/placeholder_terrain");
        TextureGaragePatch = content.Load<Texture2D>("Placeholder/garage_patch_placeholder");
    }
    public static Texture2D TextureButton;
    public static Texture2D TextureButtonPressed;
    public static Texture2D TextureButtonHover;
    public static Texture2D TextureMissingIcon;
    public static Texture2D TexturePatchLabel;
    public static Texture2D TexturePixel;
    public static Texture2D TextureTerrain;
    public static Texture2D TextureGaragePatch;
}
