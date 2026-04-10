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
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GraftGuard.UI;
internal class NightPlacementGUI
{
    private readonly Vector2 _designSize = new Vector2(72, 72);
    private const float _arrowButtonOffset = -256;

    private ScrollingGrid<CreatedTowerButton> _designButtons;
    private List<TowerDesign> _designs = [];
    private int? _selectedIndex = null;

    /// <summary>
    /// Pulls the designs from the given <paramref name="inventory"/> to populate <see cref="Tower"/>s
    /// </summary>
    /// <param name="inventory"></param>
    public void Setup(Inventory inventory)
    {
        _designButtons = new ScrollingGrid<CreatedTowerButton>(
            orientation: Orientation.Horizontal,
            gridPosition: new Vector2(0, Interface.Height - _designSize.Y),
            gridSize: new Vector2(Interface.Width, _designSize.Y),
            elementSize: _designSize,
            arrowSide: Corner.TopOrRight,
            arrowOffset: _arrowButtonOffset,
            subgridSide: Corner.TopOrRight
            );
        _designs = [];
        _selectedIndex = null;

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
        _designButtons.Update(time,
            input, (button, index) =>
            {
                button.Update(time, world, input);

                // Handle Selection
                if (button.JustClicked)
                {
                    _selectedIndex = index;
                }
            });

        // Handle Placement
        bool isOver = _designButtons.Elements.Any((button) => button.Internal.IsMouseHovered);
        if (!isOver && _selectedIndex is int designIndex)
        {
            if (input.LeftMouseClicked())
            {
                _selectedIndex = null;
                TowerDesign design = _designs[designIndex];

                _designButtons.RemoveAt(designIndex);
                _designs.RemoveAt(designIndex);
                world.Inventory.StartingDesigns.RemoveAt(designIndex);

                world.TowerManager.MakeTower(
                    design.Definition,
                    input.MouseWorldPosition.ToVector(),
                    parts: design.Parts
                    );
            }
        }
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
        _designButtons.Draw(batch, (batch, button, _) => button.Draw(batch, time, world, input));

        // Draw the a preview of the current tower
        if (_selectedIndex is int index)
        {
            _designs[index].Definition.DrawPreview(batch, time, input.MouseScreenPosition.ToVector());
        }
    }
}
