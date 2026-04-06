using GraftGuard.Data;
using GraftGuard.Grafting.Towers;
using GraftGuard.Map;
using GraftGuard.Map.Enemies;
using GraftGuard.Map.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraftGuard.Grafting.Registry.Behaviors;
internal delegate IPartBehavior CreatePartBehavior();
internal interface IPartBehavior
{
    public void Update(PartSettings settings, PartDefinition part, Vector2 partPosition, float partRotation, GameTime time, World world, InputManager inputManager, TimeState state, ProjectileManager projectileManager);
    public void Draw(PartSettings settings, PartDefinition part, Vector2 partPosition, float partRotation, GameTime time, SpriteBatch batch, World world, InputManager inputManager, TimeState state);
    public void OnDealDamage(PartSettings settings, float damageModifier, PartDefinition part, Vector2 partPosition, float partRotation, GameTime time, World world, InputManager inputManager, TimeState state, ProjectileManager projectileManager);
}
