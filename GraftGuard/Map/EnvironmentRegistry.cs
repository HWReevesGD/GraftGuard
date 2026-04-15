using GraftGuard.Map.Props;
using GraftGuard.Map.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GraftGuard.Map;
internal class EnvironmentRegistry
{
    public const int ChunkBits = 4;
    public const int ChunkSize = 1 << ChunkBits;
    public const int ChunkMask = ChunkSize - 1;
    public const int ChunkArea = 1 << ChunkSize;

    public const int TileBits = 4;
    public const int TileSize = 1 << TileBits;
    public const int TileMask = ChunkSize - 1;
    public const int TileArea = 1 << TileSize;

    public static List<PropDefinition> Props = [];
    public static List<TileDefinition> Tiles = [];
    public static MapDefinition Map;

    public static void LoadContent(ContentManager content)
    {
        // Open the Environment File
        using JsonDocument environment = JsonDocument.Parse(File.ReadAllText(Path.Join(content.RootDirectory, "Environment/environment.json")));
    
        // Get the Root Element
        JsonElement root = environment.RootElement;

        // Import Prop Definitions
        JsonElement propDefinitions = root.GetProperty("prop_definitions");
        int propCount = propDefinitions.GetArrayLength();
        Dictionary<string, PropDefinition> propsFromNames = [];
        for (int index = 0; index < propCount; index++)
        {
            PropDefinition prop = ImportPropDefinition(propDefinitions[index], content);
            propsFromNames[prop.Name] = prop;
        }

        // Get World Element
        JsonElement world = root.GetProperty("world");

        // Get Tiles Element
        JsonElement tiles = world.GetProperty("tiles");
        JsonElement tileDefinitions = tiles.GetProperty("source");
        Dictionary<int, TileDefinition> tileLibrary = [];

        // Import Tile Definitions
        int tileCount = tileDefinitions.GetArrayLength();
        for (int index = 0; index < tileCount; index++)
        {
            TileDefinition tile = ImportTileDefinition(tileDefinitions[index], content);
            tileLibrary[tileDefinitions[index].GetProperty("id").GetInt32()] = tile;
        }

        // Parse Placed Props
        JsonElement definedPlacedProps = world.GetProperty("props");
        JsonElement propLibraryLinks = definedPlacedProps.GetProperty("library");
        JsonElement placedPropIds = definedPlacedProps.GetProperty("placed_ids");
        JsonElement placedPropPositions = definedPlacedProps.GetProperty("placed_positions");
        Dictionary<int, string> propLibrary = [];
        List<PlacedProp> placedProps = [];

        foreach (JsonProperty link in propLibraryLinks.EnumerateObject())
        {
            propLibrary[int.Parse(link.Name)] = link.Value.GetString();
        }

        int placedPropCount = placedPropIds.GetArrayLength();
        for (int i = 0; i < placedPropCount; i++)
        {
            placedProps.Add(new PlacedProp(
                propsFromNames[propLibrary[placedPropIds[i].GetInt32()]],
                ImportVector2(placedPropPositions[i])
                ));
        }

        // Parse Enemy Spawns
        List<Vector2> enemySpawns = [];
        JsonElement spawns = world.GetProperty("spawns");
        foreach (JsonElement spawn in spawns.EnumerateArray())
        {
            enemySpawns.Add(
                ImportVector2(spawn)
                );
        }

        // Parse Pathing Area
        Rectangle pathingArea = ImportRectangle(world.GetProperty("pathing_area"));

        // Parse Placed Tiles
        Dictionary<Point, ReadOnlyCollection<TileDefinition>> tileChunks = [];
        JsonElement chunks = tiles.GetProperty("chunks");
        foreach (JsonElement chunk in chunks.EnumerateArray())
        {
            Point coordinate = ImportVector2(chunk.GetProperty("coordinate")).ToPoint();
            TileDefinition[] chunkTiles = new TileDefinition[256];
            JsonElement chunkTileIds = chunk.GetProperty("tiles");
            for (int index = 0; index < 256; index++)
            {
                int id = chunkTileIds[index].GetInt32();
                chunkTiles[index] = id == 0 ? null : tileLibrary[id];
            }
            tileChunks[coordinate] = chunkTiles.AsReadOnly();
        }

        // Parse Garage Position
        Vector2 garagePosition = ImportVector2(world.GetProperty("garage_position"));
        // Parse Scatter Position
        Vector2 scatterPosition = ImportVector2(world.GetProperty("scatter_position"));
        // Parse Player Spawn
        Vector2 playerSpawn = ImportVector2(world.GetProperty("player_spawn"));

        // Save a new Map
        Map = new MapDefinition()
        {
            PlacedProps = placedProps.AsReadOnly(),
            EnemySpawns = enemySpawns.AsReadOnly(),
            TileChunks = tileChunks.ToFrozenDictionary(),
            PathingArea = pathingArea,
            GaragePosition = garagePosition,
            ScatterPosition = scatterPosition,
            PlayerSpawn = playerSpawn,
        };
    }

    public static Vector2 ImportVector2(JsonElement vector2Element)
    {
        return new Vector2(
            vector2Element.GetProperty("X").GetSingle(),
            vector2Element.GetProperty("Y").GetSingle()
            );
    }

    public static Rectangle ImportRectangle(JsonElement rectangleElement)
    {
        return new Rectangle(
            ImportVector2(rectangleElement.GetProperty("position")).ToPoint(),
            ImportVector2(rectangleElement.GetProperty("size")).ToPoint()
            );
    }

    public static PropDefinition ImportPropDefinition(JsonElement propElement, ContentManager content)
    {
        PropDefinition prop = new(
            propElement.GetProperty("name").GetString(),
            content.Load<Texture2D>(Path.Join("Environment/Props", propElement.GetProperty("texture_file").GetString())),
            ImportRectangle(propElement.GetProperty("cutout_rectangle")),
            ImportVector2(propElement.GetProperty("sorting_origin")),
            propElement.GetProperty("uses_collision").GetBoolean(),
            ImportRectangle(propElement.GetProperty("collision_rectangle"))
            );
        Props.Add(prop);
        return prop;
    }

    public static TileDefinition ImportTileDefinition(JsonElement tileElement, ContentManager content)
    {
        TileDefinition tile = new TileDefinition(
            content.Load<Texture2D>(Path.Join("Environment/Tilesets", tileElement.GetProperty("texture").GetString())),
            ImportRectangle(tileElement.GetProperty("cutout")),
            tileElement.GetProperty("is_solid").GetBoolean()
            );
        Tiles.Add(tile);
        return tile;
    }
}
