using GraftGuard.Grafting.Registry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraftGuard.Grafting.Towers;

namespace GraftGuard.Grafting;
/// <summary>
/// Class used for holding a single <see cref="Tower"/> design from the <see cref="TowerGraftingGUI"/>
/// </summary>
internal class TowerDesign
{
    public TowerDefinition Definition;
    public List<PartDefinition> Parts;

    public TowerDesign(TowerDefinition definition, List<PartDefinition> parts)
    {
        Definition = definition;
        Parts = parts.ToList();
    }
}
