using GraftGuard.Grafting.Registry.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GraftGuard.Grafting.Registry;
internal static class PartBehaviorRegistry
{
    private static List<PartBehaviorDefinition> _allBehaviors = [];

    public static void Register(string name, CreatePartBehavior creatorFactory)
    {
        _allBehaviors.Add(new PartBehaviorDefinition(name, creatorFactory));
    }

    public static void Save()
    {
        //

        //System.Diagnostics.Debug.WriteLine(json);
    }
}
