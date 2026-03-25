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
            allEntities.Add(new Enemy(400, 300));

            timer1.Interval = 20;
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            foreach (var entity in allEntities)
            {
                entity.Update(pressedKeys);
            }

            this.Invalidate();
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
