using Microsoft.Xna.Framework;

namespace GraftGuard.Utility
{
    internal static class ClassExtentions
    {
        public static Point ToPoint(this Vector2 vector) => new Point((int)vector.X, (int)vector.Y);
        public static Vector2 ToVector(this Point point) => new Vector2(point.X, point.Y);
    }
}
