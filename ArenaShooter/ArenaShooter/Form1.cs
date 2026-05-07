using System;
using System.IO;
using System.Text.Json;

namespace ArenaShooter
{
    public partial class Form1 : Form
    {
        // 1. kolize zombies vùèi ostatním (h), pøidání pozadí do map (h) a asstù jako pøekážky
        // (h - easy) + zombíci musí obcházet pøekážky jako sami sebe (h), upravit hitboxy
        // pøekážek (h)
        // 2. uspoøádat soubory do složek, dodìlat dokumentaci - pøidat postup
        // a zdokumentovat nové assety do map
        // 3. udìlat si v kódu poøádek - rozdìlení do metod podle funkce
        // a odstranìní magic numbers („coding guidelines“ a „naming conventions“
        // podle jazyka C#)
        // 4. nauèit se na obhajobu co je to genericita, delegáty, animaèní vlákno atd.
        // 5. po obhajobì to smazat z gitu - je to chaos a nechci to tam

        private Player player;
        private Image playerImg;
        private Image enemyImg;
        private bool isMouseDown = false;
        private Point mousePos;

        private List<Entity> allEntities = new List<Entity>();
        HashSet<Keys> pressedKeys = new HashSet<Keys>();
        private List<Obstacle> obstacles = new List<Obstacle>();

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

        private TextureBrush bgBrush;
        private Image easyTreeImage;
        private Image easyBushImage;
        private Image easyWoodImage;

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

            this.BackColor = Color.White;
            Image tileImg = null;

            switch (currentDifficulty)
            {
                case "Easy":
                    spawnInterval = 40;
                    pointsPerKill = 5;
                    this.BackColor = Color.ForestGreen;
                    try { tileImg = Image.FromFile("Assets/tile_3.png"); } catch { }
                    break;
                case "Medium":
                    spawnInterval= 25;
                    pointsPerKill = 8;
                    this.BackColor = Color.DarkGray;
                    try { tileImg = Image.FromFile("Assets/tile_169.png"); } catch { }
                    break;
                case "Hard":
                    spawnInterval = 15;
                    pointsPerKill = 10;
                    this.BackColor = Color.DimGray;
                    try { tileImg = Image.FromFile("Assets/tile_6.png"); } catch { }
                    break;
            }

            if (tileImg != null)
            {
                bgBrush = new TextureBrush(tileImg);
            }
        }

        private void SetupMap()
        {
            obstacles.Clear();
            allEntities.RemoveAll(e => e is Obstacle);

            switch (currentDifficulty)
            {
                case "Easy": SetupEasyForest(); break;
                case "Medium": SetupMediumCity(); break;
                case "Hard": SetupHardGraveyard(); break;
            }

            foreach (var wall in obstacles)
            {
                allEntities.Add(wall);
            }
        }

        private void SetupEasyForest()
        {
            var screen = Screen.PrimaryScreen.Bounds;

            try
            {
                easyTreeImage = Image.FromFile("Assets/tile_183.png");
                easyBushImage = Image.FromFile("Assets/tile_210.png");
                easyWoodImage = Image.FromFile("Assets/tile_265.png");
            }
            catch { }

            for (int x = 100; x < screen.Width - 100; x += 200)
            {
                for (int y = 100; y < screen.Height - 100; y += 200)
                {
                    if (Math.Abs(x - screen.Width / 2) < 150 && Math.Abs(y - screen.Height / 2) < 150)
                        continue;

                    int offsetX = random.Next(-40, 40);
                    int offsetY = random.Next(-40, 40);

                    int type = random.Next(4);

                    if (type == 0 || type == 1)
                    {
                        obstacles.Add(new Obstacle(x + offsetX, y + offsetY, 64, 64, easyTreeImage));
                    }
                    else if (type == 2)
                    {
                        obstacles.Add(new Obstacle(x + offsetX, y + offsetY, 45, 45, easyBushImage));
                    }
                    else
                    {
                        obstacles.Add(new Obstacle(x + offsetX, y + offsetY, 50, 40, easyWoodImage));
                    }
                }
            }
        }

