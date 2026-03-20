using GraftGuard.Grafting.Registry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraftGuard.Grafting
{
    public class GraftLibrary
    {
        public List<PartDefinition> Parts { get; set; } = new();
        public List<BaseDefinition> Bases { get; set; } = new();
    }

}
