using System.Text.Json;

namespace ArenaShooter
{
    public partial class Form1 : Form
    {
        private Player player;
        private List<Entity> allEntities = new List<Entity>();
        HashSet<Keys> pressedKeys = new HashSet<Keys>();

        private Random random = new Random();
        private int spawnTimer = 0;
        private int fireCooldown = 0;

        private int currentScore;
        private GameData gameData = new GameData();
        private const string SaveFile = "savegame.json";

        public Form1()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            LoadGameData();
            InitializeGame();
        }

        private void InitializeGame()
        {
            allEntities.Clear();
            currentScore = 0;

            player = new Player(100, 100);
            allEntities.Add(player);

            timer1.Interval = 20;
            timer1.Start();
        }

        private void LoadGameData()
        {
            if (File.Exists(SaveFile))
            {
                string jsonString = File.ReadAllText(SaveFile);
                gameData = JsonSerializer.Deserialize<GameData>(jsonString);
            }
        }

        private void SaveGameData()
        {
            gameData.LastScore = currentScore;
            if (currentScore > gameData.HighScore)
            {
                gameData.HighScore = currentScore;
            }
            string jsonString = JsonSerializer.Serialize(gameData);
            File.WriteAllText(SaveFile, jsonString);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
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
            if (spawnTimer >= 50)
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
                            currentScore += 10;
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
            MessageBox.Show($"Game Over! Zombies ate your brain!\nScore: {currentScore}\nBest: {gameData.HighScore}");
            Application.Exit();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            foreach (var entity in allEntities)
            {
                entity.Draw(e.Graphics);
            }

            Font font = new Font("Arial", 10, FontStyle.Bold);
            float x = 20;
            e.Graphics.DrawString($"Score: {currentScore}", font, Brushes.Black, x, 20);
            e.Graphics.DrawString($"Last: {gameData.LastScore}", font, Brushes.Gray, x, 40);
            e.Graphics.DrawString($"Best: {gameData.HighScore}", font, Brushes.Gold, x, 60);
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

    public class GameData
    {
        public int HighScore { get; set; } = 0;
        public int LastScore { get; set; } = 0;
    }
}
