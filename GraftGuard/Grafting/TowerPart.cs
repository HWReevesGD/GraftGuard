using GraftGuard.Data;
using GraftGuard.Grafting.Registry;
using GraftGuard.Grafting.Registry.Behaviors;
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

namespace GraftGuard.Grafting;
internal class TowerPart
{
    private IPartBehavior[] _partBehaviors;
    public PartDefinition Definition { get; init; }
    public ReadOnlySpan<IPartBehavior> Behaviors => _partBehaviors;
    public TowerPart(PartDefinition definition)
    {
        Definition = definition;

        _partBehaviors = definition.PartBehaviorNames.Select((name) => PartBehaviorRegistry.GetFromName(name).Create()).ToArray();
    }

    public void BehaviorOnDealDamage(
        float damageModifier, PartSettings settings, PartDefinition part, Vector2 partPosition, float partRotation, GameTime time,
        World world, InputManager inputManager, TimeState state, ProjectileManager projectileManager)
    {
        foreach (IPartBehavior behavior in _partBehaviors)
        {
            behavior.OnDealDamage(settings, damageModifier, part, partPosition, partRotation, time, world, inputManager, state, projectileManager);
        }
    }

    public void UpdateBehavior(PartSettings settings, PartDefinition part, Vector2 partPosition, float partRotation, GameTime time,
        World world, InputManager inputManager, TimeState state, ProjectileManager projectileManager)
    {
        foreach (IPartBehavior behavior in _partBehaviors)
        {
            behavior.Update(settings, part, partPosition, partRotation, time, world, inputManager, state, projectileManager);
        }
    }

    public void DrawBehavior(PartSettings settings, PartDefinition part, Vector2 partPosition, float partRotation, GameTime time, SpriteBatch batch, World world, InputManager inputManager, TimeState state)
    {
        foreach (IPartBehavior behavior in _partBehaviors)
        {
            behavior.Draw(settings, part, partPosition, partRotation, time, batch, world, inputManager, state);
        }
    }
}
