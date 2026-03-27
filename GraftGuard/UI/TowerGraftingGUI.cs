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
    private readonly Vector2 _createdTowerSize = new Vector2(72, 72);//new Vector2(72, 48);
    private readonly Vector2 _saveButtonSize = new Vector2(128, 64);
    private readonly Vector2 _removePartSize = new Vector2(112, 64);
    private readonly Vector2 _arrowButtonSize = new Vector2(32, 64);
    private readonly Vector2 _maxTowersLabelSize = new Vector2(112, 48);
    private const float _previewScale = 2.0f;
    private const float _arrowScrollSpeed = 256.0f;

    // Internal Projectile Manager
    private ProjectileManager _projectiles;

    // All UI Elements
    private List<IMouseDetectable> _allUI = [];

    // Tower Display
    private PatchLabel _towerDisplay;

    // Save Button
    private PatchButton _saveButton;
    // Remove Part Button
    private PatchButton _removePartButton;

    // Indices in these arrays point to matching data in each
    private List<Button> _towerChoiceButtons = [];
    private List<TowerDefinition> _towerChoices = [];

    // Same as above, but for parts
    private List<Button> _partChoiceButtons = [];
    private List<PartDefinition> _partChoices = [];

    // Arrow Button and Boolean
    private PatchButton _upButton;
    private PatchButton _downButton;
    private bool _showArrowButtons = false;

    // Label which displays the currently selected building option
    private PatchLabel _currentChosenLabel;

    // Tracking currently selected parts and towers
    private TowerDefinition _currentlyGraftingTower = null;
    private PartDefinition _currentlyChosenPart = null;
    private Tower _editingTower = null;

    // Tracking Created Towers and Offset
    private List<CreatedTowerButton> _createdTowers = [];
    private float _createdTowersOffset = 0.0f;

    // Night Button
    private PatchButton _nightButton;
    public event Clicked OnNightButtonPressed;

    // Created Tower Limit and Label
    private PatchLabel _maxTowersLabel;
    public int MaxAllowedTowers { get; set; } = 6;

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

        // Create Remove Part Button
        _removePartButton = PatchButton.MakeBase(
            _saveButton.Position - Vector2.UnitX * _removePartSize,
            _removePartSize,
            text: "Remove Part"
            );

        // Create Arrow Buttons
        _upButton = PatchButton.MakeBase(
            new Vector2(_createdTowerSize.X, Interface.ScreenCenter.Y - _arrowButtonSize.Y),
            _arrowButtonSize,
            text: @"/\"
            );
        _downButton = PatchButton.MakeBase(
            new Vector2(_createdTowerSize.X, Interface.ScreenCenter.Y),
            _arrowButtonSize,
            text: @"\/"
            );

        // Create Max Towers Label
        _maxTowersLabel = PatchLabel.MakeBase(
            text: $"Towers: 0 / {MaxAllowedTowers}",
            new Vector2(_createdTowerSize.X, 0),
            _maxTowersLabelSize
            );

        // Populate All UI
        _allUI.AddRange(_towerChoiceButtons);
        _allUI.AddRange(_partChoiceButtons);
        _allUI.Add(_currentChosenLabel);
        _allUI.Add(_nightButton);
        _allUI.Add(_saveButton);
        _allUI.Add(_upButton);
        _allUI.Add(_downButton);
        _allUI.Add(_maxTowersLabel);
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

        // Update Part Attachment
        if (!isOverUI)
        {
            if (inputManager.LeftMouseClicked() &&
                _currentlyChosenPart is not null &&
                _editingTower is not null &&
                _editingTower.IsOver(Vector2.Transform(inputManager.MouseScreenPosition.ToVector(), Matrix.CreateScale(1.0f / _previewScale))))
            {
                if (world.Inventory.GetPartCount(_currentlyChosenPart) > 0)
                {
                    _editingTower.AttachPart(_currentlyChosenPart);
                    world.Inventory.ModifyPartCount(_currentlyChosenPart, -1);
                }
            }
        }

        // Update Towers Selection
        for (int index = 0; index < _towerChoiceButtons.Count; index++)
        {
            Button button = _towerChoiceButtons[index];
            button.Update();
            if (button.JustClicked)
            {
                // Create the new Tower
                if (_editingTower is not null)
                {
                    RefundParts(_editingTower, world.Inventory);
                }
                _currentlyGraftingTower = _towerChoices[index];
                _editingTower = _currentlyGraftingTower.Factory((Interface.ScreenCenter - _towerDisplayOffset) / _previewScale);
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

        // Update Max Towers Label
        _maxTowersLabel.Text = $"Towers: {_createdTowers.Count} / {MaxAllowedTowers}";
        // Move Max Towers Text Color towards white
        _maxTowersLabel.TextColor = _maxTowersLabel.TextColor with
        {
          R = (byte)MathHelper.Clamp(_maxTowersLabel.TextColor.R + (int)(time.Delta() * 256), 0, 255),
          G = (byte)MathHelper.Clamp(_maxTowersLabel.TextColor.G + (int)(time.Delta() * 256), 0, 255),
          B = (byte)MathHelper.Clamp(_maxTowersLabel.TextColor.B + (int)(time.Delta() * 256), 0, 255)
        };

        // Update Save Button
        _saveButton.Update();

        // Handle Saving
        if (_saveButton.JustClicked && _editingTower is not null && _editingTower.HasParts)
        {
            // Allow saving if there is space
            if (_createdTowers.Count < MaxAllowedTowers)
            {
                _createdTowers.Add(
                new CreatedTowerButton(_editingTower,
                _currentlyGraftingTower,
                Vector2.Zero,
                _createdTowerSize
                ));
                _currentlyGraftingTower = null;
                _editingTower = null;
            }
            // Otherwise, if there is not space...
            else
            {
                // Set the non red channels of the Max Towers Label's Color to zero
                _maxTowersLabel.TextColor = _maxTowersLabel.TextColor with { G = 0, B = 0 };
            }
        }

        // Update Remove Part Button
        _removePartButton.Update();

        // Handle Part Removal
        if (_removePartButton.JustClicked && _editingTower is not null && _editingTower.HasParts)
        {
            PartDefinition removed = _editingTower.RemovePart();
            world.Inventory.ModifyPartCount(removed, 1);
        }

        // Update Created Towers
        for (int index = 0; index < _createdTowers.Count; index++)
        {
            CreatedTowerButton created = _createdTowers[index];
            created.Update(time, world, inputManager);
            created.Internal.Position = new Vector2(
                created.Internal.Position.X,
                _createdTowerSize.Y * index - _createdTowersOffset
                );
        }

        // Handle Selecting Created Towers
        for (int index = 0; index < _createdTowers.Count; index++)
        {
            CreatedTowerButton button = _createdTowers[index];
            
            if (button.Internal.JustClicked)
            {
                if (_editingTower is not null)
                {
                    // Refund the parts of the CURRENTLY editing Tower
                    RefundParts(_editingTower, world.Inventory);
                }
                // Set the selected Tower
                _currentlyGraftingTower = button.Definition;
                _editingTower = button.Tower;
                _editingTower.Position = (Interface.ScreenCenter - _towerDisplayOffset) / _previewScale;
                // Remove the selected Tower's button
                _createdTowers.RemoveAt(index);
                // Leave the loop
                break;
            }
        }

        // Calculate how much "beyond" the screen the created towers go
        float createdBeyond = MathF.Max(0.0f, -(Interface.ScreenSize.Y - _createdTowerSize.Y * _createdTowers.Count));
        _showArrowButtons = createdBeyond > 0.0f;

        // Update Arrow Buttons (If there are enough created towers)
        if (_showArrowButtons)
        {
            _upButton.Update();
            _downButton.Update();

            float delta = time.Delta();

            if (_upButton.IsPressed)
            {
                _createdTowersOffset -= delta * _arrowScrollSpeed;
            }
            if (_downButton.IsPressed)
            {
                _createdTowersOffset += delta * _arrowScrollSpeed;
            }
        }
        // Clamp Created Towers Offset
        _createdTowersOffset = MathHelper.Clamp(_createdTowersOffset, 0.0f, createdBeyond);

        // Update Night Button
        _nightButton.Update();
        if (_nightButton.JustClicked)
        {
            // Save Designs
            world.Inventory.Designs.AddRange(
                _createdTowers.Select(
                    (tower) => new TowerDesign(tower.Definition, tower.Tower.Parts.ToList())
                    )
                );
            // Start Night
            OnNightButtonPressed?.Invoke();
        }

        // Update Preview Tower
        _editingTower?.Update(time, world, inputManager, TimeState.Day, projectileDiversion: _projectiles);

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
        // Draw Background for created towers
        batch.Draw(Placeholders.TexturePixel, new Rectangle(0, 0, (int)_createdTowerSize.X, (int)Interface.ScreenSize.Y), new Color(0.0f, 0.0f, 0.0f, 0.4f));

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

        // Draw Remove Part Button
        _removePartButton.Draw(batch);

        // Draw Arrow Buttons (If there are enough created towers)
        if (_showArrowButtons)
        {
            _upButton.Draw(batch);
            _downButton.Draw(batch);
        }

        // Draw Tower Count Label
        _maxTowersLabel.Draw(batch);

        // Custom Tower Drawing
        batch.End();

        // Set Scissor Mask
        batch.GraphicsDevice.ScissorRectangle = _towerDisplay.Box;

        batch.Begin(
            transformMatrix: Matrix.CreateScale(_previewScale),
            samplerState: SamplerState.PointWrap,
            rasterizerState: new RasterizerState { ScissorTestEnable = true });

        // Draw Tower
        _editingTower?.Draw(time, batch, world, inputManager, TimeState.Day);

        // Draw Projectiles
        _projectiles.Draw(batch, time, world, inputManager);

        batch.End();
        // This should match the GUI static call in Game1
        batch.Begin(samplerState: SamplerState.PointWrap);

        // Draw the preview of the chosen part
        if (_currentlyChosenPart is not null)
        {
            batch.DrawCentered(_currentlyChosenPart.Texture, Mouse.GetState().Position.ToVector(), color: new Color(1.0f, 1.0f, 1.0f, 0.3f));
        }
    }

    /// <summary>
    /// Adds the parts currently on the given <paramref name="tower"/> to the <see cref="Inventory"/>. This does not modify the <see cref="Tower"/>
    /// </summary>
    /// <param name="tower"><see cref="Tower"/> to refund from</param>
    /// <param name="inventory"><see cref="Inventory"/> to refund to</param>
    public void RefundParts(Tower tower, Inventory inventory)
    {
        foreach (PartDefinition part in tower.Parts)
        {
            if (part is not null)
            {
                inventory.ModifyPartCount(part, 1);
            }
        }
    }
}
