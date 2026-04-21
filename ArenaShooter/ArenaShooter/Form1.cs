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

        public Form1()
        {
            InitializeComponent();
            this.DoubleBuffered = true;

            this.Text = "Arena Shooter: Zombie Apocalypse";

            currentDifficulty = GameDialogs.ShowDifficultySelection();
            if (currentDifficulty == null)
            {
                Environment.Exit(0);
                return;
            }

            ApplyDifficultySettings();
            gameData = FileManager.Load();
            InitializeGame();
        }

        private void ApplyDifficultySettings()
        {
            this.WindowState = FormWindowState.Maximized;
            switch (currentDifficulty)
            {
                case "Easy":
                    spawnInterval = 40;
                    pointsPerKill = 5;
                    this.BackColor = Color.ForestGreen;
                    break;
                case "Medium":
                    spawnInterval= 25;
                    pointsPerKill = 8;
                    this.BackColor = Color.DarkGray;
                    break;
                case "Hard":
                    spawnInterval = 15;
                    pointsPerKill = 10;
                    this.BackColor = Color.DimGray;
                    break;
            }
        }

        private void InitializeGame()
        {
            allEntities.Clear();
            currentScore = 0;
            spawnTimer = 0;

            var screenBounds = Screen.PrimaryScreen.Bounds;
            float playerStartX = (screenBounds.Width / 2) - 20;
            float playerStartY = (screenBounds.Height / 2) - 20;

            player = new Player(playerStartX, playerStartY);
            allEntities.Add(player);

            timer1.Interval = 20;
            timer1.Start();
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

        private void GameOver()
        {
            timer1.Stop();

            var stats = gameData.Levels[currentDifficulty];
            stats.LastScore = currentScore;
            if (currentScore > stats.HighScore)
            {
                stats.HighScore = currentScore;
            }

            FileManager.Save(gameData);
            
            bool wantsRestart = GameDialogs.ShowGameOver(currentScore, stats.HighScore, currentDifficulty);
            if (wantsRestart)
            {
                InitializeGame();
            } else
            {
                Application.Exit();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (string.IsNullOrEmpty(currentDifficulty) || gameData == null) return;

            base.OnPaint(e);
            foreach (var entity in allEntities)
            {
                entity.Draw(e.Graphics);
            }

            var stats = gameData.Levels[currentDifficulty];
            Font font = new Font("Arial", 10, FontStyle.Bold);

            float x = 20;
            e.Graphics.DrawString($"Difficulty: {currentDifficulty}", font, Brushes.Black, x, 20);
            e.Graphics.DrawString($"Score: {currentScore}", font, Brushes.Black, x, 40);
            e.Graphics.DrawString($"Last: {stats.LastScore}", font, Brushes.Gray, x, 60);
            e.Graphics.DrawString($"Best: {stats.HighScore}", font, Brushes.Gold, x, 80);
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            pressedKeys.Add(e.KeyCode);
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            pressedKeys.Remove(e.KeyCode);
        }
    }
}
