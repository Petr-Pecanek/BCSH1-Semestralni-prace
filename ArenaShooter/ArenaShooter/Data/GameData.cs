using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArenaShooter.Data
{
    public class GameData
    {
        public Dictionary<string, DifficultyStats> Levels { get; set; } = new Dictionary<string, DifficultyStats>()
        {
            {"Easy", new DifficultyStats() },
            {"Medium", new DifficultyStats() },
            {"Hard", new DifficultyStats() }
        };
    }
}