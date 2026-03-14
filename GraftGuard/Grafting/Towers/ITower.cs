using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraftGuard.Grafting.Towers;
internal interface ITower
{
    public static abstract Tower Create(Vector2 position);
}
