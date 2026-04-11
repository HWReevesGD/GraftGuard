using GraftGuard.Graphics;
using GraftGuard.Map.Props;
using GraftGuard.Map.Tiles;
using GraftGuard.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Schema;

namespace GraftGuard.Map;
internal class Terrain
{
    public Dictionary<Point, TileDefinition[]> Chunks;
    public List<PlacedProp> Props;
    private Player player;

    public Terrain(Player player)
    {
        this.player = player;
    }

    public void LoadMap(MapDefinition map)
    {
        Chunks = map.TileChunks.ToDictionary((pair) => pair.Key, (pair) => pair.Value.ToArray());
        Props = map.PlacedProps.ToList();
    }

    public void Update(GameTime time)
    {

    }

    public void Draw(SpriteBatch batch, GameTime time)
    {
        if (Chunks is null)
        {
            return;
        }

        foreach ((Point coordinate, IEnumerable<TileDefinition> chunk) in Chunks)
        {
            Vector2 chunkPosition = coordinate.ShiftLeft(EnvironmentRegistry.ChunkBits + EnvironmentRegistry.TileBits).ToVector();
            int index = 0;
            foreach (TileDefinition tile in chunk)
            {
                if (tile is null)
                {
                    index++;
                    continue;
                }

                float x = ((index & EnvironmentRegistry.ChunkMask) << EnvironmentRegistry.TileBits) + chunkPosition.X;
                float y = ((index >> EnvironmentRegistry.ChunkBits) << EnvironmentRegistry.TileBits) + chunkPosition.Y;

                batch.Draw(tile.Texture, new Vector2(x, y), tile.Cutout, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, -0f);
                index++;
            }
        }

        if (Props is not null)
        {
            foreach (PlacedProp prop in Props)
            {
                prop.Draw(batch, player);
            }
        }

        foreach (Rectangle box in GetTileBoxes())
        {
            batch.Draw(Placeholders.TexturePixel, box, Color.Black);
        }
    }

