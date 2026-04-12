using GraftGuard.Grafting.Registry;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace GraftGuard.Grafting
{
    public class GraftLibrary
    {
        public readonly static List<PartDefinition> AllParts = [];
        public readonly static List<BaseDefinition> AllBases = [];
        public static List<PartDefinition> Limbs { get; private set; } = [];
        public static List<PartDefinition> Heads { get; private set; } = [];

        public List<PartDefinition> Parts { get; set; } = [];
        public List<BaseDefinition> Bases { get; set; } = [];

        private static readonly Random rng = new Random();

        public static void LoadLibrary(ContentManager content, string jsonPath)
        {
            using (Stream stream = TitleContainer.OpenStream(Path.Join(content.RootDirectory, jsonPath)))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    string json = reader.ReadToEnd();
                    var library = JsonSerializer.Deserialize<GraftLibrary>(json);


                    if (library == null) return;

                    // --- Process Parts ---
                    if (library.Parts != null)
                    {
                        AllParts.Clear();
                        foreach (var data in library.Parts)
                        {
                            Texture2D tex = content.Load<Texture2D>($"Parts/{data.TextureName}");

                            data.Texture = tex;
                            AllParts.Add(data);

                            Debug.WriteLine($"Loaded Part: {data.Name}");
                        }
                    }

                    // --- Process Bases ---
                    if (library.Bases != null)
                    {
                        AllBases.Clear();
                        foreach (var data in library.Bases)
                        {
                            // Load the texture for the base
                            Texture2D tex = content.Load<Texture2D>($"Parts/{data.TextureName}");

                            data.Texture = tex;
                            AllBases.Add(data);

                            Debug.WriteLine($"Loaded Base: {data.Name}");
                        }
                    }
                }
            }
            CalculatePartPools();
        }

        public static void CalculatePartPools()
        {
            Limbs = AllParts.Where((part) => part.Type == PartType.Limb).ToList();

            Heads = AllParts.Where((part) => part.Type == PartType.Head).ToList();
        }

        /// <summary>
        /// Gets a <see cref="PartDefinition"/> from the Registry by its name, case-insensitive
        /// </summary>
        /// <param name="name">Name of part to search for</param>
        /// <returns>Found <see cref="PartDefinition"/></returns>
        public static PartDefinition GetPartByName(string name)
        {
            PartDefinition definition = AllParts.Find((part) => part.Name.ToLower() == name.ToLower());
            return definition;
        }

        /// <summary>
        /// Gets a <see cref="BaseDefinition"/> from the Registry by its name, case-insensitive
        /// </summary>
        /// <param name="name">Name of part to search for</param>
        /// <returns>Found <see cref="BaseDefinition"/></returns>
        public static BaseDefinition GetBaseByName(string name)
        {
            BaseDefinition definition = AllBases.Find((part) => part.Name.ToLower() == name.ToLower());
            return definition;
        }

        /// <summary>
        /// Returns a random PartDefinition from the loaded library, using their rarities as weights
        /// </summary>
        public static PartDefinition GetRandomPart() => GetRandomPartFromList(AllParts);
        /// <summary>
        /// Returns a random PartDefinition from the loaded library's limbs, using their rarities as weights
        /// </summary>
        public static PartDefinition GetRandomLimb() => GetRandomPartFromList(Limbs);
        /// <summary>
        /// Returns a random PartDefinition from the loaded library's heads, using their rarities as weights
        /// </summary>
        public static PartDefinition GetRandomHead() => GetRandomPartFromList(Heads);

        /// <summary>
        /// Returns a random PartDefinition from the given list, using their rarities as weights
        /// </summary>
        private static PartDefinition GetRandomPartFromList(List<PartDefinition> parts)
        {
            if (parts.Count == 0) return null;

            // Combined weights of all parts
            float totalWeight = parts.Sum((part) => part.RarityWeight);
            // When we reach this number, we choose this part
            float requiredWeight = rng.NextSingle() * totalWeight;
            // The sum of all of the weights before this number, we compare this against the required weight
            float cumulativeWeight = 0.0f;

            foreach (PartDefinition part in parts)
            {
                cumulativeWeight += part.RarityWeight;
                if (cumulativeWeight > requiredWeight)
                {
                    return part;
                }
            }

            // This should'nt be reachable, but I'll leave it here as a fallback
            return parts[rng.Next(parts.Count)];
        }

        /// <summary>
        /// Returns a random BaseDefinition from the loaded library.
        /// </summary>
        public static BaseDefinition GetRandomBase()
        {
            if (AllBases.Count == 0) return null;
            return AllBases[rng.Next(AllBases.Count)];
        }
    }


}
