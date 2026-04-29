using System.Text.Json;
using System.IO;

namespace ArenaShooter
{
    public partial class Form1 : Form
    {
        private Player player;
        private Image playerImg;
        private Image enemyImg;
        private bool isMouseDown = false;
        private Point mousePos;

        private List<Entity> allEntities = new List<Entity>();
        HashSet<Keys> pressedKeys = new HashSet<Keys>();

        private Random random = new Random();
        private Font statsFont = new Font("Arial", 10, FontStyle.Bold);

        private string currentDifficulty = "Easy";
        private int spawnInterval = 40;
        private int spawnTimer;
        private int fireCooldown = 0;
        private int pointsPerKill = 5;

        private int currentScore;
        private GameData gameData = new GameData();
        private DifficultyStats currentStats;

        public Form1()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.Text = "Arena Shooter: Zombie Apocalypse";

            this.MouseDown += (s, e) => { if (e.Button == MouseButtons.Left) isMouseDown = true; };
            this.MouseUp += (s, e) => { if (e.Button == MouseButtons.Left) isMouseDown = false; };
            this.MouseMove += (s, e) => { mousePos = e.Location; };

            try
            {
                playerImg = Image.FromFile("Assets/soldier1_machine.png");
                enemyImg = Image.FromFile("Assets/zoimbie1_hold.png");
            } catch { }

            currentDifficulty = GameDialogs.ShowDifficultySelection();
            if (currentDifficulty == null)
            {
                Environment.Exit(0);
                return;
            }

            gameData = FileManager.Load();
            ApplyDifficultySettings();
            InitializeGame();
        }

        private void ApplyDifficultySettings()
        {
            this.WindowState = FormWindowState.Maximized;
            currentStats = gameData.Levels[currentDifficulty];

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
            Cursor.Hide();

            pressedKeys.Clear();
            allEntities.Clear();
            currentScore = 0;
            spawnTimer = 0;

            player = new Player(0, 0, playerImg);
            var screenBounds = Screen.PrimaryScreen.Bounds;
            player.X = (screenBounds.Width / 2) - (player.Width / 2);
            player.Y = (screenBounds.Height / 2) - (player.Height / 2);

            allEntities.Add(player);

            timer1.Interval = 20;
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (player == null) return;

            HandleEnemySpawning();
            if (isMouseDown)
            {
                HandleManualShooting();
            } else
            {
                if (fireCooldown < 10) fireCooldown++;
            }

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

            allEntities.Add(new Enemy(x, y, player, enemyImg));
        }

        private void HandleManualShooting()
        {
            fireCooldown++;
            if (fireCooldown >= 10)
            {
                allEntities.Add(new Bullet(
                    player.X + (player.Width / 2),
                    player.Y + (player.Height / 2),
                    mousePos.X,
                    mousePos.Y));

                fireCooldown = 0;
            }
        }

        private void UpdateEntities()
        {
            Point localMouse = this.PointToClient(Cursor.Position);
            player.UpdateRotation(localMouse);

            foreach (var entity in allEntities)
            {
                entity.Update(pressedKeys);
            }

            if (player.X < 0) player.X = 0;
            if (player.Y < 0) player.Y = 0;

            if (player.X + player.Width > this.ClientSize.Width)
            {
                player.X = this.ClientSize.Width - player.Width;
            }
            if (player.Y + player.Height > this.ClientSize.Height)
            {
                player.Y = this.ClientSize.Height - player.Height;
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

            Cursor.Show();

            currentStats.LastScore = currentScore;
            if (currentScore > currentStats.HighScore)
            {
                currentStats.HighScore = currentScore;
            }

            FileManager.Save(gameData);
            
            bool wantsRestart = GameDialogs.ShowGameOver(currentScore, currentStats.HighScore, currentDifficulty);
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

            float x = 20;
            e.Graphics.DrawString($"Difficulty: {currentDifficulty}", statsFont, Brushes.White, x, 20);
            e.Graphics.DrawString($"Score: {currentScore}", statsFont, Brushes.White, x, 40);
            e.Graphics.DrawString($"Last: {currentStats.LastScore}", statsFont, Brushes.Gray, x, 60);
            e.Graphics.DrawString($"Best: {currentStats.HighScore}", statsFont, Brushes.Gold, x, 80);

            if (!timer1.Enabled) return;

            using (Pen sightPen = new Pen(Color.Red, 2))
            {
                e.Graphics.DrawLine(sightPen, mousePos.X - 10, mousePos.Y, mousePos.X + 10, mousePos.Y);
                e.Graphics.DrawLine(sightPen, mousePos.X, mousePos.Y - 10, mousePos.X, mousePos.Y + 10);
                e.Graphics.DrawEllipse(sightPen, mousePos.X - 5, mousePos.Y - 5, 10, 10);
            }
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
