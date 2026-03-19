using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace GraftGuard.Grafting.Registry;
/// <summary>
/// Collection holding all <see cref="PartDefinition"/>s currently loaded into the game
/// </summary>
internal static class PartRegistry
{
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
    public static void Register(
        string name, Texture2D texture, PartType type, float baseDamage, float speedModifier = 1.0f,
        float armorModifier = 1.0f, float rangeModifier = 1.0f, float criticalModifier = 1.0f,
        float healthModifier = 1.0f)
    {
        _allParts.Add(new PartDefinition(name, texture, type, baseDamage, speedModifier, armorModifier, rangeModifier, criticalModifier, healthModifier));
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
}
