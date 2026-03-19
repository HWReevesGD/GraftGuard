using GraftGuard.Grafting.Registry;
using GraftGuard.Grafting.Towers;
using GraftGuard.Map;
using GraftGuard.UI;
using GraftGuard.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;

namespace GraftGuard.Grafting;

/// <summary>
/// Handles the Grafting Interface and Part Inventory
/// </summary>
internal class TowerGrafter
{
    // Constants for button sizing
    private readonly Vector2 _towerButtonSize = new Vector2(72, 48);
    private readonly Vector2 _currentTowerLabelSize = new Vector2(128, 128);
    private readonly Vector2 _partButtonSize = new Vector2(48, 48);

    // All UI Elements
    private List<IMouseDetectable> _allUI = [];

    // Indices in these arrays point to matching data in each
    private List<Button> _towerChoiceButtons = [];
    private List<TowerDefinition> _towerChoices = [];

    // Same as above, but for parts
    private List<Button> _partChoiceButtons = [];
    private List<PartDefinition> _partChoices = [];

    // Label which displays the currently selected building option
    private PatchLabel _currentChosenLabel;

    // Tracking currently selected parts and towers
    private TowerDefinition _currentlyGraftingTower = null;
    private PartDefinition _currentlyChosenPart = null;

    // Inventory Dictionary
    private Dictionary<string, int> _inventory;

    /// <summary>
    /// Creates a new <see cref="TowerGrafter"/>
    /// </summary>
    public TowerGrafter(TowerManager towerManager)
    {
        _inventory = [];
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

    /// <summary>
    /// Updates the <see cref="TowerGrafter"/>
    /// </summary>
    /// <param name="time">Game Time</param>
    /// <param name="inputManager">Current Input Manager</param>
    /// <param name="world">World Owner</param>
    public void Update(GameTime time, InputManager inputManager, World world)
    {
        TowerManager towerManager = world.TowerManager;
        bool isOverUI = _allUI.Any((uiPart) => uiPart.IsMouseOver(inputManager));

        if (!isOverUI)
        {
            // Handle Tower Placement
            if (inputManager.LeftMouseClicked() && _currentlyGraftingTower is TowerDefinition tower)
            {
                if (CanAffordTower(tower))
                {
                    RemoveCostOfTower(tower);
                    towerManager.MakeTower(tower, inputManager.MouseWorldPosition.ToVector());
                }
            }
            // Handle Part Attaching
            if (inputManager.LeftMouseClicked() && _currentlyChosenPart is PartDefinition part && towerManager.GetFirstTowerAtMousePosition(inputManager) is Tower overTower)
            {
                // Don't allow part attachment if the player does not have enough parts
                if (GetPartCount(part) != 0)
                {
                    overTower.AttachPart(part);
                    ModifyPartCount(part, -1);
                }
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

                _currentChosenLabel.Text += "\n-- Cost --";
                FrozenSet<PartAmount> cost = _currentlyGraftingTower.Cost;
                foreach (PartAmount amount in cost)
                {
                    _currentChosenLabel.Text += $"\n{amount.Amount} - {amount.Part.Name}";
                }
            }
        }
        // Update Parts
        for (int index = 0; index < _partChoiceButtons.Count; index++)
        {
            Button button = _partChoiceButtons[index];
            button.Text = $"{GetPartCount(_partChoices[index])}";

            button.Update();
            if (button.ClickedThisFrame)
            {
                Deselect();

                _currentlyChosenPart = _partChoices[index];
                _currentChosenLabel.Text = "Current:\n" + (_currentlyChosenPart is not null ? _currentlyChosenPart.Name : "Nothing");
            }
        }
    }

    /// <summary>
    /// Draws the <see cref="TowerGrafter"/> Interface
    /// </summary>
    /// <param name="batch"><see cref="SpriteBatch"/> to use</param>
    /// <param name="time">Game Time</param>
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

    /// <summary>
    /// Deselects all selected parts and towers
    /// </summary>
    public void Deselect()
    {
        _currentlyGraftingTower = null;
        _currentlyChosenPart = null;
    }

    /// <summary>
    /// Checks if the given <see cref="TowerDefinition"/> can be afforded with the current Inventory
    /// </summary>
    /// <param name="tower"><see cref="TowerDefinition"/> to check</param>
    /// <returns>True if the <see cref="TowerDefinition"/> can be afforded, false otherwise</returns>
    public bool CanAffordTower(TowerDefinition tower)
    {
        return tower.Cost.All((amount) => GetPartCount(amount.Part) >= amount.Amount);
    }

    /// <summary>
    /// Removes the required cost of building a <see cref="TowerDefinition"/> from the Inventory.
    /// This will throw a <see cref="ArgumentOutOfRangeException"/> if the <see cref="TowerDefinition"/> cannot be afforded
    /// </summary>
    /// <param name="tower"><see cref="TowerDefinition"/> to get cost from</param>
    public void RemoveCostOfTower(TowerDefinition tower)
    {
        foreach (PartAmount amount in tower.Cost)
        {
            ModifyPartCount(amount.Part, -amount.Amount);
        }
    }

    /// <summary>
    /// Returns the count for the given <paramref name="part"/>
    /// </summary>
    /// <param name="part">Part to get count for</param>
    /// <returns>Number of this part in inventory</returns>
    public int GetPartCount(PartDefinition part) => GetPartCount(part.Name);
    /// <summary>
    /// Returns the count for the given <paramref name="partName"/>
    /// </summary>
    /// <param name="partName">Part to get count for</param>
    /// <returns>Number of this part in inventory</returns>
    public int GetPartCount(string partName)
    {
        partName = partName.ToLower();
        return _inventory.GetValueOrDefault(partName, 0);
    }
    /// <summary>
    /// Sets the count for the give <paramref name="part"/>
    /// </summary>
    /// <param name="part">Part to set count for</param>
    /// <param name="value">Count value to set</param>
    public void SetPartCount(PartDefinition part, int value) => SetPartCount(part.Name, value);
    /// <summary>
    /// Sets the count for the give <paramref name="partName"/>
    /// </summary>
    /// <param name="partName">Part to set count for</param>
    /// <param name="value">Count value to set</param>
    public void SetPartCount(string partName, int value)
    {
        partName = partName.ToLower();
        if (value < 0)
        {
            throw new ArgumentOutOfRangeException("Cannot modify a part count to a negative number");
        }
        _inventory[partName] = value;
    }
    /// <summary>
    /// Modifiers the count of the given <paramref name="part"/>
    /// </summary>
    /// <param name="part">Part to modify count of</param>
    /// <param name="change">Amount to modify count by</param>
    public void ModifyPartCount(PartDefinition part, int change) => ModifyPartCount(part.Name, change);
    /// <summary>
    /// Modifiers the count of the given <paramref name="partName"/>
    /// </summary>
    /// <param name="partName">Part to modify count of</param>
    /// <param name="change">Amount to modify count by</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the part count is changed below zero</exception>
    public void ModifyPartCount(string partName, int change)
    {
        partName = partName.ToLower();
        if (GetPartCount(partName) + change < 0)
        {
            throw new ArgumentOutOfRangeException("Cannot modify a part count to a negative number");
        }
        _inventory[partName] = GetPartCount(partName) + change;
    }

}
