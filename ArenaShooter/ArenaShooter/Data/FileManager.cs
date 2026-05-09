using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace ArenaShooter.Data
{
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
            }
            catch
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