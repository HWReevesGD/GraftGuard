using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraftGuard.Grafting
{
    internal class Tower : GameObject
    {
        private Part[] _attachedParts;
        /// <summary>
        /// Constructs a Tower with Empty Parts
        /// </summary>
        /// <param name="position">Tower's Initial Position</param>
        /// <param name="size">Tower's Drawing Size</param>
        /// <param name="texture">Tower's Texture</param>
        public Tower(Vector2 position, Vector2 size, Texture2D texture) : base(position, size, texture)
        {
            _attachedParts = new Part[4];
        }
    }
}
