using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraftGuard.Grafting
{
    internal class WorldPart : GameObject
    {
        public PartDefinition Definition { get; private set; }
        public WorldPart(Vector2 position, Vector2 hitboxSize, Texture2D texture) : base(position, hitboxSize, texture)
        {
        }
    }
}
