using System.ComponentModel;
using System.Text.Json;

namespace Grafter
{
    public interface ITextureEditor
    {
        string TextureName { get; set; }
        string FullImagePath { get; set; }
    }
    public static class DataManager
    {
        public static BindingList<PartDefinition> Parts { get; set; } = [];
        public static BindingList<BaseDefinition> Bases { get; set; } = [];
        public static string[] Behaviors = [];

        public static string ProjectContentPath
        {
            get => Properties.Settings.Default.ContentPath;
            set
            {
                Properties.Settings.Default.ContentPath = value;
                Properties.Settings.Default.Save(); 
            }
        }

        public static string ProjectRootPath
        {
            get => Properties.Settings.Default.MgcbPath;
            set
            {
                Properties.Settings.Default.MgcbPath = value;
                Properties.Settings.Default.Save();
            }
        }

        public static string CurrentFilePath = "graft_library.json";
        public const string RelativeBehaviorPath = "../../../../GraftGuard/Content/part_behaviors.json";

        public static void Save(string path)
        {
            try
            {
                CurrentFilePath = path;
                var library = new { Parts = Parts.ToList(), Bases = Bases.ToList() };

                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(library, options);
                File.WriteAllText(path, json);

                Properties.Settings.Default.LastLibraryPath = path;
                Properties.Settings.Default.Save();
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
                string json = File.ReadAllText(path);
                GraftLibrary library = JsonSerializer.Deserialize<GraftLibrary>(json);

                if (library != null)
                {
                    CurrentFilePath = path; 

                    Properties.Settings.Default.LastLibraryPath = path;
                    Properties.Settings.Default.Save();

                    Parts.Clear();
                    foreach (var limb in library.Parts) Parts.Add(limb);

                    Bases.Clear();
                    foreach (var b in library.Bases) Bases.Add(b);
                }
            }
            catch (Exception ex)
            {
                // If it's unparsable or corrupted, clear the setting so we don't 
                // try to load a broken file forever.
                Properties.Settings.Default.LastLibraryPath = "";
                Properties.Settings.Default.Save();
                MessageBox.Show($"Failed to load library: {ex.Message}");
            }
        }

        public static void LoadBehaviors()
        {
            try
            {
                string json = File.ReadAllText(RelativeBehaviorPath);

                Behaviors = JsonSerializer.Deserialize<string[]>(json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load behaviors: {ex.Message}");
            }
        }
    }
}