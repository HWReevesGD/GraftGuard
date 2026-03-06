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
        public float X {  get => position.X ; }
        public float Y { get => position.Y ; }

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
        /// Ensures the circle stays on the node even if the node is moved for some reason
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            if (position != debugCircle.Center)
                debugCircle.Center = position;
        }

        /// <summary>
        /// Draws the circle
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch)
        {
            ShapeBatch.Circle(debugCircle.Center, debugCircle.Radius, Color.Red);
        }
    }
}
