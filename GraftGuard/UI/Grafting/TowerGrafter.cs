using GraftGuard.Grafting;
using GraftGuard.Grafting.Registry;
using GraftGuard.Grafting.Towers;
using GraftGuard.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GraftGuard.UI.Grafting;
internal class TowerGrafter
{
    private readonly Vector2 _towerButtonSize = new Vector2(72, 48);
    private readonly Vector2 _currentTowerLabelSize = new Vector2(128, 72);

    private List<Button> _towerChoiceButtons = [];
    private List<TowerDefinition> _towerChoices = [];
    private PatchLabel _currentTowerLabel;

    private TowerDefinition? _currentlyGraftingTower = null;

    public TowerGrafter()
    {
        _currentTowerLabel = PatchLabel.MakeBase("Current:\nNothing", Interface.TopRight - new Vector2(_currentTowerLabelSize.X, 0), _currentTowerLabelSize);

        int index = 0;
        foreach (TowerDefinition towerDefinition in TowerRegistry.Towers)
        {
            _towerChoices.Add(towerDefinition);
            _towerChoiceButtons.Add(
                PatchButton.MakeBase(
                    new Vector2(Interface.Width - _towerButtonSize.X, _currentTowerLabelSize.Y + _towerButtonSize.Y * index),
                    _towerButtonSize,
                    towerDefinition.Name
                    ));
            index++;
        }
    }

    public void Update(GameTime time)
    {
        int index = 0;
        foreach(Button button in _towerChoiceButtons)
        {
            button.Update();
            if (button.ClickedThisFrame)
            {
                _currentlyGraftingTower = _towerChoices[index];
                _currentTowerLabel.Text = "Current:\n" + (_currentlyGraftingTower is not null ? _currentlyGraftingTower.Value.Name : "Nothing");
            }
            index++;
        }
    }

    public void Draw(SpriteBatch batch, GameTime time)
    {
        _currentTowerLabel.Draw(batch);
        foreach (Button button in _towerChoiceButtons)
        {
            button.Draw(batch);
        }

        if (_currentlyGraftingTower is not null)
        {

        }
    }

}
