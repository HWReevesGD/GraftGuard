using GraftGuard.Map.Props;
using GraftGuard.Map.Tiles;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraftGuard.Map;
internal class MapDefinition
{
    public ReadOnlyCollection<Vector2> EnemySpawns { get; init; }
    public ReadOnlyCollection<PlacedProp> PlacedProps { get; init; }
    public FrozenDictionary<Point, ReadOnlyCollection<TileDefinition>> TileChunks { get; init; }
}
