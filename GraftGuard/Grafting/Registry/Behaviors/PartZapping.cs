using GraftGuard.Data;
using GraftGuard.Map;
using GraftGuard.Map.Enemies;
using GraftGuard.Map.Projectiles;
using GraftGuard.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraftGuard.Grafting.Registry.Behaviors;
internal class PartZapping : IPartBehavior
{
    public IntervalTimer ZapInterval = new IntervalTimer(interval: 1.0f);
    public const float ZapSearchRadius = 96.0f;
    public const int MaxZaps = 2;
    public const int MaxChain = 3;
    public static IPartBehavior Create() => new PartZapping();
    public void Draw(PartSettings settings, PartDefinition part, PartTransform transform, GameTime time, SpriteBatch batch, World world, InputManager inputManager, TimeState state)
    {
        
    }

    public void OnDealDamage(PartSettings settings, float damageModifier, PartDefinition part, PartTransform transform, GameTime time, World world, InputManager inputManager, TimeState state, ProjectileManager projectileManager)
    {
        
    }

    public void Update(PartSettings settings, PartDefinition part, PartTransform transform, GameTime time, World world, InputManager inputManager, TimeState state, ProjectileManager projectileManager)
    {

        bool doZap = ZapInterval.Update(time);

        if (doZap)
        {
            int chain = 0;
            List<GameObject> allZapped = [];
            List<ProjectileZap> currentZaps = [];
            List<ProjectileZap> allZaps = [];
            while (chain < MaxChain)
            {
                List<ProjectileZap> zaps;

                if (chain == 0)
                {
                    (currentZaps, allZapped) = DoZap(
                        allZapped,
                        settings,
                        transform,
                        world);
                    allZaps.AddRange(currentZaps);
                    chain++;
                    continue;
                }

                foreach (ProjectileZap zap in currentZaps)
                {
                    List<GameObject> zapped;
                    (zaps, zapped) = DoZap(
                        allZapped,
                        settings,
                        transform,
                        world,
                        zap);
                    allZaps.AddRange(zaps);

                    allZapped.AddRange(zapped);
                }

                projectileManager.AddAll(allZaps);
                chain++;
            }
        }

    }

    public (List<ProjectileZap>, List<GameObject>) DoZap(List<GameObject> previousZapped, PartSettings settings, PartTransform transform, World world, ProjectileZap zapSource = null)
    {
        List<GameObject> zapped = [];
        List<ProjectileZap> zaps = [];

        switch (settings.Source)
        {
            case Source.Player:

                Enemy next = FindEnemy(transform.Position, world, previousZapped);
                while (next is not null && zapped.Count < MaxZaps)
                {
                    zapped.Add(next);
                    ProjectileZap zap = new ProjectileZap(zapSource?.Position ?? transform.Position, ProjectileTarget.Enemy);

                    List<GameObject> allZapped = zapped;
                    allZapped.AddRange(previousZapped);
                    next = FindEnemy(transform.Position, world, allZapped);

                    zapSource?.Next.Add(zap);
                    zaps.Add(zap);
                }

                break;
            case Source.Enemy:
                return ([], []);
                // TODO: Implement Enemy Part
        }

        return (zaps, zapped);
    }

    public Enemy? FindEnemy(Vector2 position, World world, List<GameObject> exclude = null)
    {
        Enemy chosen = null;
        float chosenDistance = float.PositiveInfinity;
        float maxDistance = ZapSearchRadius * ZapSearchRadius;
        foreach (Enemy enemy in world.EnemyManager.Enemies)
        {
            if (exclude?.Any((zappedEnemy) => ReferenceEquals(zappedEnemy, enemy)) is true)
            {
                continue;
            }
            float distance = Vector2.DistanceSquared(enemy.Position, position);
            if (distance <= maxDistance && distance < chosenDistance)
            {
                chosenDistance = distance;
                chosen = enemy;
            }
        }
        return chosen;
    }
}
