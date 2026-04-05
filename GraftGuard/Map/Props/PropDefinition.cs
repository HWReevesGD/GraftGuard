using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraftGuard.Map.Props;
internal class PropDefinition
{
	public readonly string Name;
	public readonly Texture2D Texture;
	public readonly Rectangle Cutout;
	public readonly Vector2 Origin;
	public readonly bool UsesCollision;
	public readonly Rectangle Collision;

    public PropDefinition(string name, Texture2D texture, Rectangle cutout, Vector2 origin, bool usesCollision, Rectangle collision)
    {
        Name = name;
        Texture = texture;
        Cutout = cutout;
        Origin = origin;
        UsesCollision = usesCollision;
        Collision = usesCollision ? collision : Rectangle.Empty;
    }
}
