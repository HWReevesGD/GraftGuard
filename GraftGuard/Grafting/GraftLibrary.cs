using GraftGuard.Grafting.Registry;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;

namespace GraftGuard.Grafting
{
    public class GraftLibrary
    {
        public readonly static List<PartDefinition> AllParts = new();
        public readonly static List<BaseDefinition> AllBases = new();

        public List<PartDefinition> Parts { get; set; } = new();
        public List<BaseDefinition> Bases { get; set; } = new();

        private static readonly Random rng = new Random();

        public static void LoadLibrary(ContentManager content, string jsonPath)
        {
            using (Stream stream = TitleContainer.OpenStream(Path.Join(content.RootDirectory, jsonPath)))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    string json = File.ReadAllText(jsonPath);
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
        /// Returns a random PartDefinition from the loaded library.
        /// </summary>
        public static PartDefinition GetRandomPart()
        {
            if (AllParts.Count == 0) return null;
            return AllParts[rng.Next(AllParts.Count)];
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
