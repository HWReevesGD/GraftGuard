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
    public Dictionary<Point, RenderTarget2D> RenderingChunks;
    public List<PlacedProp> Props;
    private Player player;
    private Texture2D _terrain = Placeholders.TexturePixel;

    public Terrain(Player player)
    {
        this.player = player;
        RenderingChunks = [];
        Chunks = [];
        Props = [];
    }

    public void LoadMap(MapDefinition map, DrawManager drawing)
    {
        Chunks = map.TileChunks.ToDictionary((pair) => pair.Key, (pair) => pair.Value.ToArray());
        Props = map.PlacedProps.ToList();
        RenderingChunks.Clear();
        foreach (Point coordinate in Chunks.Keys)
        {
            RenderingChunks[coordinate] = new RenderTarget2D(drawing.Batch.GraphicsDevice, EnvironmentRegistry.ChunkSize << EnvironmentRegistry.TileBits, EnvironmentRegistry.ChunkSize << EnvironmentRegistry.TileBits);
        }
        RenderChunks(drawing);
    }

    public void Update(GameTime time)
    {

    }

    public void Draw(DrawManager drawing, GameTime time)
    {
        if (Chunks is null)
        {
            return;
        }

        foreach ((Point coordinate, TileDefinition[] chunk) in Chunks)
        {
            Vector2 chunkPosition = coordinate.ShiftLeft(EnvironmentRegistry.ChunkBits + EnvironmentRegistry.TileBits).ToVector();
            drawing.Draw(RenderingChunks[coordinate], chunkPosition, sortMode: SortMode.Bottom, drawLayer: 0);
        }

        if (Props is not null)
        {
            foreach (PlacedProp prop in Props)
            {
                prop.Draw(drawing, player);
            }
        }
    }

    public void RenderChunks(DrawManager drawing)
    {
        GraphicsDevice device = drawing.Batch.GraphicsDevice;

        foreach ((Point coordinate, RenderTarget2D target) in RenderingChunks)
        {
            TileDefinition[] chunk = Chunks[coordinate];
            // Set the device to the render target
            device.SetRenderTarget(target);

            // Clear the graphics buffer to a solid color
            device.Clear(Color.Black);

            drawing.Batch.Begin(sortMode: SpriteSortMode.Deferred, blendState: BlendState.NonPremultiplied, samplerState: SamplerState.PointClamp);

            int index = 0;
            foreach (TileDefinition tile in chunk)
            {
                if (tile is null)
                {
                    index++;
                    continue;
                }

                float x = (index & EnvironmentRegistry.ChunkMask) << EnvironmentRegistry.TileBits;
                float y = (index >> EnvironmentRegistry.ChunkBits) << EnvironmentRegistry.TileBits;

                drawing.Batch.Draw(tile.Texture, position: new Vector2(x, y), sourceRectangle: tile.Cutout, color: Color.White);
                index++;
            }

            drawing.Batch.End();

            // Reset the device to the back buffer
            device.SetRenderTarget(null);
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
