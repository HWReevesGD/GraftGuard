using GraftGuard.Grafting.Registry.Behaviors;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GraftGuard.Grafting.Registry;
internal static class PartBehaviorRegistry
{
    public const string ProjectContent = "../../../Content";
    public const string SavePath = "part_behaviors.json";
    private static List<PartBehaviorDefinition> _allBehaviors = [];

    public static void Register(string name, CreatePartBehavior creatorFactory)
    {
        _allBehaviors.Add(new PartBehaviorDefinition(name, creatorFactory));
    }

    public static void Save(ContentManager content)
    {
        string[] names = _allBehaviors.Select((behavior) => behavior.Name).ToArray();
        string json = JsonSerializer.Serialize(names);
        File.WriteAllText(Path.Join(ProjectContent, SavePath), json);
        System.Diagnostics.Debug.WriteLine("Saved Part Behaviors at: " + Path.Join(ProjectContent, SavePath));
    }

    public static PartBehaviorDefinition GetFromName(string name)
    {
        return _allBehaviors.First((behavior) => behavior.Name == name);
    }
}
