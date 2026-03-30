using GraftGuard.Data;
using GraftGuard.Grafting.Registry;
using GraftGuard.Grafting.Towers;
using GraftGuard.Map;
using GraftGuard.Map.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
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
        Internal.Update();
        Tower.Update(time, world, input, state, Projectiles);
        Projectiles.Update(time, world, input);
    }
    public void Draw(SpriteBatch batch, GameTime time, World world, InputManager input, TimeState state = TimeState.Day)
    {
        Internal.Draw(batch);

        // Clipping
        batch.End();
        batch.GraphicsDevice.ScissorRectangle = Internal.MarginBox;
        batch.Begin(samplerState: SamplerState.PointWrap, rasterizerState: new RasterizerState() { ScissorTestEnable = true });

        Tower.Draw(time, batch, world, input, state);
        Projectiles.Draw(batch, time, world, input);

        batch.End();
        batch.Begin(samplerState: SamplerState.PointWrap);
    }
}
