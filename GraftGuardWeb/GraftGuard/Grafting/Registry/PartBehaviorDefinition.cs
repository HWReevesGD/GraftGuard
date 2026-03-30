using GraftGuard.Grafting.Registry.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraftGuard.Grafting.Registry;
internal class PartBehaviorDefinition
{
    public string Name { get; init; }
    public CreatePartBehavior Create { get; init; }

    public PartBehaviorDefinition(string name, CreatePartBehavior creatorFactory)
    {
        Name = name;
        Create = creatorFactory;
    }
}
