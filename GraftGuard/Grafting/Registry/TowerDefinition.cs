using GraftGuard.Grafting.Towers;
using GraftGuard.Utility;
using Microsoft.Xna.Framework.Graphics;

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
    public TowerDefinition(string name, CreateTower factory, DrawPreview drawPreview, Texture2D? icon = null)
    {
        Name = name;
        Factory = factory;
        DrawPreview = drawPreview;
        Icon = icon ?? Placeholders.TextureMissingIcon;
    }
}