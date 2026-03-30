using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace GraftGuard.Data
{
    /// <summary>
    /// The Save Data Structure for JSON Serialization
    /// </summary>
    public class SaveData
    {
        public int HighScore { get; set; }
        public List<GameData> GameHistory { get; set; }

    }

    public static class PlayerData
    {
        public static GameState CurrentState { get; set; } = GameState.MainMenu;

        public static SaveData SaveData { get; set; }

        public static int HighScore { get; set; }
        public static List<GameData> GameHistory { get; set; }


        // The active game
        public static GameData CurrentGame { get; set; } = new GameData();

        private static string SaveFilePath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "save.json");

        public static void StartNewGame(float initialTimer)
        {
            CurrentGame = new GameData
            {
                Time = TimeState.Dawn,
                Timer = initialTimer
            };
            CurrentState = GameState.Game;
        }

        public static void RecordCurrentGame()
        {
            if (CurrentGame != null)
            {
                SaveData.GameHistory.Add(CurrentGame);
                if (CurrentGame.CurrentScore > SaveData.HighScore)
                    SaveData.HighScore = CurrentGame.CurrentScore;
            }
        }

        //for future
        public static void Save()
        {
            var data = new SaveData
            {
                HighScore = HighScore,
                GameHistory = GameHistory
            };

            string json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(SaveFilePath, json);
        }

        public static void Load()
        {
            if (!File.Exists(SaveFilePath)) return;

            try
            {
                string json = File.ReadAllText(SaveFilePath);
                var data = JsonSerializer.Deserialize<SaveData>(json);

                if (data != null)
                {
                    HighScore = data.HighScore;
                    GameHistory = data.GameHistory ?? new List<GameData>();
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to load save: {e.Message}");
            }
        }
    

    
    }
}
