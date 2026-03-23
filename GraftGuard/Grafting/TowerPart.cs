using GraftGuard.Grafting.Registry;
using GraftGuard.Grafting.Registry.Behaviors;
using GraftGuard.Grafting.Towers;
using GraftGuard.Map;
using GraftGuard.Map.Enemies;
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

    public void BehaviorOnDealDamage(float damageModifier, Tower tower, PartDefinition part, Vector2 partPosition, GameTime time, World world, InputManager inputManager, TimeState state)
    {
        foreach (IPartBehavior behavior in _partBehaviors)
        {
            behavior.OnDealDamage(damageModifier, tower, part, partPosition, time, world, inputManager, state);
        }
    }

    public void UpdateBehavior(Tower tower, PartDefinition part, Vector2 partPosition, GameTime time, World world, InputManager inputManager, TimeState state)
    {
        foreach (IPartBehavior behavior in _partBehaviors)
        {
            behavior.Update(tower, part, partPosition, time, world, inputManager, state);
        }
    }

    public void DrawBehavior(Tower tower, PartDefinition part, Vector2 partPosition, GameTime time, SpriteBatch batch, World world, InputManager inputManager, TimeState state)
    {
        foreach (IPartBehavior behavior in _partBehaviors)
        {
            behavior.Draw(tower, part, partPosition, time, batch, world, inputManager, state);
        }
    }
}
