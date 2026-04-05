using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraftGuard.Map.Tiles;
internal class TileDefinition
{
    public Texture2D Texture;
    public Rectangle Cutout;
    public bool IsSolid;

    public TileDefinition(Texture2D texture, Rectangle cutout, bool isSolid)
    {
        Texture = texture;
        Cutout = cutout;
        IsSolid = isSolid;
    }
}
