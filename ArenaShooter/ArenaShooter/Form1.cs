namespace ArenaShooter
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.DoubleBuffered = true; // vykreslování bez blikání
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            // timer enable, interval 20 - pohyb hráèe, PC a let støel 
            // vše co tu bude se provede 50x za sekundu (fps)
        }
    }
}
