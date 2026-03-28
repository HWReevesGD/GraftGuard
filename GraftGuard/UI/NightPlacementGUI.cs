using GraftGuard.Grafting;
using GraftGuard.Grafting.Registry;
using GraftGuard.Grafting.Towers;
using GraftGuard.Map;
using GraftGuard.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraftGuard.UI;
internal class NightPlacementGUI
{
    private readonly Vector2 _designSize = new Vector2(72, 72);
    private readonly Vector2 _arrowButtonSize = new Vector2(64, 32);
    private readonly Vector2 _arrowButtonOffset = new Vector2(-256, 0);
    private const float _arrowScrollSpeed = 256.0f;

    private List<CreatedTowerButton> _designButtons = [];
    private List<TowerDesign> _designs = [];
    private int? _selectedIndex = null;

    private bool _showArrows = false;
    private PatchButton _leftButton;
    private PatchButton _rightButton;
    private float _designsOffset = 0.0f;

    public NightPlacementGUI()
    {
        // Setup Arrows
        _leftButton = PatchButton.MakeBase(
            new Vector2(Interface.ScreenCenter.X - _arrowButtonSize.X, Interface.ScreenSize.Y - _designSize.Y - _arrowButtonSize.Y) + _arrowButtonOffset,
            _arrowButtonSize,
            text: @"<"
            );
        _rightButton = PatchButton.MakeBase(
            new Vector2(Interface.ScreenCenter.X, Interface.ScreenSize.Y - _designSize.Y - _arrowButtonSize.Y) + _arrowButtonOffset,
            _arrowButtonSize,
            text: @">"
            );
    }

    /// <summary>
    /// Pulls the designs from the given <paramref name="inventory"/> to populate <see cref="Tower"/>s
    /// </summary>
    /// <param name="inventory"></param>
    public void Setup(Inventory inventory)
    {
        _designButtons = [];
        _designs = [];
        _selectedIndex = null;
        _showArrows = false;
        _designsOffset = 0.0f;

        // Setup Designs
        foreach (TowerDesign design in inventory.StartingDesigns)
        {
            _designs.Add(design);
            Tower tower = design.Definition.Factory(Vector2.Zero);
            // Attach all design parts
            foreach (PartDefinition part in design.Parts)
            {
                // Skip null Parts
                if (part is null)
                {
                    break;
                }

                tower.AttachPart(part);
            }
            _designButtons.Add(
                new CreatedTowerButton(
                    tower,
                    design.Definition,
                    Vector2.Zero,
                    _designSize
                    ));
        }
    }
    public void Update(GameTime time, World world, InputManager input)
    {
        // Update Designs
        for (int index = 0; index < _designButtons.Count; index++)
        {
            CreatedTowerButton button = _designButtons[index];
            button.Update(time, world, input);
            button.Internal.Position = new Vector2(_designSize.X * index - _designsOffset, Interface.Height - _designSize.Y);

            // Handle Selection
            if (button.JustClicked)
            {
                _selectedIndex = index;
            }
        }

        // Handle Placement
        bool isOver = _designButtons.Any((button) => button.Internal.IsMouseHovered);
        if (!isOver && _selectedIndex is int designIndex)
        {
            if (input.LeftMouseClicked())
            {
                _selectedIndex = null;
                TowerDesign design = _designs[designIndex];

                _designButtons.RemoveAt(designIndex);
                _designs.RemoveAt(designIndex);

                world.TowerManager.MakeTower(
                    design.Definition,
                    input.MouseWorldPosition.ToVector(),
                    parts: design.Parts
                    );
            }
        }

        // Calculate how much "beyond" the screen the design towers go
        float createdBeyond = MathF.Max(0.0f, _designSize.X * _designButtons.Count - Interface.Width);
        _showArrows = createdBeyond > 0.0f;

        // Update Arrow Buttons (If there are enough created towers)
        if (_showArrows)
        {
            _leftButton.Update();
            _rightButton.Update();

            float delta = time.Delta();

            if (_leftButton.IsPressed)
            {
                _designsOffset -= delta * _arrowScrollSpeed;
            }
            if (_rightButton.IsPressed)
            {
                _designsOffset += delta * _arrowScrollSpeed;
            }
        }
        // Clamp Created Towers Offset
        _designsOffset = MathHelper.Clamp(_designsOffset, 0.0f, createdBeyond);
    }
    public void Draw(SpriteBatch batch, GameTime time, World world, InputManager input)
    {
        // Draw Buttons Background
        batch.Draw(
            Placeholders.TexturePixel,
            new Rectangle(0, (int)(Interface.ScreenSize.Y - _designSize.Y),
                (int)Interface.ScreenSize.X, (int)_designSize.X),
            new Color(0.0f, 0.0f, 0.0f, 0.4f));

        // Draw Designs
        foreach (CreatedTowerButton button in _designButtons)
        {
            button.Draw(batch, time, world, input);
        }

        // Draw Arrows
        if (_showArrows)
        {
            _leftButton.Draw(batch);
            _rightButton.Draw(batch);
        }

        // Draw the a preview of the current tower
        if (_selectedIndex is int index)
        {
            _designs[index].Definition.DrawPreview(batch, time, input.MouseScreenPosition.ToVector());
        }
    }
}
