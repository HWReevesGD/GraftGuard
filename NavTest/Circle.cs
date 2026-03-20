using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace NavTest
{
    internal class Circle
    {
        // Fields
        private Vector2 center;

        // Properties
        public float CenterX { get => center.X; set => center.X = value; }
        public float CenterY { get => center.Y; set => center.Y = value; }
        public Vector2 Center { get => center; }
        public float Radius { get; set; }

        // Constructor
        /// <summary>
        /// Constructs a <see cref="Circle"/> from an <paramref name="x"/> position, <paramref name="y"/> position, and <paramref name="radius"/>
        /// </summary>
        /// <param name="x">X position</param>
        /// <param name="y">Y position</param>
        /// <param name="radius">Radius</param>
        public Circle(float x, float y, float radius)
        {
            center = new Vector2(x, y);
            this.Radius = radius;
        }
    }
}
