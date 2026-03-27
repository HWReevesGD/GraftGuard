using GraftGuard.Data;
using GraftGuard.Grafting.Towers;
using GraftGuard.Map;
using GraftGuard.Map.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraftGuard.UI;
internal class CreatedTowerButton
{
    public PatchButton Internal;
    public Tower Tower;
    public ProjectileManager Projectiles;
    public bool JustClicked => Internal.JustClicked;
    public CreatedTowerButton(Tower tower, Vector2 position, Vector2 size)
    {
        Internal = PatchButton.MakeBase(position, size);
        Tower = tower;
        Projectiles = new ProjectileManager();
    }
    public void Update(GameTime time, World world, InputManager input, TimeState state = TimeState.Day)
    {
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
