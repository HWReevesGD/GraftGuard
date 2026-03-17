using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnemyTest
{
    public static class PartLoader
    {
        public static Dictionary<string, PartDefinition> LoadParts(string filePath, ContentManager content)
        {
            var partsDict = new Dictionary<string, PartDefinition>();

            Debug.WriteLine("Trying to find file/");
            Debug.WriteLine("Working Directory: " + Directory.GetCurrentDirectory());
            if (!File.Exists($"../../../{filePath}")) return partsDict;

            Console.WriteLine("File Exists!");

            string[] lines = File.ReadAllLines($"../../../{filePath}");

            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                try
                {
                    // Split the CSV
                    string[] data = line.Split(',');

                    // Map indices to PartDefinition
                    string name = data[0];
                    string texName = data[7]; 
                    float px = float.Parse(data[8]);
                    float py = float.Parse(data[9]);

                    // Load the texture from MonoGame Content Pipeline
                    // Assuming textures are in a "Parts" subfolder
                    Texture2D texture = content.Load<Texture2D>($"{texName}");

                    var part = new PartDefinition(name, texture, new Vector2(px, py))
                    {
                        BaseDamage = float.Parse(data[1]),
                        SpeedModifier = float.Parse(data[2]),
                        ArmorModifier = float.Parse(data[3]),
                        RangeModifier = float.Parse(data[4]),
                        CriticalModifier = float.Parse(data[5]),
                        HealthModifier = float.Parse(data[6])
                    };

                    partsDict.Add(name, part);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error parsing part: {e.Message}");
                }
            }

            return partsDict;
        }
    }
}
