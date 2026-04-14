using GraftGuard.Data;
using GraftGuard.Grafting.Registry;
using GraftGuard.Grafting.Registry.Behaviors;
using GraftGuard.Grafting.Towers;
using GraftGuard.Graphics;
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
internal class AttachedPart
{
    private IPartBehavior[] _partBehaviors;
    public PartDefinition Definition { get; init; }
    public ReadOnlySpan<IPartBehavior> Behaviors => _partBehaviors;
    public string? SlotName;
    public AttachedPart(PartDefinition definition, string? slotName = null)
    {
        Definition = definition;

        _partBehaviors = definition.PartBehaviorNames.Select((name) => PartBehaviorRegistry.GetFromName(name).Create()).ToArray();
        SlotName = slotName;
    }

    public void BehaviorOnDealDamage(
        float damageModifier, PartSettings settings, PartTransform transform, GameTime time, World world, InputManager inputManager,
        TimeState state, ProjectileManager projectileManager)
    {
        foreach (IPartBehavior behavior in _partBehaviors)
        {
            behavior.OnDealDamage(settings, damageModifier, Definition, transform, time, world, inputManager, state, projectileManager);
        }
    }

    public void UpdateBehavior(PartSettings settings, PartTransform transform, GameTime time, World world, InputManager inputManager,
        TimeState state, ProjectileManager projectileManager)
    {
        foreach (IPartBehavior behavior in _partBehaviors)
        {
            behavior.Update(settings, Definition, transform, time, world, inputManager, state, projectileManager);
        }
    }

    public void DrawBehavior(PartSettings settings, PartTransform transform, GameTime time, DrawManager drawing, World world, InputManager inputManager, TimeState state, bool isUi = false)
    {
        foreach (IPartBehavior behavior in _partBehaviors)
        {
            behavior.Draw(settings, Definition, transform, time, drawing, world, inputManager, state);
        }
    }
}
