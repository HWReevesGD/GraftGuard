using GraftGuard.Data;
using GraftGuard.Grafting.Registry;
using GraftGuard.Grafting.Towers;
using GraftGuard.Graphics;
using GraftGuard.Map;
using GraftGuard.Map.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraftGuard.UI;
internal class CreatedTowerButton : IPositional, ISizeable
{
    public PatchButton Internal;
    public Tower Tower;
    public TowerDefinition Definition;
    public ProjectileManager Projectiles;
    public bool JustClicked => Internal.JustClicked;
    public Vector2 Position { get => Internal.Position; set => Internal.Position = value; }
    public Vector2 Size { get => Internal.Size; set => Internal.Size = value; }
    public CreatedTowerButton(Tower tower, TowerDefinition definition, Vector2 position, Vector2 size)
    {
        Internal = PatchButton.MakeBase(position, size);
        Tower = tower;
        Definition = definition;
        tower.Position = position + size * 0.5f;
        Projectiles = new ProjectileManager();
    }
    public void Update(GameTime time, World world, InputManager input, TimeState state = TimeState.Day)
    {
        Tower.Position = Internal.Position + Internal.Size * 0.5f;
        Internal.Update(input);
        Tower.Update(time, world, input, state, Projectiles);
        Projectiles.Update(time, world, input);
    }
    public void Draw(DrawManager drawing, GameTime time, World world, InputManager input, TimeState state = TimeState.Day)
    {
        Internal.Draw(drawing);

        // Clipping
        drawing.ForceScissor = Internal.MarginBox;

        Tower.Draw(time, drawing, world, input, state, isUi: true, drawLayerOffset: 1);
        Projectiles.Draw(drawing, time, world, input, isUi: true);

        drawing.ForceScissor = null;
    }
}
