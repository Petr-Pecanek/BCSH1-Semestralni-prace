using System.Text.Json;
using System.IO;

namespace ArenaShooter
{
    public partial class Form1 : Form
    {
        private Player player;
        private List<Entity> allEntities = new List<Entity>();
        HashSet<Keys> pressedKeys = new HashSet<Keys>();

        private Random random = new Random();
        private string currentDifficulty = "Easy";
        private int spawnInterval = 40;
        private int spawnTimer;
        private int fireCooldown = 0;
        private int pointsPerKill = 5;

        private int currentScore;
        private GameData gameData = new GameData();
        private const string SaveFile = "savegame.json";

        public Form1()
        {
            InitializeComponent();
            this.DoubleBuffered = true;

            using (DifficultyForm df = new DifficultyForm())
            {
                if (df.ShowDialog() == DialogResult.OK)
                {
                    currentDifficulty = df.SelectedDifficulty;
                    ApplyDifficultySettings();
                } else
                {
                    Application.Exit();
                    return;
                }
            }

            LoadGameData();
            InitializeGame();
        }

        private void ApplyDifficultySettings()
        {
            switch (currentDifficulty)
            {
                case "Easy":
                    spawnInterval = 40;
                    pointsPerKill = 5;
                    this.BackColor = Color.DarkGreen;
                    break;
                case "Medium":
                    spawnInterval= 25;
                    pointsPerKill = 8;
                    this.BackColor = Color.DimGray;
                    break;
                case "Hard":
                    spawnInterval = 15;
                    pointsPerKill = 10;
                    this.BackColor = Color.FromArgb(20, 20, 20);
                    break;
            }
        }

        private void InitializeGame()
        {
            allEntities.Clear();
            currentScore = 0;
            spawnTimer = 0;

            player = new Player(100, 100);
            allEntities.Add(player);

            timer1.Interval = 20;
            timer1.Start();
        }

        private void LoadGameData()
        {
            if (File.Exists(SaveFile))
            {
                try
                {
                    string jsonString = File.ReadAllText(SaveFile);
                    gameData = JsonSerializer.Deserialize<GameData>(jsonString);
                } catch
                {
                    gameData = new GameData();
                }
            }
        }

