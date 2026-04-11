namespace ArenaShooter
{
    public partial class Form1 : Form
    {
        private Player player;
        private List<Entity> allEntities = new List<Entity>();
        HashSet<Keys> pressedKeys = new HashSet<Keys>();

        public Form1()
        {
            InitializeComponent();
            this.DoubleBuffered = true;

            player = new Player(100, 100);
            allEntities.Add(player);
            allEntities.Add(new Enemy(400, 300, player));

            timer1.Interval = 20;
            timer1.Start();
        }

        int spawnTimer = 0;
        Random random = new Random();

        private void timer1_Tick(object sender, EventArgs e)
        {
            spawnTimer++;
            if (spawnTimer >= 100)
            {
                SpawnEnemy();
                spawnTimer = 0;
            }

            foreach (var entity in allEntities)
            {
                entity.Update(pressedKeys);

                if (entity is Enemy enemy && player.Bounds.IntersectsWith(enemy.Bounds))
                {
                    GameOver();
                    return;
                }
            }
            this.Invalidate();
        }

        private void GameOver()
        {
            timer1.Stop();
            MessageBox.Show("Game Over! Zombies ate your brain!");
            Application.Exit();
        }

        private void SpawnEnemy()
        {
            int side = random.Next(4);
            int x = 0, y = 0;

            switch(side)
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

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            foreach (var entity in allEntities)
            {
                entity.Draw(e.Graphics);
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
