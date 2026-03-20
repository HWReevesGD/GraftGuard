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
        public List<BaseData> Bases { get; set; } = new();
    }

    public class BaseData
    {
        public string Name { get; set; } = "New Base";
        public string Id { get; set; } = "id_0";
        public string TextureName { get; set; } = "";
        public bool IsTorso { get; set; }
        public string FullImagePath { get; set; } = "";
        public override string ToString() => $"{Name} ({Id})";
    }
}
