using GraftGuard.Grafting.Towers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraftGuard.Grafting.Registry;
internal static class TowerRegistry
{
    private static List<TowerDefinition> _allTowers = [];
    public static ReadOnlyCollection<TowerDefinition> Towers => _allTowers.AsReadOnly();
    public static void Register(string towerName, CreateTower towerFactory, DrawPreview drawPreview, HashSet<PartAmount> requiredResources, Texture2D? towerIcon = null)
    {
        _allTowers.Add(new TowerDefinition(towerName, towerFactory, drawPreview, requiredResources, icon: towerIcon));
    }
}