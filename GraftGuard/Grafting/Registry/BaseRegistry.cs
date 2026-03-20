using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GraftGuard.Grafting.Registry
{
    /// <summary>
    /// Collection holding all <see cref="PartDefinition"/>s currently loaded into the game
    /// </summary>
    internal static class BaseRegistry
    {
        private static Random random = new Random();
        private static List<BaseDefinition> _allBases = [];
        public static ReadOnlyCollection<BaseDefinition> Parts => _allBases.AsReadOnly();


        /// <summary>
        /// Loads the JSON library exported from Grafter and populates the registry.
        /// </summary>
        public static void LoadFromLibrary(ContentManager content, string jsonPath)
        {
            Debug.WriteLine("--- PART REGISTRY: STARTING LOAD ---");
            jsonPath = Path.Join(content.RootDirectory, jsonPath);
            Debug.WriteLine($"Attempting to load from: {Path.GetFullPath(jsonPath)}");

            if (!File.Exists(jsonPath)) return;

            string json = File.ReadAllText(jsonPath);
            var library = JsonSerializer.Deserialize<GraftLibrary>(json);

            if (library != null && library.Bases != null)
            {
                _allBases.Clear();

                foreach (var data in library.Bases)
                {
                    Debug.WriteLine($"Loading: {data.TextureName}!");
                    Texture2D tex = content.Load<Texture2D>($"Bases/{data.TextureName}");


                    // Create the definition with pivot data
                    var definition = new BaseDefinition(
                        data.Name,
                        data.TextureName,
                        tex,
                        data.IsTorso,
                        data.AttachPoints

                    );

                    _allBases.Add(definition);
                }
            }
        }

        /// <summary>
        /// Gets a <see cref="BaseDefinition"/> from the Registry by its name, case-insensitive
        /// </summary>
        /// <param name="name">Name of part to search for</param>
        /// <returns>Found <see cref="BaseDefinition"/></returns>
        public static BaseDefinition GetByName(string name)
        {
            BaseDefinition definition = _allBases.Find((part) => part.Name.ToLower() == name.ToLower());
            return definition;
        }

        /// <summary>
        /// Gets a random <see cref="BaseDefinition"/>
        /// </summary>
        /// <returns>Returns a random <see cref="BaseDefinition"/></returns>
        public static BaseDefinition GetRandom()
        {
            if( _allBases.Count == 0 ) return null;
            return _allBases[random.Next(_allBases.Count)];
        }

    }

}
