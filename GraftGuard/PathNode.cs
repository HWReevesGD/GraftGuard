using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ShapeUtils;

namespace GraftGuard
{
    internal class PathNode
    {
        // Fields
        private Vector2 position;
        private Circle debugCircle;

        // Properties
        public Vector2 Position { get => position ; }

        // Constructor
        /// <summary>
        /// PathNode objects are used as navigation points for enemies
        /// </summary>
        /// <param name="position">the node's position</param>
        public PathNode(Vector2 position)
        {
            this.position = position;
            debugCircle = new Circle(X, Y, 5);
        }

        // Methods
        /// <summary>
        /// Draws the circle
        /// this should only be called if game is in debug mode
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch)
        {
            ShapeBatch.Circle(debugCircle.Center, debugCircle.Radius, Color.Red);
        }
    }
}