    public bool Overlaps(Circle circle)
    {
        if (Chunks is null || Chunks.Count == 0)
        {
            if (Props is null)
            {
                return false;
            }

            return Props.Any((prop) => prop.Definition.UsesCollision && prop.Collision.Intersects(circle));
        }

        IEnumerable<(Point chunkCoordinate, TileDefinition[])> overlappingChunks = Chunks.Keys.Where((coordinate) =>
        {
            Point chunkPosition = coordinate.ShiftLeft(EnvironmentRegistry.ChunkBits + EnvironmentRegistry.TileBits);
            return circle.Intersects(new Rectangle(chunkPosition, new Point(EnvironmentRegistry.ChunkSize << EnvironmentRegistry.TileBits)));
        }).Select((chunkCoordinate) => (chunkCoordinate, Chunks[chunkCoordinate]));

        int tileSize = EnvironmentRegistry.TileSize;
        foreach ((Point coordinate, TileDefinition[] chunk) in overlappingChunks)
        {
            Point chunkPosition = coordinate.ShiftLeft(EnvironmentRegistry.ChunkBits + EnvironmentRegistry.TileBits);
            int index = 0;
            foreach (TileDefinition definition in chunk)
            {
                int x = index & EnvironmentRegistry.ChunkMask;
                int y = index >> EnvironmentRegistry.ChunkBits;

                if (definition is null || !definition.IsSolid)
                {
                    index++;
                    continue;
                }

                Rectangle tile = new Rectangle(
                    tileSize * x, tileSize * y,
                    tileSize, tileSize);
                tile.Offset(chunkPosition);

                if (tile.Intersects(circle))
                {
                    return true;
                }

                index++;
            }
        }

        return Props is not null && Props.Any((prop) => prop.Definition.UsesCollision && prop.Collision.Intersects(circle));
    }
    public bool Overlaps(Rectangle rectangle)
    {
        if (Chunks is null || Chunks.Count == 0)
        {
            if (Props is null)
            {
                return false;
            }

            return Props.Any((prop) => prop.Definition.UsesCollision && prop.Collision.Intersects(rectangle));
        }

        IEnumerable<(Point chunkCoordinate, TileDefinition[])> overlappingChunks = Chunks.Keys.Where((coordinate) =>
        {
            Point chunkPosition = coordinate.ShiftLeft(EnvironmentRegistry.ChunkBits + EnvironmentRegistry.TileBits);
            return rectangle.Intersects(new Rectangle(chunkPosition, new Point(EnvironmentRegistry.ChunkSize << EnvironmentRegistry.TileBits)));
        }).Select((chunkCoordinate) => (chunkCoordinate, Chunks[chunkCoordinate]));

        int tileSize = EnvironmentRegistry.TileSize;
        foreach ((Point coordinate, TileDefinition[] chunk) in overlappingChunks)
        {
            Point chunkPosition = coordinate.ShiftLeft(EnvironmentRegistry.ChunkBits + EnvironmentRegistry.TileBits);
            int index = 0;
            foreach (TileDefinition definition in chunk)
            {
                int x = index & EnvironmentRegistry.ChunkMask;
                int y = index >> EnvironmentRegistry.ChunkBits;

                if (definition is null || !definition.IsSolid)
                {
                    index++;
                    continue;
                }

                Rectangle tile = new Rectangle(
                    tileSize * x, tileSize * y,
                    tileSize, tileSize);
                tile.Offset(chunkPosition);

                if (tile.Intersects(rectangle))
                {
                    return true;
                }

                index++;
            }
        }

        return Props is not null && Props.Any((prop) => prop.Definition.UsesCollision && prop.Collision.Intersects(rectangle));
    }
    public List<Rectangle> GetOverlappingBoxes(Rectangle rectangle)
    {
        if (Chunks is null || Chunks.Count == 0)
        {
            if (Props is null)
            {
                return [];
            }

            return Props.Where((prop) => prop.Definition.UsesCollision && prop.Collision.Intersects(rectangle)).Select((prop) => prop.Collision).ToList();
        }

        IEnumerable<(Point chunkCoordinate, TileDefinition[])> overlappingChunks = Chunks.Keys.Where((coordinate) =>
        {
            Point chunkPosition = coordinate.ShiftLeft(EnvironmentRegistry.ChunkBits + EnvironmentRegistry.TileBits);
            return rectangle.Intersects(new Rectangle(chunkPosition, new Point(EnvironmentRegistry.ChunkSize << EnvironmentRegistry.TileBits)));
        }).Select((chunkCoordinate) => (chunkCoordinate, Chunks[chunkCoordinate]));

        int tileSize = EnvironmentRegistry.TileSize;
        List<Rectangle> overlapping = [];
        foreach ((Point coordinate, TileDefinition[] chunk) in overlappingChunks)
        {
            Point chunkPosition = coordinate.ShiftLeft(EnvironmentRegistry.ChunkBits + EnvironmentRegistry.TileBits);
            int index = 0;
            foreach (TileDefinition definition in chunk)
            {
                int x = index & EnvironmentRegistry.ChunkMask;
                int y = index >> EnvironmentRegistry.ChunkBits;

                if (definition is null || !definition.IsSolid)
                {
                    index++;
                    continue;
                }

                Rectangle tile = new Rectangle(
                    tileSize * x, tileSize * y,
                    tileSize, tileSize);
                tile.Offset(chunkPosition);

                if (tile.Intersects(rectangle))
                {
                    overlapping.Add(tile);
                }

                index++;
            }
        }

        if (Props is not null)
        {
            overlapping.AddRange(Props.Where((prop) => prop.Definition.UsesCollision && prop.Collision.Intersects(rectangle)).Select((prop) => prop.Collision));
        }

        return overlapping;
    }
    public List<Rectangle> GetOverlappingBoxes(Circle circle)
    {
        if (Chunks is null || Chunks.Count == 0)
        {
            if (Props is null)
            {
                return [];
            }

            return Props.Where((prop) => prop.Definition.UsesCollision && prop.Collision.Intersects(circle)).Select((prop) => prop.Collision).ToList();
        }

        IEnumerable<(Point chunkCoordinate, TileDefinition[])> overlappingChunks = Chunks.Keys.Where((coordinate) =>
        {
            Point chunkPosition = coordinate.ShiftLeft(EnvironmentRegistry.ChunkBits + EnvironmentRegistry.TileBits);
            return circle.Intersects(new Rectangle(chunkPosition, new Point(EnvironmentRegistry.ChunkSize << EnvironmentRegistry.TileBits)));
        }).Select((chunkCoordinate) => (chunkCoordinate, Chunks[chunkCoordinate]));

        int tileSize = EnvironmentRegistry.TileSize;
        List<Rectangle> overlapping = [];
        foreach ((Point coordinate, TileDefinition[] chunk) in overlappingChunks)
        {
            Point chunkPosition = coordinate.ShiftLeft(EnvironmentRegistry.ChunkBits + EnvironmentRegistry.TileBits);
            int index = 0;
            foreach (TileDefinition definition in chunk)
            {
                int x = index & EnvironmentRegistry.ChunkMask;
                int y = index >> EnvironmentRegistry.ChunkBits;

                if (definition is null || !definition.IsSolid)
                {
                    index++;
                    continue;
                }

                Rectangle tile = new Rectangle(
                    tileSize * x, tileSize * y,
                    tileSize, tileSize);
                tile.Offset(chunkPosition);

                if (tile.Intersects(circle))
                {
                    overlapping.Add(tile);
                }

                index++;
            }
        }

        if (Props is not null)
        {
            overlapping.AddRange(Props.Where((prop) => prop.Definition.UsesCollision && prop.Collision.Intersects(circle)).Select((prop) => prop.Collision));
        }

        return overlapping;
    }

    public List<Rectangle> GetTileBoxes()
    {
        if (Chunks is null || Chunks.Count == 0)
        {
            return [];
        }

        List<Rectangle> boxes = [];
        int tileSize = EnvironmentRegistry.TileSize;
        foreach ((Point coordinate, TileDefinition[] chunk) in Chunks)
        {
            Point chunkPosition = coordinate.ShiftLeft(EnvironmentRegistry.ChunkBits + EnvironmentRegistry.TileBits);
            int index = 0;
            foreach (TileDefinition definition in chunk)
            {
                int x = index & EnvironmentRegistry.ChunkMask;
                int y = index >> EnvironmentRegistry.ChunkBits;

                if (definition is null || !definition.IsSolid)
                {
                    index++;
                    continue;
                }

                Rectangle tile = new Rectangle(
                    tileSize * x, tileSize * y,
                    tileSize, tileSize);
                tile.Offset(chunkPosition);

                boxes.Add(tile);
                index++;
            }
        }
        return boxes;
    }
}
