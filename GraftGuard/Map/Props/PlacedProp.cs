using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraftGuard.Map.Props;
internal class PlacedProp
{
    public PropDefinition Definition;
    public Vector2 Position;

    public PlacedProp(PropDefinition definition, Vector2 position)
    {
        Definition = definition;
        Position = position;
    }
}
