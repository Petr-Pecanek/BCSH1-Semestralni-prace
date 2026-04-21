using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ArenaShooter
{
    public class DifficultyStats
    {
        public int HighScore { get; set; } = 0;
        public int LastScore { get; set; } = 0;
    }

    public class GameData
    {
        public Dictionary<string, DifficultyStats> Levels { get; set; } = new Dictionary<string, DifficultyStats>()
        {
            {"Easy", new DifficultyStats() },
            {"Medium", new DifficultyStats() },
            {"Hard", new DifficultyStats() }
        };
    }

    public static class FileManager
    {
        private const string SaveFile = "savegame.json";

        public static GameData Load()
        {
            if (!File.Exists(SaveFile)) return new GameData();

            try
            {
                string jsonString = File.ReadAllText(SaveFile);
                return JsonSerializer.Deserialize<GameData>(jsonString) ?? new GameData();
            } catch
            {
                return new GameData();
            }
        }

        public static void Save(GameData data)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(data, options);
            File.WriteAllText(SaveFile, jsonString);
        }
    }
}
