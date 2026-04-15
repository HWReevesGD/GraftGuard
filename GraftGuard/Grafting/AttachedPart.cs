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

    public bool IsCollectable => collectible;

    public string? SlotName;

    private bool collectible;
    public AttachedPart(PartDefinition definition, string? slotName = null, bool collectible = true)
    {
        Definition = definition;
        SlotName = slotName;
        this.collectible = collectible;

        // If PartBehaviorNames is null, it defaults to an empty array before the Select runs
        _partBehaviors = (definition.PartBehaviorNames ?? Array.Empty<string>())
            .Select(name => PartBehaviorRegistry.GetFromName(name).Create())
            .ToArray();
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
