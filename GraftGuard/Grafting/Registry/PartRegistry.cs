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
    /// Register a new <see cref="PartDefinition"/>
    /// </summary>
    /// <param name="name">Part name</param>
    /// <param name="texture">Part texture</param>
    /// <param name="type">Part Type, either Limb or Head</param>
    /// <param name="baseDamage">Base damage of the part</param>
    /// <param name="speedModifier">Part speed modifier</param>
    /// <param name="armorModifier">Part armor modifier</param>
    /// <param name="rangeModifier">Part range modifier</param>
    /// <param name="criticalModifier">Part critical hit chance multiplier</param>
    /// <param name="healthModifier">Part health modifier</param>
    /* Preserving for now
     * public static void Register(
        string name, Texture2D texture, PartType type, float baseDamage, float speedModifier = 1.0f,
        float armorModifier = 1.0f, float rangeModifier = 1.0f, float criticalModifier = 1.0f,
        float healthModifier = 1.0f)
    {
        _allParts.Add(new PartDefinition(name, texture, type, baseDamage, speedModifier, armorModifier, rangeModifier, criticalModifier, healthModifier));
    }*/

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
                    PartType.Limb,
                    data.BaseDamage,
                    data.SpeedModifier,
                    data.ArmorModifier,
                    data.RangeModifier,
                    data.CriticalModifier,
                    data.HealthModifier
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
