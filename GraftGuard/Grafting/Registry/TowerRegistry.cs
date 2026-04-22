using GraftGuard.Grafting.Towers;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace GraftGuard.Grafting.Registry;
internal static class TowerRegistry
{
    private static List<TowerDefinition> _allTowers = [];
    public static ReadOnlyCollection<TowerDefinition> Towers => _allTowers.AsReadOnly();
    public static void Register(string towerName, CreateTower towerFactory, DrawPreview drawPreview, Texture2D? towerIcon = null, int roundUnlocked = 0)
    {
        _allTowers.Add(new TowerDefinition(towerName, towerFactory, drawPreview, icon: towerIcon, roundUnlocked));
    }
}