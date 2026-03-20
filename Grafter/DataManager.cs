using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Grafter
{
    public static class DataManager
    {
        public static BindingList<PartDefinition> Parts { get; set; } = [];
        public static BindingList<BaseDefinition> Bases { get; set; } = [];

        public static string CurrentFilePath = "graft_library.json";

        public static void Save(string path)
        {
            try
            {
                CurrentFilePath = path;
                var library = new { Parts = Parts.ToList(), Bases = Bases.ToList() };

                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(library, options);
                File.WriteAllText(path, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Save failed: {ex.Message}");
            }
        }

        public static void Load(string path)
        {
            if (!File.Exists(path)) return;

            try
            {
                CurrentFilePath = path;
                string json = File.ReadAllText(path);

                GraftLibrary library = JsonSerializer.Deserialize<GraftLibrary>(json);

                if (library != null)
                {
                    Parts.Clear();
                    foreach (var limb in library.Parts) Parts.Add(limb);

                    Bases.Clear();
                    foreach (var b in library.Bases) Bases.Add(b);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load library: {ex.Message}");
            }
        }
    }
}