using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraftGuard;
internal class Camera
{
    public Vector2 Position { get; set; } = Vector2.Zero;
    public Matrix ScreenToWorld => Matrix.CreateTranslation(new Vector3(Position, 0));
    public Matrix WorldToScreen => Matrix.CreateTranslation(new Vector3(-Position, 0));
}
