using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text.Json;

namespace GraftGuard.Grafting.Registry;
/// <summary>
/// Collection holding all <see cref="PartDefinition"/>s currently loaded into the game
/// </summary>
internal static class PartRegistry
{
    private static Random random = new Random();
    private static List<PartDefinition> _allParts = [];
    public static ReadOnlyCollection<PartDefinition> Parts => _allParts.AsReadOnly();


    /// <summary>
    /// Loads the JSON library exported from Grafter and populates the registry.
    /// </summary>
    public static void LoadFromLibrary(ContentManager content, string jsonPath)
    {
        Debug.WriteLine("--- PART REGISTRY: STARTING LOAD ---");
        jsonPath = Path.Join(content.RootDirectory, jsonPath);
        Debug.WriteLine($"Attempting to load from: {Path.GetFullPath(jsonPath)}");

        if (!File.Exists(jsonPath)) return;

        string json = File.ReadAllText(jsonPath);
        var library = JsonSerializer.Deserialize<GraftLibrary>(json);

        if (library != null && library.Parts != null)
        {
            _allParts.Clear();

            foreach (var data in library.Parts)
            {
                Debug.WriteLine($"Loading: {data.TextureName}!");
                Texture2D tex = content.Load<Texture2D>($"Parts/{data.TextureName}");

                // Create the definition with pivot data
                var definition = new PartDefinition(
                    data.Name,
                    tex,
                    data.TextureName,
                    data.PivotX,
                    data.PivotY,
                    data.Type,
                    data.BaseDamage,
                    data.SpeedModifier,
                    data.ArmorModifier,
                    data.RangeModifier,
                    data.CriticalModifier,
                    data.HealthModifier,
                    data.PartBehaviorNames
                );

                _allParts.Add(definition);
            }
        }
    }

    /// <summary>
    /// Gets a <see cref="PartDefinition"/> from the Registry by its name, case-insensitive
    /// </summary>
    /// <param name="name">Name of part to search for</param>
    /// <returns>Found <see cref="PartDefinition"/></returns>
    public static PartDefinition GetByName(string name)
    {
        PartDefinition definition = _allParts.Find((part) => part.Name.ToLower() == name.ToLower());
        return definition;
    }

    /// <summary>
    /// Gets a random <see cref="PartDefinition"/>
    /// </summary>
    /// <returns>Returns a random <see cref="PartDefinition"/></returns>
    public static PartDefinition GetRandom()
    {
        return _allParts[random.Next(_allParts.Count)];
    }

}
