using GraftGuard.Data;
using GraftGuard.Grafting.Registry;
using GraftGuard.Grafting.Towers;
using GraftGuard.Map;
using GraftGuard.Map.Projectiles;
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
    // Constants / Readonly
    private readonly Vector2 _towerButtonSize = new Vector2(72, 48);
    private readonly Vector2 _currentTowerLabelSize = new Vector2(128, 128);
    private readonly Vector2 _partButtonSize = new Vector2(48, 48);
    private readonly Vector2 _towerDisplaySize = new Vector2(350, 350);
    private readonly Vector2 _towerDisplayOffset = new Vector2(0, 64);
    private readonly Vector2 _createdTowerSize = new Vector2(72, 48);
    private readonly Vector2 _saveButtonSize = new Vector2(128, 64);
    private const float _previewScale = 2.0f;

    // Internal Projectile Manager
    private ProjectileManager _projectiles;

    // All UI Elements
    private List<IMouseDetectable> _allUI = [];

    // Tower Display
    private PatchLabel _towerDisplay;

    // Save Button
    private PatchButton _saveButton;

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
    private Tower _previewTower = null;

    // Tracking Created Towers
    private List<CreatedTowerButton> _createdTowers = [];

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
                    new Vector2(Interface.ScreenCenter.X - (PartRegistry.Parts.Count * 0.5f * _partButtonSize.X) + _partButtonSize.X * index, Interface.ScreenSize.Y - _partButtonSize.Y),
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

        // Create Projectile Manager
        _projectiles = new ProjectileManager();

        // Create Save Button
        _saveButton = PatchButton.MakeBaseCentered(
            Interface.ScreenCenter - _towerDisplayOffset + Vector2.UnitY * _towerDisplaySize * 0.5f + Vector2.UnitY * _saveButtonSize * 0.5f,
            _saveButtonSize,
            "Save");

        // Populate All UI
        _allUI.AddRange(_towerChoiceButtons);
        _allUI.AddRange(_partChoiceButtons);
        _allUI.Add(_currentChosenLabel);
        _allUI.Add(_nightButton);
        _allUI.Add(_saveButton);
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
            if (inputManager.LeftMouseClicked() && _currentlyChosenPart is not null &&
                _previewTower.IsOver(Vector2.Transform(inputManager.MouseScreenPosition.ToVector(),
                    Matrix.CreateScale(1.0f / _previewScale))))
            {
                _previewTower.AttachPart(_currentlyChosenPart);
            }
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
                _previewTower = _currentlyGraftingTower.Factory((Interface.ScreenCenter - _towerDisplayOffset) / _previewScale);
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

        // Update Created Towers
        foreach (CreatedTowerButton created in _createdTowers)
        {
            created.Update(time, world, inputManager);
        }

        // Update Save Button
        _saveButton.Update();

        // Handle Saving
        if (_saveButton.JustClicked && _previewTower is not null && _previewTower.HasParts)
        {
            _createdTowers.Add(
                new CreatedTowerButton(_previewTower,
                Vector2.Zero,
                _createdTowerSize
                ));
            _previewTower = null;
        }

        // Update Night Button
        _nightButton.Update();
        if (_nightButton.JustClicked)
        {
            OnNightButtonPressed?.Invoke();
        }

        // Update Preview Tower
        _previewTower?.Update(time, world, inputManager, TimeState.Day, projectileDiversion: _projectiles);

        // Update Projectiles
        _projectiles.Update(time, world, inputManager);
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
        // Draw the preview of the chosen part
        if (_currentlyChosenPart is not null)
        {
            batch.DrawCentered(_currentlyChosenPart.Texture, Mouse.GetState().Position.ToVector(), color: new Color(1.0f, 1.0f, 1.0f, 0.3f));
        }

        // Draw Night Button
        _nightButton.Draw(batch);
        // Draw Tower Display Border
        _towerDisplay.Draw(batch);

        // Draw Created Towers
        foreach (CreatedTowerButton created in _createdTowers)
        {
            created.Draw(batch, time, world, inputManager);
        }

        // Draw Save Button
        _saveButton.Draw(batch);

        // Custom Tower Drawing
        batch.End();

        // Set Scissor Mask
        batch.GraphicsDevice.ScissorRectangle = _towerDisplay.Box;

        batch.Begin(
            transformMatrix: Matrix.CreateScale(_previewScale),
            samplerState: SamplerState.PointWrap,
            rasterizerState: new RasterizerState { ScissorTestEnable = true });

        // Draw Tower
        _previewTower?.Draw(time, batch, world, inputManager, TimeState.Day);

        // Draw Projectiles
        _projectiles.Draw(batch, time, world, inputManager);

        batch.End();
        // This should match the GUI static call in Game1
        batch.Begin(samplerState: SamplerState.PointWrap);
    }
}
