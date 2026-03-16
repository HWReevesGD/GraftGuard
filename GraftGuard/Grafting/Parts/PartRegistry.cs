using GraftGuard.Grafting.Registry;
using GraftGuard.Grafting.Towers;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraftGuard.Grafting.Parts;
internal static class PartRegistry
{
    private static List<PartDefinition> _allParts = [];
    public static ReadOnlyCollection<PartDefinition> Parts => _allParts.AsReadOnly();
    public static void Register(
        string name, Texture2D texture, PartType type, float baseDamage, float speedModifier = 1.0f,
        float armorModifier = 1.0f, float rangeModifier = 1.0f, float criticalModifier = 1.0f,
        float healthModifier = 1.0f)
    {
        _allParts.Add(new PartDefinition(name, texture, type, baseDamage, speedModifier, armorModifier, rangeModifier, criticalModifier, healthModifier));
    }
}
