using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GraftGuard
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
        /// This Class exists so I can visualize PathNodes while debugging
        /// - Philip
        /// </summary>
        /// <param name="x">X position</param>
        /// <param name="y">Y position</param>
        /// <param name="radius">radious</param>
        public Circle(float x, float y, float radius)
        {
            center = new Vector2(x, y);
            this.Radius = radius;
        }
    }
}
