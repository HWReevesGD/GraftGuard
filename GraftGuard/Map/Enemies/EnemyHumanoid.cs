using GraftGuard.Grafting;
using GraftGuard.Grafting.Registry;
using GraftGuard.Graphics;
using GraftGuard.Map.Pathing;
using GraftGuard.UI;
using GraftGuard.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using static GraftGuard.Map.Pathing.PathManager;

namespace GraftGuard.Map.Enemies;
internal class EnemyHumanoid : EnemyBasic
{
    public EnemyHumanoid(Vector2 position) : this(position, HumanoidEnemyRegistry.GetRandomSet()) { }

    private EnemyHumanoid(Vector2 position, (BaseDefinition Torso, int Type) set) : base(position, set.Torso)
    {
        Random random = new Random();
        List<PartDefinition> humanoidParts = HumanoidEnemyRegistry.GetPartsForType(set.Type);

        Visual.AttachedParts.Clear();

        float limbRandomChance = 0.5f; // 25% chance for a random arm/leg
        float headRandomChance = 0.2f; // 5% chance for a random head

        foreach (var standardPart in humanoidParts)
        {
            PartDefinition partToAttach = standardPart;
            bool isHead = standardPart.Name.Contains("Head");

            // Roll for randomization
            float roll = (float)random.NextDouble();
            float chance = isHead ? headRandomChance : limbRandomChance;
            bool fromLibrary = false;

            if (roll < chance)
            {
                fromLibrary = true;
                if(isHead) GraftLibrary.GetRandomHead();
                else partToAttach = GraftLibrary.GetRandomPart();
            }

            Visual.AttachedParts.Add(new AttachedPart(partToAttach, standardPart.Name, fromLibrary));
        }

    }
}