        private void SetupMediumCity()
        {

        }

        private void SetupHardGraveyard()
        {

        }

        private void InitializeGame()
        {
            Cursor.Hide();
            pressedKeys.Clear();
            allEntities.Clear();
            isMouseDown = false;
            currentScore = 0;
            spawnTimer = 0;

            player = new Player(0, 0, playerImg);
            var screenBounds = Screen.PrimaryScreen.Bounds;
            player.X = (screenBounds.Width / 2) - (player.Width / 2);
            player.Y = (screenBounds.Height / 2) - (player.Height / 2);

            allEntities.Add(player);
            SetupMap();

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

            float playerOldX = player.X;
            float playerOldY = player.Y;

            foreach (var entity in allEntities)
            {
                entity.Update(pressedKeys);
            }

            var enemies = allEntities.OfType<Enemy>().ToList();

            for (int i = 0; i < enemies.Count; i++)
            {
                foreach (var wall in obstacles)
                {
                    ApplyRepelForce(enemies[i], wall, 50);
                }

                for (int j = i + 1; j < enemies.Count; j++)
                {
                    ApplyRepelForce(enemies[i], enemies[j], 40);
                    ApplyRepelForce(enemies[j], enemies[i], 40);
                }
            }

            foreach (var entity in allEntities)
            {
                if (entity is Enemy enemy)
                {
                    HandleEntityWallCollision(enemy, enemy.X, enemy.Y);
                }
            }

            HandleEntityWallCollision(player, playerOldX, playerOldY);
            KeepPlayerInScreenBounds();
        }

        private void ApplyRepelForce(Entity toMove, Entity awayFrom, float repelDistance)
        {
            float dx = (toMove.X + toMove.Width / 2) - (awayFrom.X + awayFrom.Width / 2);
            float dy = (toMove.Y + toMove.Height / 2) - (awayFrom.Y + awayFrom.Height / 2);
            float dist = (float)Math.Sqrt(dx * dx + dy * dy);

            if (dist <  repelDistance && dist > 0)
            {
                toMove.X += (dx / dist) * (repelDistance - dist) * 0.15f;
                toMove.Y += (dy / dist) * (repelDistance - dist) * 0.15f;
            }
        }

        private void HandleEntityWallCollision(Entity entity, float oldX, float oldY)
        {
            foreach (var wall in obstacles)
            {
                if (entity.Bounds.IntersectsWith(wall.Bounds))
                {
                    entity.X = oldX;
                    entity.Y = oldY;
                    break;
                }
            }
        }

        private void KeepPlayerInScreenBounds() 
        {
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
            var bullets = allEntities.OfType<Bullet>().ToList();
            var enemies = allEntities.OfType<Enemy>().ToList();

            foreach (var enemy in enemies)
            {
                if (player.Bounds.IntersectsWith(enemy.Bounds))
                {
                    GameOver();
                    return;
                }

                foreach (var bullet in bullets)
                {
                    if (bullet.Bounds.IntersectsWith(enemy.Bounds))
                    {
                        toRemove.Add(enemy);
                        toRemove.Add(bullet);
                        currentScore += pointsPerKill;
                    }
                }
            }

            foreach (var bullet in bullets)
            {
                foreach (var wall in obstacles)
                {
                    if (bullet.Bounds.IntersectsWith(wall.Bounds))
                    {
                        toRemove.Add(bullet);
                        break;
                    }
                }

                if (IsOutOfBounds(bullet))
                {
                    toRemove.Add(bullet);
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

            if (bgBrush != null)
            {
                e.Graphics.FillRectangle(bgBrush, this.ClientRectangle);
            } else
            {
                using (Brush b = new SolidBrush(this.BackColor))
                {
                    e.Graphics.FillRectangle(b, this.ClientRectangle);
                }
            }

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
