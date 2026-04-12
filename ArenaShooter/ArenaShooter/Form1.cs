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

        public Form1()
        {
            InitializeComponent();
            this.DoubleBuffered = true;

            InitializeGame();
        }

        private void InitializeGame()
        {
            player = new Player(100, 100);
            allEntities.Add(player);

            timer1.Interval = 20;
            timer1.Start();
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
            return b.X < -100 || b.X > 2000 || b.Y < -100 || b.Y > 2000;
        }
        #endregion

        private void GameOver()
        {
            timer1.Stop();
            MessageBox.Show("Game Over! Zombies ate your brain!");
            Application.Exit();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            foreach (var entity in allEntities)
            {
                entity.Draw(e.Graphics);
            }
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
}