        private void SaveGameData()
        {
            var stats = gameData.Levels[currentDifficulty];

            stats.LastScore = currentScore;
            if (currentScore > stats.HighScore)
            {
                stats.HighScore = currentScore;
            }
            string jsonString = JsonSerializer.Serialize(gameData, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(SaveFile, jsonString);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (player == null) return;

            HandleEnemySpawning();
            HandleAutoShooting();

            UpdateEntities();

            ResolveCollisionsAndCleanup();

            this.Invalidate();
        }

        #region Herní Logika (Spawning & Shooting)
        private void HandleEnemySpawning()
        {
            spawnTimer++;
            if (spawnTimer >= spawnInterval)
            {
                SpawnEnemy();
                spawnTimer = 0;
            }
        }

        private void SpawnEnemy()
        {
            int side = random.Next(4);
            int x = 0, y = 0;

            switch (side)
            {
                case 0:
                    x = random.Next(0, this.ClientSize.Width);
                    y = -50;
                    break;
                case 1:
                    x = random.Next(0, this.ClientSize.Width);
                    y = this.ClientSize.Height + 50;
                    break;
                case 2:
                    x = -50;
                    y = random.Next(0, this.ClientSize.Height);
                    break;
                case 3:
                    x = this.ClientSize.Width + 50;
                    y = random.Next(0, this.ClientSize.Height);
                    break;
            }

            allEntities.Add(new Enemy(x, y, player));
        }

        private void HandleAutoShooting()
        {
            fireCooldown++;
            if (fireCooldown >= 10)
            {
                var localMouse = this.PointToClient(Cursor.Position);
                allEntities.Add(new Bullet(player.X + 15, player.Y + 15, localMouse.X, localMouse.Y));
                fireCooldown = 0;
            }
        }
        #endregion

        #region Aktualizace a Kolize
        private void UpdateEntities()
        {
            foreach (var entity in allEntities)
            {
                entity.Update(pressedKeys);
            }
        }

        private void ResolveCollisionsAndCleanup()
        {
            List<Entity> toRemove = new List<Entity>();

            foreach (var entity in allEntities)
            {

                if (entity is Enemy enemy)
                {
                    if (player.Bounds.IntersectsWith(enemy.Bounds))
                    {
                        GameOver();
                        return;
                    }

                    foreach (var bullet in allEntities.OfType<Bullet>())
                    {
                        if (bullet.Bounds.IntersectsWith(enemy.Bounds))
                        {
                            toRemove.Add(enemy);
                            toRemove.Add(bullet);
                            currentScore += pointsPerKill;
                        }
                    }
                }

                if (entity is Bullet b && IsOutOfBounds(b))
                {
                    toRemove.Add(b);
                }
            }

            foreach (var item in toRemove)
            {
                allEntities.Remove(item);
            }
        }

        private bool IsOutOfBounds(Bullet b)
        {
            return b.X < -100 || b.X > this.ClientSize.Width + 100 || b.Y < -100 || b.Y > this.ClientSize.Height + 100;
        }
        #endregion

        private void GameOver()
        {
            timer1.Stop();
            SaveGameData();

            var stats = gameData.Levels[currentDifficulty];
            MessageBox.Show($"Game Over! Zombies ate your brain!\nDifficulty: {currentDifficulty}\nScore: {currentScore}\nBest: {stats.HighScore}");
            Application.Exit();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            foreach (var entity in allEntities)
            {
                entity.Draw(e.Graphics);
            }

            var stats = gameData.Levels[currentDifficulty];
            Font font = new Font("Arial", 10, FontStyle.Bold);

            Brush textBrush = (currentDifficulty == "Hard") ? Brushes.White : Brushes.Black;

            float x = 20;
            e.Graphics.DrawString($"Difficulty: {currentDifficulty}", font, textBrush, x, 20);
            e.Graphics.DrawString($"Score: {currentScore}", font, textBrush, x, 40);
            e.Graphics.DrawString($"Last: {stats.LastScore}", font, Brushes.Gray, x, 60);
            e.Graphics.DrawString($"Best: {stats.HighScore}", font, Brushes.Gold, x, 80);
        }

        #region Vstupy (Klávesnice)
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            pressedKeys.Add(e.KeyCode);
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            pressedKeys.Remove(e.KeyCode);
        }
        #endregion
    }

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
    
    public class DifficultyForm : Form
    {
        public string SelectedDifficulty { get; private set; }

        public DifficultyForm()
        {
            this.Text = "Arena Shooter - Select Difficulty";
            this.Size = new Size(300, 250);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.ControlBox = false;

            Label lbl = new Label()
            {
                Text = "CHOOSE DIFFICULTY",
                Dock = DockStyle.Top,
                Height = 50,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Arial", 14, FontStyle.Bold)
            };

            Button btnEasy = CreateButton("Easy (Forest)", Color.LightGreen, 0);
            Button btnMed = CreateButton("Medium (City)", Color.LightGray, 1);
            Button btnHard = CreateButton("Hard (Graveyard)", Color.IndianRed, 2);

            btnEasy.Click += (s, e) => { SelectedDifficulty = "Easy"; this.DialogResult = DialogResult.OK; };
            btnMed.Click += (s, e) => { SelectedDifficulty = "Medium"; this.DialogResult = DialogResult.OK; };
            btnHard.Click += (s, e) => { SelectedDifficulty = "Hard"; this.DialogResult = DialogResult.OK; };

            this.Controls.Add(btnHard);
            this.Controls.Add(btnMed);
            this.Controls.Add(btnEasy);
            this.Controls.Add(lbl);
        }

        private Button CreateButton(string text, Color color, int index)
        {
            return new Button()
            {
                Text = text,
                Dock = DockStyle.Top,
                Height = 45,
                BackColor = color,
                FlatStyle = FlatStyle.Flat
            };
        }
    }
}
