using GraftGuard.Grafting.Towers;
using GraftGuard.Utility;
using Microsoft.Xna.Framework.Graphics;

using System.Collections.Frozen;
using System.Collections.Generic;

namespace GraftGuard.Grafting.Registry;

/// <summary>
/// Structure which holds static data pertaining to a single tower type.
/// This should not be confused with <see cref="Tower"/>, which is the base class for
/// *Instantiated* towers, which <see cref="TowerDefinition"/> is for data such as the Tower's name, icon, and factory method
/// </summary>
internal class TowerDefinition
{
    public readonly string Name;
    public readonly CreateTower Factory;
    public readonly DrawPreview DrawPreview;
    public readonly Texture2D Icon;
    public readonly FrozenSet<PartAmount> Cost;
    public readonly int RoundUnlocked;
    /// <summary>
    /// Creates a new <see cref="TowerDefinition"/>
    /// </summary>
    /// <param name="name">Tower name</param>
    /// <param name="factory">Tower creation factory method</param>
    /// <param name="drawPreview">Draw preview method</param>
    /// <param name="icon">Part's icon texture</param>
    public TowerDefinition(string name, CreateTower factory, DrawPreview drawPreview, Texture2D? icon = null, int roundUnlocked = 0)
    {
        Name = name;
        Factory = factory;
        DrawPreview = drawPreview;
        Icon = icon ?? Placeholders.TextureMissingIcon;
        RoundUnlocked = roundUnlocked;
    }
}
