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
using System.Reflection.Emit;
using System.Reflection.Metadata.Ecma335;

namespace GraftGuard.Grafting;

delegate void NightButtonPressed();

/// <summary>
/// Handles the Grafting Interface and Part Inventory
/// </summary>
internal class TowerGraftingGUI
{
    // Constants for button sizing
    private readonly Vector2 _towerButtonSize = new Vector2(72, 48);
    private readonly Vector2 _currentTowerLabelSize = new Vector2(128, 128);
    private readonly Vector2 _partButtonSize = new Vector2(48, 48);
    private readonly Vector2 _towerDisplaySize = new Vector2(350, 350);
    private readonly Vector2 _towerPartAttachmentSize = new Vector2(64, 64);
    private readonly Vector2 _towerDisplayOffset = new Vector2(0, 64);

    // All UI Elements
    private List<IMouseDetectable> _allUI = [];

    // Tower Display and Buttons
    private PatchLabel _towerDisplay;
    private List<PatchButton> _towerPartAttachments = [];

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

    // Night Button
    private PatchButton _nightButton;
    public event Clicked OnNightButtonPressed;

    /// <summary>
    /// Creates a new <see cref="TowerGraftingGUI"/>
    /// </summary>
    public TowerGraftingGUI()
    {
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

        // Create Night Button
        _nightButton = PatchButton.MakeBase(new Vector2(Interface.ScreenSize.X - _towerButtonSize.X, Interface.ScreenSize.Y - _towerButtonSize.Y), _towerButtonSize, "Begin");
        if (_nightButton.JustClicked)
        {
            Console.WriteLine("Click");
            OnNightButtonPressed();
        }

        // The default tower is the first in the TowerRegistry
        _currentlyGraftingTower = TowerRegistry.Towers[0];

        // Tower Display
        _towerDisplay = PatchLabel.MakeBaseCentered(
            text: "",
            position: Interface.ScreenCenter - _towerDisplayOffset,
            size: _towerDisplaySize
            );

        // Tower Display Part Attachment Buttons
        for (int index = 0; index < Tower.MaxParts; index++)
        {
            _towerPartAttachments.Add(
            PatchButton.MakeBase(
                position: Interface.ScreenCenter + Vector2.UnitY * _towerDisplaySize * 0.5f
                    - Vector2.UnitX * (_towerPartAttachmentSize * Tower.MaxParts * 0.5f)
                    + Vector2.UnitX * _towerPartAttachmentSize * index
                    - _towerDisplayOffset,
                size: _towerPartAttachmentSize
                )
            );
        }

        // Populate All UI
        _allUI.AddRange(_towerChoiceButtons);
        _allUI.AddRange(_partChoiceButtons);
        _allUI.Add(_currentChosenLabel);
        _allUI.Add(_nightButton);
        _allUI.AddRange(_towerPartAttachments);
    }

    /// <summary>
    /// Updates the <see cref="TowerGraftingGUI"/>
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
            // TODO: Handle Part Attaching
        }

        // Update Towers Selection
        for (int index = 0; index < _towerChoiceButtons.Count; index++)
        {
            Button button = _towerChoiceButtons[index];
            button.Update();
            if (button.JustClicked)
            {
                // Select the tower
                _currentlyGraftingTower = _towerChoices[index];
            }
        }
        // Update Parts Selection
        for (int index = 0; index < _partChoiceButtons.Count; index++)
        {
            Button button = _partChoiceButtons[index];
            button.Text = $"{world.Inventory.GetPartCount(_partChoices[index])}";

            button.Update();
            if (button.JustClicked)
            {
                // Select the part
                _currentlyChosenPart = _partChoices[index];
                _currentChosenLabel.Text = "Current:\n" + (_currentlyChosenPart is not null ? _currentlyChosenPart.Name : "Nothing");
            }
        }

        // Update Night Button
        _nightButton.Update();
        if (_nightButton.JustClicked)
        {
            
            Console.WriteLine("Begin button pressed");
        }
    }

    /// <summary>
    /// Draws the <see cref="TowerGraftingGUI"/> Interface
    /// </summary>
    /// <param name="batch"><see cref="SpriteBatch"/> to use</param>
    /// <param name="time">Game Time</param>
    public void Draw(SpriteBatch batch, GameTime time, World world, InputManager inputManager)
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

        // Draw Night Button
        _nightButton.Draw(batch);
        // Draw Tower Display Border
        _towerDisplay.Draw(batch);
        
        // Draw Part Attachment Buttons
        foreach (PatchButton button in _towerPartAttachments)
        {
            button.Draw(batch);
        }
    }
}
