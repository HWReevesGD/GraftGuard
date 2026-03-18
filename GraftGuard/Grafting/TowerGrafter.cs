using GraftGuard.Grafting.Registry;
using GraftGuard.Grafting.Towers;
using GraftGuard.UI;
using GraftGuard.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GraftGuard.Grafting;
internal class TowerGrafter
{
    // Constants for button sizing
    private readonly Vector2 _towerButtonSize = new Vector2(72, 48);
    private readonly Vector2 _currentTowerLabelSize = new Vector2(128, 72);
    private readonly Vector2 _partButtonSize = new Vector2(48, 48);

    // All UI Elements
    private List<IMouseDetectable> _allUI = [];

    // Indices in these arrays point to matching data in each
    private List<Button> _towerChoiceButtons = [];
    private List<TowerDefinition> _towerChoices = [];

    // Same as above, but for parts
    private List<Button> _partChoiceButtons = [];
    private List<PartDefinition> _partChoices = [];

    private PatchLabel _currentChosenLabel;

    private TowerDefinition _currentlyGraftingTower = null;
    private PartDefinition _currentlyChosenPart = null;

    private TowerManager _towerManager;

    public TowerGrafter(TowerManager towerManager)
    {
        _towerManager = towerManager;
        _currentChosenLabel = PatchLabel.MakeBase("Current:\nNothing", Interface.TopRight - new Vector2(_currentTowerLabelSize.X, 0), _currentTowerLabelSize);

        // Populate Towers
        for (int index = 0; index < TowerRegistry.Towers.Count; index++)
        {
            TowerDefinition towerDefinition = TowerRegistry.Towers[index];
            _towerChoices.Add(towerDefinition);
            _towerChoiceButtons.Add(
                PatchButton.MakeBase(
                    new Vector2(Interface.Width - _towerButtonSize.X, _currentTowerLabelSize.Y + _towerButtonSize.Y * index),
                    _towerButtonSize,
                    towerDefinition.Name
                    ));
        }

        // Populate Parts
        for (int index = 0; index < PartRegistry.Parts.Count; index++)
        {
            PartDefinition partDefinition = PartRegistry.Parts[index];
            _partChoices.Add(partDefinition);
            _partChoiceButtons.Add(
                PatchButton.MakeBase(
                    new Vector2(0, _partButtonSize.Y * index),
                    _partButtonSize,
                    icon: partDefinition.Texture
                    ));
        }

        // Populate All UI
        _allUI.AddRange(_towerChoiceButtons);
        _allUI.AddRange(_partChoiceButtons);
        _allUI.Add(_currentChosenLabel);
    }

    public void Update(GameTime time, InputManager inputManager)
    {
        bool isOverUI = _allUI.Any((uiPart) => uiPart.IsMouseOver(inputManager));

        if (!isOverUI)
        {
            // Handle Tower Placement
            if (inputManager.LeftMouseClicked() && _currentlyGraftingTower is TowerDefinition tower)
            {
                _towerManager.MakeTower(tower, inputManager.MouseWorldPosition.ToVector());
            }
            // Handle Part Attaching
            if (inputManager.LeftMouseClicked() && _currentlyChosenPart is PartDefinition part && _towerManager.GetFirstTowerAtMousePosition(inputManager) is Tower overTower)
            {
                overTower.AttachPart(part);
            }
        }

        // Update Towers
        for (int index = 0; index < _towerChoiceButtons.Count; index++)
        {
            Button button = _towerChoiceButtons[index];
            button.Update();
            if (button.ClickedThisFrame)
            {
                Deselect();
                _currentlyGraftingTower = _towerChoices[index];
                _currentChosenLabel.Text = "Current:\n" + (_currentlyGraftingTower is not null ? _currentlyGraftingTower.Name : "Nothing");
            }
        }
        // Update Parts
        for (int index = 0; index < _partChoiceButtons.Count; index++)
        {
            Button button = _partChoiceButtons[index];
            button.Update();
            if (button.ClickedThisFrame)
            {
                Deselect();
                _currentlyChosenPart = _partChoices[index];
                _currentChosenLabel.Text = "Current:\n" + (_currentlyChosenPart is not null ? _currentlyChosenPart.Name : "Nothing");
            }
        }
    }

    public void Draw(SpriteBatch batch, GameTime time)
    {
        // Draw the Label showing the Currently Grafting Tower
        _currentChosenLabel.Draw(batch);
        // Draw each Tower Button
        foreach (Button button in _towerChoiceButtons)
        {
            button.Draw(batch);
        }
        // Draw each Part Button
        foreach (Button button in _partChoiceButtons)
        {
            button.Draw(batch);
        }
        // Draw the preview of the chosen Tower
        if (_currentlyGraftingTower is not null)
        {
            _currentlyGraftingTower.DrawPreview(batch, time, Mouse.GetState().Position.ToVector());
        }
        // Draw the preview of the chosen part
        if (_currentlyChosenPart is not null)
        {
            batch.DrawCentered(_currentlyChosenPart.Texture, Mouse.GetState().Position.ToVector(), color: new Color(1.0f, 1.0f, 1.0f, 0.3f));
        }
    }

    public void Deselect()
    {
        _currentlyGraftingTower = null;
        _currentlyChosenPart = null;
    }

}
