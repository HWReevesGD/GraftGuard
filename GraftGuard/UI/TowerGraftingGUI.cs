using GraftGuard.Data;
using GraftGuard.Grafting.Registry;
using GraftGuard.Grafting.Towers;
using GraftGuard.Graphics;
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
using System.Reflection;
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
    private readonly static float guiScale = 2.0f;

    private readonly static Vector2 _towerButtonSize = new Vector2(72, 48) * guiScale;
    private readonly static Vector2 _currentChosenLabelSize = new Vector2(350, 48) * guiScale;
    private readonly static Vector2 _partButtonSize = new Vector2(48, 48) * guiScale;
    private readonly static Vector2 _towerDisplaySize = new Vector2(350, 350) * guiScale;
    private readonly static Vector2 _towerDisplayOffset = new Vector2(0, 64) * guiScale;
    private readonly static Vector2 _createdTowerSize = new Vector2(72, 72) * guiScale;
    private readonly static Vector2 _saveButtonSize = new Vector2(128, 56) * guiScale;
    private readonly static Vector2 _removePartSize = new Vector2(112, 56) * guiScale;
    private readonly static Vector2 _maxTowersLabelSize = new Vector2(110, 56) * guiScale;
    private readonly static Vector2 _partSelectorPosition = new Vector2(0, 64);
    private readonly static Vector2 _towerSelectorPositionOffset = new Vector2(0, 64);
    private readonly static Vector2 _partSelectorSizeOffset = new Vector2(0, 64);
    private const float _arrowCreatedTowerButtonOffset = -256.0f * 2;
    private const float _previewScale = 4.0f;

    // Internal Projectile Manager
    private ProjectileManager _projectiles;

    // Tower Display
    private PatchLabel _towerDisplay;

    // Save Button
    private PatchButton _saveButton;
    // Remove Part Button
    private PatchButton _removePartButton;

    // Indices in these arrays point to matching data in each
    private ScrollingGrid<PatchButton> _towerChoiceButtons;
    private List<TowerDefinition> _towerChoices;

    // Same as above, but for parts
    private ScrollingGrid<PatchButton> _partChoiceButtons;
    private List<PartDefinition> _partChoices;

    // Label which displays the currently selected building option
    private PatchLabel _currentChosenLabel;

    // Tracking currently selected parts and towers
    private TowerDefinition _currentlyGraftingTower = null;
    private PartDefinition _currentlyChosenPart = null;
    private Tower _editingTower = null;

    // Created Towers Grid
    private ScrollingGrid<CreatedTowerButton> _createdTowers;

    // Night Button
    private PatchButton _nightButton;
    public event Clicked OnNightButtonPressed;

    // Created Tower Limit and Label
    private PatchLabel _maxTowersLabel;

    public int MaxAllowedTowers { get; set; } = 20;

    /// <summary>
    /// Creates a new <see cref="TowerGraftingGUI"/>
    /// </summary>
    public TowerGraftingGUI()
    {
        // Create Night Button
        _nightButton = PatchButton.MakeBase(new Vector2(Interface.ScreenSize.X - _towerButtonSize.X, Interface.ScreenSize.Y - _towerButtonSize.Y - _createdTowerSize.Y), _towerButtonSize, "Begin");

        // Tower Display
        _towerDisplay = PatchLabel.MakeBaseCentered(
            text: "",
            position: Interface.ScreenCenter - _towerDisplayOffset,
            size: _towerDisplaySize
            );

        // Current Chosen Label
        _currentChosenLabel = PatchLabel.MakeBase(
            "Current: Nothing",
            _towerDisplay.Position + (_towerDisplaySize - _currentChosenLabelSize) * Vector2.UnitY,
            _currentChosenLabelSize);

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

        // Create Max Towers Label
        _maxTowersLabel = PatchLabel.MakeBase(
            text: $"Towers: 0 / {MaxAllowedTowers}",
            new Vector2(_saveButton.Position.X + _saveButtonSize.X, _saveButton.Position.Y),
            _maxTowersLabelSize
            );
    }

    public void Setup(Inventory inventory)
    {
        _projectiles = new ProjectileManager();

        _towerChoiceButtons = new ScrollingGrid<PatchButton>(
            orientation: Orientation.Vertical,
            gridPosition: new Vector2(Interface.Width - _towerButtonSize.X, 0) + _towerSelectorPositionOffset,
            gridSize: new Vector2(_towerButtonSize.X, Interface.Height - _createdTowerSize.Y - _towerSelectorPositionOffset.Y),
            elementSize: _towerButtonSize,
            arrowSide: Corner.TopOrRight,
            arrowOffset: 0.0f,
            subgridSide: Corner.BottomOrLeft
        );
        _towerChoices = [];

        _partChoiceButtons = new ScrollingGrid<PatchButton>(
            orientation: Orientation.Vertical,
            gridPosition: _partSelectorPosition,
            gridSize: new Vector2(_partButtonSize.X * 2.0f, Interface.Height - _createdTowerSize.Y - _partSelectorPosition.Y) - _partSelectorSizeOffset,
            elementSize: _partButtonSize,
            arrowSide: Corner.BottomOrLeft,
            arrowOffset: 0.0f,
            subgridSide: Corner.TopOrRight
        );
        _partChoices = [];

        _currentlyGraftingTower = null;
        _currentlyChosenPart = null;
        _editingTower = null;

        _createdTowers = new ScrollingGrid<CreatedTowerButton>(
            orientation: Orientation.Horizontal,
            gridPosition: new Vector2(0, Interface.Height - _createdTowerSize.Y),
            gridSize: new Vector2(Interface.Width, _createdTowerSize.Y),
            elementSize: _createdTowerSize,
            arrowSide: Corner.TopOrRight,
            arrowOffset: _arrowCreatedTowerButtonOffset,
            subgridSide: Corner.TopOrRight
        );

        List<PartDefinition> availableParts = inventory.GetCollectedParts();
        List<TowerDefinition> availableTowers = TowerRegistry.Towers.ToList();

        // Populate Towers
        for (int index = 0; index < availableTowers.Count; index++)
        {
            TowerDefinition towerDefinition = availableTowers[index];
            _towerChoices.Add(towerDefinition);
            _towerChoiceButtons.Add(
                PatchButton.MakeBase(
                    Vector2.One, Vector2.One,
                    towerDefinition.Name
                    ));
        }

        // Populate Parts
        for (int index = 0; index < availableParts.Count; index++)
        {
            PartDefinition partDefinition = availableParts[index];
            _partChoices.Add(partDefinition);
            _partChoiceButtons.Add(
                PatchButton.MakeBase(
                    Vector2.One, Vector2.One,
                    icon: partDefinition.Texture
                    ));
        }
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

        // Update Part Attachment
        if (inputManager.LeftMouseClicked() &&
            _currentlyChosenPart is not null &&
            _editingTower is not null &&
            _editingTower.IsOver(Vector2.Transform(inputManager.MouseScreenPosition.ToVector(), Matrix.CreateScale(1.0f / _previewScale))) &&
            !_editingTower.PartsFull)
        {
            if (world.Inventory.GetPartCount(_currentlyChosenPart) > 0)
            {
                _editingTower.AttachPart(_currentlyChosenPart);
                world.Inventory.ModifyPartCount(_currentlyChosenPart, -1);
            }
        }

        // Update Towers Selection
        bool mouseOverTowers = _towerChoiceButtons.IsMouseOver;
        _towerChoiceButtons.Update(time,
            inputManager, (button, index) =>
            {
                button.Update(inputManager);
                if (button.JustClicked && mouseOverTowers)
                {
                    // Create the new Tower
                    if (_editingTower is not null)
                    {
                        SaveCurrentTower();
                    }
                    _currentlyGraftingTower = _towerChoices[index];
                    _editingTower = _currentlyGraftingTower.Factory((Interface.ScreenCenter - _towerDisplayOffset) / _previewScale);
                }
            });

        // Update Parts Selection
        bool mouseOverParts = _partChoiceButtons.IsMouseOver;
        _partChoiceButtons.Update(time,
            inputManager, (button, index) =>
            {
                button.Text = $"{world.Inventory.GetPartCount(_partChoices[index])}";

                button.Update(inputManager);
                if (button.JustClicked && mouseOverParts)
                {
                    // Select the part
                    _currentlyChosenPart = _partChoices[index];
                    _currentChosenLabel.Text = "Current: " + (_currentlyChosenPart is not null ? _currentlyChosenPart.Name : "Nothing");
                }
            });

        // Update Max Towers Label
        _maxTowersLabel.Text = $"Towers: {_createdTowers.Elements.Count} / {MaxAllowedTowers}";
        // Move Max Towers Text Color towards white
        _maxTowersLabel.TextColor = _maxTowersLabel.TextColor with
        {
          R = (byte)MathHelper.Clamp(_maxTowersLabel.TextColor.R + (int)(time.Delta() * 256), 0, 255),
          G = (byte)MathHelper.Clamp(_maxTowersLabel.TextColor.G + (int)(time.Delta() * 256), 0, 255),
          B = (byte)MathHelper.Clamp(_maxTowersLabel.TextColor.B + (int)(time.Delta() * 256), 0, 255)
        };

        // Update Save Button
        _saveButton.Update(inputManager);

        // Handle Saving
        if (_saveButton.JustClicked)
        {
            SaveCurrentTower(button: true);
        }

        // Update Remove Part Button
        _removePartButton.Update(inputManager);

        // Handle Part Removal
        if (_removePartButton.JustClicked && _editingTower is not null && _editingTower.HasParts)
        {
            PartDefinition removed = _editingTower.RemovePart();
            world.Inventory.ModifyPartCount(removed, 1);
        }

        // Update Created Towers
        _createdTowers.Update(time, inputManager, (created, _) => created.Update(time, world, inputManager));

        // Handle Selecting Created Towers
        for (int index = 0; index < _createdTowers.Elements.Count; index++)
        {
            CreatedTowerButton button = _createdTowers.Elements[index];
            
            if (button.Internal.JustClicked)
            {
                if (_editingTower is not null)
                {
                    // Refund the parts of the CURRENTLY editing Tower
                    SaveCurrentTower();
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

        // Update Night Button
        _nightButton.Update(inputManager);
        if (_nightButton.JustClicked)
        {
            SaveCurrentTower();
            // Save Designs
            world.Inventory.StartingDesigns.AddRange(
                _createdTowers.Elements.Select(
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

    public void SaveCurrentTower(bool button = false)
    {
        if (_editingTower is not null && _editingTower.HasParts)
        {
            // Allow saving if there is space
            if (_createdTowers.Elements.Count < MaxAllowedTowers)
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
            else if (button)
            {
                // Set the non red channels of the Max Towers Label's Color to zero
                _maxTowersLabel.TextColor = _maxTowersLabel.TextColor with { G = 0, B = 0 };
            }
        }
    }

    /// <summary>
    /// Draws the <see cref="TowerGraftingGUI"/> Interface
    /// </summary>
    /// <param name="drawing"><see cref="SpriteBatch"/> to use</param>
    /// <param name="time">Game Time</param>
    public void Draw(DrawManager drawing, GameTime time, World world, InputManager inputManager)
    {
        // Draw Garage Background
        drawing.Draw(
            Placeholders.GarageBackgroundTexture,
            destination: Interface.ScreenRect,
            isUi: true,
            sortMode: SortMode.Top,
            drawLayer: 0
        );

        // Scissor for Parts
        //ReBatchWithScissor(drawing, _towerChoiceButtons.GridRectangle);
        // Draw each Tower Button
        _towerChoiceButtons.Draw(drawing, (batch, button, _) => button.Draw(batch));

        // Scissor for Parts
        //ReBatchWithScissor(drawing, _partChoiceButtons.GridRectangle);
        // Draw each Part Button
        _partChoiceButtons.Draw(drawing, (batch, button, _) => button.Draw(batch));

        // Back to Normal
        //ReBatchNormal(drawing);

        // Draw Non-Button parts of Grids
        _towerChoiceButtons.Draw(drawing, skipBackground: true);
        _partChoiceButtons.Draw(drawing, skipBackground: true);

        // Draw Night Button
        _nightButton.Draw(drawing);
        // Draw Tower Display Border
        _towerDisplay.Draw(drawing);

        // Draw Created Towers
        _createdTowers.Draw(drawing, (batch, created, _) => created.Draw(batch, time, world, inputManager));

        // Draw Save Button
        _saveButton.Draw(drawing);

        // Draw Remove Part Button
        _removePartButton.Draw(drawing);

        // Draw Tower Count Label
        _maxTowersLabel.Draw(drawing);

        // Custom Tower Drawing

        // Set Scissor Mask
        drawing.ForceScissor = _towerDisplay.Box;

        drawing.ExtraMatrix = Matrix.CreateScale(_previewScale);

        // Draw Tower
        _editingTower?.Draw(time, drawing, world, inputManager, TimeState.Day, isUi: true);

        // Draw Projectiles
        _projectiles.Draw(drawing, time, world, inputManager, isUi: true);

        // Back to Normal
        drawing.ExtraMatrix = null;
        drawing.ForceScissor = null;

        // Draw Text
        if (_currentlyChosenPart is not null)
        {
            drawing.DrawString("Attach Parts Here", font: Fonts.SubFont, position: Interface.ScreenCenter - new Vector2(0, 420f), isUi: true, centered: true);
        }
        drawing.DrawString("Parts", font: Fonts.SubFont, position: new Vector2(8, 8), isUi: true);
        drawing.DrawString("Towers", font: Fonts.SubFont, position: Interface.TopRight + new Vector2(-8, 8) - Fonts.SubFont.MeasureString("Towers") * Vector2.UnitX, isUi: true);
        if (_createdTowers.Elements.Count > 0)
        {
            drawing.DrawString("Saved Towers - Click to Edit", font: Fonts.SubFont, position: Interface.BottomLeft - new Vector2(0, _createdTowerSize.Y + Fonts.SubFont.MeasureString("Saved Towers - Click to Edit").Y), isUi: true);
        }

        // Draw the Label showing the Currently Chosen Part
        _currentChosenLabel.Draw(drawing);

        // Draw the preview of the chosen part
        if (_currentlyChosenPart is not null)
        {
            drawing.DrawCentered(_currentlyChosenPart.Texture, inputManager.CurrentMouse.Position.ToVector(), color: new Color(1.0f, 1.0f, 1.0f, 0.7f), isUi: true, scale: Vector2.One * 3);
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

    /// <summary>
    /// Ends current and Starts a new Batch Begin, with Scissor testing and the given <paramref name="scissor"/> <see cref="Rectangle"/>
    /// </summary>
    /// <param name="batch"><see cref="SpriteBatch"/> to use</param>
    /// <param name="scissor"><see cref="Rectangle"/> for scissoring</param>
    public void ReBatchWithScissorDEPRICATED(SpriteBatch batch, Rectangle scissor)
    {
        batch.End();
        batch.GraphicsDevice.ScissorRectangle = scissor;
        batch.Begin(samplerState: SamplerState.PointWrap, rasterizerState: new RasterizerState() { ScissorTestEnable = true });
    }

    /// <summary>
    /// Ends current and Stars a new Batch Begin with default static settings
    /// </summary>
    /// <param name="batch"></param>
    public void ReBatchNormalDEPRICATED(SpriteBatch batch)
    {
        batch.End();
        batch.Begin(samplerState: SamplerState.PointWrap);
    }
}
