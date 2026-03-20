using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraftGuard.Grafting.Registry
{
    //TEMPORARY
    // Defines where things can be attached to a specific Torso
    public class BaseDefinition
    {
        public string Name { get; set; } = "New Base";
        public string TextureName { get; set; } = "";
        public Texture2D Texture { get; set; }
        public bool IsTorso { get; set; } // true = Torso, false = Tower Base

        public List<AttachPoint> AttachPoints { get; set; }

        public Dictionary<string, Vector2> AttachmentPoints { get; set; }

        public BaseDefinition(string name, string textureName, Texture2D texture, bool isTorso, List<AttachPoint> attachPoints) 
        {
            Name = name;
            TextureName = textureName;
            Texture = texture;
            IsTorso = isTorso;
            AttachmentPoints = attachPoints.ToDictionary(
                point => point.Name,
                point => new Vector2(point.PivotX, point.PivotY));
           
        }
        

        // This makes the name show up correctly in the ListBox
        public override string ToString() => $"{Name}";
    }

    public struct AttachPoint
    {
        public string Name;
        public float PivotX { get; set; }
        public float PivotY { get; set; }
    }
}
