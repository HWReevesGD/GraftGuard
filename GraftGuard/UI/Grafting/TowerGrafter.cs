using GraftGuard.Grafting;
using GraftGuard.Grafting.Registry;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GraftGuard.UI.Grafting;
internal class TowerGrafter
{
    private readonly Vector2 _towerButtonSize = new Vector2(200, 100);

    private List<Button> _towerChoiceButtons = [];

    public TowerGrafter()
    {
        foreach (TowerDefinition towerDefinition in TowerRegistry.Towers)
        {
            
        }
    }

}
